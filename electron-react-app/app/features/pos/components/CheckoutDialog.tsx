import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Input } from '@/app/components/ui/input';
import { Separator } from '@/app/components/ui/separator';
import { CheckCircle, Loader2, CreditCard, DollarSign } from 'lucide-react';
import { formatPrice } from '@/lib/utils';
import { useCartStore } from '../stores/cartStore';
import { useCreateAndCompleteSale } from '../hooks/usePosActions';
import { CompletedSaleDto } from '../lib/types';
import { toast } from 'sonner';

interface CheckoutDialogProps {
    open: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

export function CheckoutDialog({ open, onClose, onSuccess }: CheckoutDialogProps) {
    const inventoryId = useCartStore(state => state.inventoryId);
    const items = useCartStore(state => state.items);
    const getTotal = useCartStore(state => state.getTotal);
    const getTotalItems = useCartStore(state => state.getTotalItems);
    const getCurrency = useCartStore(state => state.getCurrency);
    const clearCart = useCartStore(state => state.clearCart);
    
    const [amountPaid, setAmountPaid] = useState('');
    const [completedSale, setCompletedSale] = useState<CompletedSaleDto | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [hasSubmitted, setHasSubmitted] = useState(false);

    const createAndCompleteSale = useCreateAndCompleteSale();
    
    const total = getTotal();
    const totalItems = getTotalItems();
    const currency = getCurrency();
    const amountPaidNum = parseFloat(amountPaid) || 0;
    const change = amountPaidNum - total;

    // Reset state when dialog opens/closes
    useEffect(() => {
        if (open) {
            setAmountPaid('');
            setCompletedSale(null);
            setIsProcessing(false);
            setHasSubmitted(false);
        }
    }, [open]);

    const handleCompleteSale = async () => {
        // CRITICAL: Prevent duplicate submissions
        if (hasSubmitted || isProcessing || createAndCompleteSale.isPending) {
            return;
        }
        
        if (!inventoryId) {
            return;
        }

        if (amountPaidNum < total) {
            return; // Amount paid is less than total
        }

        // Set guards immediately
        setHasSubmitted(true);
        setIsProcessing(true);

        try {
            // Validate cart items have required fields
            if (items.length === 0) {
                toast.error('السلة فارغة');
                setIsProcessing(false);
                setHasSubmitted(false);
                return;
            }
            
            // Check if all items have productPackagingId
            const invalidItems = items.filter(item => !item.productPackagingId);
            if (invalidItems.length > 0) {
                toast.error('خطأ في بيانات السلة، يرجى حذف العناصر وإضافتها مرة أخرى');
                setIsProcessing(false);
                setHasSubmitted(false);
                return;
            }
            
            // Convert local cart items to SaleItemInput format
            const saleItems = items.map(item => ({
                productPackagingId: item.productPackagingId,
                quantity: item.quantity,
                discount: item.discount
                    ? {
                        amount: item.discount.value,
                        isPercentage: item.discount.type === 1,
                    }
                    : undefined,
            }));

            // Create and complete sale in one transaction
            const completed = await createAndCompleteSale.mutateAsync({
                inventoryId,
                items: saleItems,
                amountPaid: amountPaidNum,
            });

            setCompletedSale(completed);
            
            // Clear local cart
            clearCart();
            
            // Call success callback
            setTimeout(() => {
                onSuccess();
            }, 2000); // Show success screen for 2 seconds
        } catch {
            setIsProcessing(false);
            setHasSubmitted(false);
        }
    };

    const handleSetExactAmount = () => {
        setAmountPaid(total.toString());
    };

    const handleQuickAmount = (amount: number) => {
        setAmountPaid(amount.toString());
    };

    const handleClose = () => {
        if (completedSale) {
            // If sale completed, trigger success callback
            onSuccess();
        } else {
            onClose();
        }
    };

    // Success Screen
    if (completedSale) {
        return (
            <Dialog open={open} onOpenChange={handleClose}>
                <DialogContent className="max-w-md">
                    <div className="flex flex-col items-center justify-center space-y-4 py-8">
                        <div className="rounded-full bg-green-100 p-4">
                            <CheckCircle className="h-16 w-16 text-green-600" />
                        </div>
                        <h2 className="text-2xl font-bold text-center">
                            تمت عملية البيع بنجاح!
                        </h2>
                        
                        <Separator />
                        
                        <div className="w-full space-y-2 text-sm">
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">رقم البيع:</span>
                                <span className="font-medium">{completedSale.saleNumber}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">التاريخ:</span>
                                <span className="font-medium">
                                    {new Date(completedSale.saleDate).toLocaleDateString('ar-IQ')}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">عدد الأصناف:</span>
                                <span className="font-medium">{completedSale.totalItems}</span>
                            </div>
                        </div>

                        <Separator />

                        <div className="w-full space-y-2">
                            <div className="flex justify-between text-lg">
                                <span>الإجمالي:</span>
                                <span className="font-bold">
                                    {formatPrice(completedSale.grandTotal)}
                                </span>
                            </div>
                            <div className="flex justify-between text-lg">
                                <span>المبلغ المدفوع:</span>
                                <span className="font-bold">
                                    {formatPrice(completedSale.amountPaid)}
                                </span>
                            </div>
                            <div className="flex justify-between text-xl text-green-600">
                                <span className="font-semibold">الباقي:</span>
                                <span className="font-bold">
                                    {formatPrice(completedSale.change)}
                                </span>
                            </div>
                        </div>

                        <Button onClick={handleClose} className="w-full" size="lg">
                            إنهاء
                        </Button>
                    </div>
                </DialogContent>
            </Dialog>
        );
    }

    // Checkout Screen
    return (
        <Dialog open={open} onOpenChange={handleClose}>
            <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-2xl">إتمام الشراء</DialogTitle>
                </DialogHeader>

                <div className="space-y-6">
                    {/* Order Summary */}
                    <div className="bg-muted p-4 rounded-lg space-y-3">
                        <h3 className="font-semibold mb-3">المنتجات:</h3>
                        <div className="max-h-64 overflow-y-auto space-y-2">
                            {items.map((item) => (
                                <div
                                    key={item.productPackagingId}
                                    className="flex justify-between items-start text-sm"
                                >
                                    <div className="flex-1">
                                        <div className="font-medium">{item.productName}</div>
                                        <div className="text-muted-foreground text-xs">
                                            {item.packagingName} • الكمية: {item.quantity} • المتوفر: {item.availableStock}
                                        </div>
                                    </div>
                                    <div className="font-semibold">
                                        {formatPrice({
                                            amount: item.unitPrice.amount * item.quantity,
                                            currency: item.unitPrice.currency,
                                        })}
                                    </div>
                                </div>
                            ))}
                        </div>

                        <Separator />

                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">عدد الأصناف:</span>
                                <span className="font-medium">{totalItems}</span>
                            </div>
                            <div className="flex justify-between text-lg font-bold">
                                <span>الإجمالي:</span>
                                <span className="text-primary">
                                    {formatPrice({ amount: total, currency })}
                                </span>
                            </div>
                        </div>
                    </div>

                    {/* Payment Section */}
                    <div className="space-y-4">
                        <div>
                            <label className="text-sm font-medium mb-2 block">
                                المبلغ المدفوع:
                            </label>
                            <div className="relative">
                                <DollarSign className="absolute right-3 top-1/2 -translate-y-1/2 h-5 w-5 text-muted-foreground" />
                                <Input
                                    type="number"
                                    value={amountPaid}
                                    onChange={(e) => setAmountPaid(e.target.value)}
                                    placeholder="أدخل المبلغ المدفوع"
                                    className="pr-10 text-lg"
                                    disabled={isProcessing}
                                />
                            </div>
                        </div>

                        {/* Quick Amount Buttons */}
                        <div className="grid grid-cols-5 gap-2">
                            <Button
                                variant="outline"
                                onClick={() => handleQuickAmount(1000)}
                                disabled={isProcessing}
                            >
                                1,000
                            </Button>
                            <Button
                                variant="outline"
                                onClick={() => handleQuickAmount(5000)}
                                disabled={isProcessing}
                            >
                                5,000
                            </Button>
                            <Button
                                variant="outline"
                                onClick={() => handleQuickAmount(10000)}
                                disabled={isProcessing}
                            >
                                10,000
                            </Button>
                            <Button
                                variant="outline"
                                onClick={() => handleQuickAmount(20000)}
                                disabled={isProcessing}
                            >
                                20,000
                            </Button>
                            <Button
                                variant="outline"
                                onClick={handleSetExactAmount}
                                disabled={isProcessing}
                            >
                                المبلغ المضبوط
                            </Button>
                        </div>

                        {/* Change Display */}
                        {amountPaidNum > 0 && (
                            <div className={`p-4 rounded-lg ${
                                change >= 0 ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'
                            }`}>
                                <div className="flex justify-between items-center">
                                    <span className="font-medium">
                                        {change >= 0 ? 'الباقي:' : 'المبلغ غير كافٍ:'}
                                    </span>
                                    <span className={`text-2xl font-bold ${
                                        change >= 0 ? 'text-green-600' : 'text-red-600'
                                    }`}>
                                        {formatPrice({ amount: Math.abs(change), currency })}
                                    </span>
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Action Buttons */}
                    <div className="flex gap-2">
                        <Button
                            variant="outline"
                            onClick={handleClose}
                            disabled={isProcessing || hasSubmitted}
                            className="flex-1"
                        >
                            إلغاء
                        </Button>
                        <Button
                            onClick={handleCompleteSale}
                            disabled={
                                amountPaidNum < total ||
                                isProcessing ||
                                hasSubmitted ||
                                createAndCompleteSale.isPending
                            }
                            className="flex-1"
                            size="lg"
                        >
                            {isProcessing || createAndCompleteSale.isPending ? (
                                <>
                                    <Loader2 className="ml-2 h-5 w-5 animate-spin" />
                                    جاري المعالجة...
                                </>
                            ) : (
                                <>
                                    <CreditCard className="ml-2 h-5 w-5" />
                                    إتمام البيع
                                </>
                            )}
                        </Button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    );
}
