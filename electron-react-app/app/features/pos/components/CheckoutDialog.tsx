import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Input } from '@/app/components/ui/input';
import { Separator } from '@/app/components/ui/separator';
import { CheckCircle2, Loader2, Receipt, X } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { useCompleteSale } from '../hooks/usePosActions';
import { formatPrice } from '@/lib/utils';
import { CompletedSaleDto, SaleDto } from '../lib/types';

interface CheckoutDialogProps {
    open: boolean;
    onClose: () => void;
    onSuccess: () => void;
    draftSale: SaleDto | null;
}

export function CheckoutDialog({ open, onClose, onSuccess, draftSale }: CheckoutDialogProps) {
    const [amountPaid, setAmountPaid] = useState('');
    const [completedSale, setCompletedSale] = useState<CompletedSaleDto | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);
    const [hasSubmitted, setHasSubmitted] = useState(false);

    const inventoryId = useCartStore(state => state.inventoryId);
    const completeSale = useCompleteSale();

    // Reset state when dialog opens
    useEffect(() => {
        if (open) {
            console.log('💳 Checkout Dialog - Opened with data:', {
                saleId: draftSale?.saleId,
                itemsCount: draftSale?.items?.length,
                grandTotal: draftSale?.grandTotal?.amount
            });
            setAmountPaid('');
            setCompletedSale(null);
            setIsProcessing(false);
            setHasSubmitted(false);
        }
    }, [open, draftSale]);

    const total = draftSale?.grandTotal.amount || 0;
    const totalDiscount = draftSale?.totalDiscount || { amount: 0, currency: 'IQD' };
    const amountPaidNum = parseFloat(amountPaid) || 0;
    const change = amountPaidNum - total;

    const handleOpenChange = (isOpen: boolean) => {
        if (!isOpen && !isProcessing) {
            setAmountPaid('');
            setCompletedSale(null);
            onClose();
        }
    };

    const handleCompleteSale = async () => {
        console.log('💳 Checkout - Complete Sale Called', {
            hasSubmitted,
            isProcessing,
            isPending: completeSale.isPending,
            saleId: draftSale?.saleId,
            inventoryId
        });

        // CRITICAL: Prevent duplicate submissions with multiple guards
        if (hasSubmitted || isProcessing || completeSale.isPending) {
            console.log('💳 Checkout - BLOCKED by guard', { hasSubmitted, isProcessing, isPending: completeSale.isPending });
            return;
        }
        
        if (!inventoryId || !draftSale) {
            console.log('💳 Checkout - BLOCKED missing data', { hasInventoryId: !!inventoryId, hasDraftSale: !!draftSale });
            return;
        }

        if (amountPaidNum < total) {
            console.log('💳 Checkout - BLOCKED insufficient payment', { paid: amountPaidNum, total });
            return; // Amount paid is less than total
        }

        console.log('💳 Checkout - Proceeding with completion', { saleId: draftSale.saleId });

        // Set guards immediately
        setHasSubmitted(true);
        setIsProcessing(true);

        try {
            console.log('💳 Checkout - Calling completeSale API...');
            const completed = await completeSale.mutateAsync({
                saleId: draftSale.saleId,
                inventoryId,
                amountPaid: amountPaidNum
            });

            console.log('💳 Checkout - Sale completed successfully', { 
                completedSaleId: completed?.saleId,
                saleNumber: completed?.saleNumber 
            });
            setCompletedSale(completed ?? null);
        } catch (error) {
            console.error('💳 Checkout - Error completing sale:', error);
            // On error, allow retry
            setHasSubmitted(false);
            // Error is handled by the interceptor
        } finally {
            setIsProcessing(false);
        }
    };

    const handleFinish = () => {
        setAmountPaid('');
        setCompletedSale(null);
        onSuccess();
        onClose();
    };

    // Show success screen if sale is completed
    if (completedSale) {
        return (
            <Dialog open={open} onOpenChange={handleOpenChange}>
                <DialogContent className="max-w-md">
                    <DialogHeader>
                        <div className="flex flex-col items-center gap-4 py-4">
                            <div className="rounded-full bg-green-100 p-3">
                                <CheckCircle2 className="h-12 w-12 text-green-600" />
                            </div>
                            <DialogTitle className="text-2xl text-center">
                                تمت عملية البيع بنجاح!
                            </DialogTitle>
                        </div>
                    </DialogHeader>

                    <div className="space-y-4">
                        <div className="bg-muted p-4 rounded-lg space-y-2">
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">رقم البيع:</span>
                                <span className="font-semibold">{completedSale.saleNumber}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">التاريخ:</span>
                                <span className="font-semibold">
                                    {new Date(completedSale.saleDate).toLocaleDateString('ar')}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">عدد الأصناف:</span>
                                <span className="font-semibold">{completedSale.totalItems}</span>
                            </div>
                        </div>

                        <Separator />

                        <div className="space-y-2">
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
                            {completedSale.change.amount > 0 && (
                                <div className="flex justify-between text-lg text-green-600">
                                    <span>الباقي:</span>
                                    <span className="font-bold">
                                        {formatPrice(completedSale.change)}
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>

                    <DialogFooter className="gap-2">
                        <Button variant="outline" onClick={handleFinish} className="flex-1 gap-2">
                            <Receipt className="h-4 w-4" />
                            طباعة الفاتورة
                        </Button>
                        <Button onClick={handleFinish} className="flex-1">
                            إنهاء
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        );
    }

    // Show checkout form
    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <div className="flex items-center justify-between">
                        <DialogTitle className="text-2xl">إتمام الشراء</DialogTitle>
                        <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => handleOpenChange(false)}
                            disabled={isProcessing}
                        >
                            <X className="h-4 w-4" />
                        </Button>
                    </div>
                </DialogHeader>

                <div className="space-y-6">
                    {/* Items List with Stock */}
                    {draftSale && draftSale.items.length > 0 && (
                        <div className="space-y-2">
                            <label className="text-sm font-medium">المنتجات:</label>
                            <div className="bg-muted p-3 rounded-lg space-y-2 max-h-48 overflow-y-auto">
                                {draftSale.items.map((item) => (
                                    <div key={item.itemId} className="flex items-center justify-between text-sm bg-background p-2 rounded">
                                        <div className="flex-1">
                                            <div className="font-medium">{item.productName}</div>
                                            <div className="text-xs text-muted-foreground flex items-center gap-2">
                                                <span>الكمية: {item.quantity}</span>
                                                <span>•</span>
                                                <span className={item.availableStock < item.quantity ? 'text-destructive font-semibold' : 'text-green-600'}>
                                                    المتوفر: {item.availableStock}
                                                </span>
                                            </div>
                                        </div>
                                        <div className="text-right">
                                            <div className="font-semibold">{formatPrice(item.lineTotal)}</div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {/* Sale Summary */}
                    <div className="bg-muted p-4 rounded-lg space-y-2">
                        <div className="flex justify-between">
                            <span className="text-muted-foreground">عدد الأصناف:</span>
                            <span className="font-semibold">{draftSale?.totalItems || 0}</span>
                        </div>
                        {totalDiscount.amount > 0 && (
                            <div className="flex justify-between text-green-600">
                                <span>الخصم:</span>
                                <span className="font-semibold">-{formatPrice(totalDiscount)}</span>
                            </div>
                        )}
                        <Separator />
                        <div className="flex justify-between text-lg font-bold">
                            <span>الإجمالي:</span>
                            <span className="text-primary">{formatPrice({ amount: total, currency: 'IQD' })}</span>
                        </div>
                    </div>

                    {/* Amount Paid Input */}
                    <div className="space-y-3">
                        <label className="text-sm font-medium">المبلغ المدفوع:</label>
                        <Input
                            type="number"
                            placeholder="0.00"
                            value={amountPaid}
                            onChange={(e) => setAmountPaid(e.target.value)}
                            className="text-2xl text-center font-bold"
                            autoFocus
                            disabled={isProcessing}
                        />

                        {/* Quick Amount Buttons */}
                        <div className="grid grid-cols-4 gap-2">
                            {[1000, 5000, 10000, 20000].map((amount) => (
                                <Button
                                    key={amount}
                                    variant="outline"
                                    size="sm"
                                    onClick={() => setAmountPaid(String(amount))}
                                    disabled={isProcessing}
                                >
                                    {amount.toLocaleString()}
                                </Button>
                            ))}
                        </div>
                        <Button
                            variant="outline"
                            size="sm"
                            className="w-full"
                            onClick={() => setAmountPaid(String(total))}
                            disabled={isProcessing}
                        >
                            المبلغ المضبوط
                        </Button>
                    </div>

                    {/* Change Display */}
                    {amountPaidNum > 0 && (
                        <div className="p-4 bg-muted rounded-lg">
                            {amountPaidNum < total ? (
                                <div className="text-center text-destructive">
                                    <p className="text-sm font-medium">المبلغ غير كافٍ</p>
                                    <p className="text-xs">
                                        المتبقي: {formatPrice({ amount: total - amountPaidNum, currency: 'IQD' })}
                                    </p>
                                </div>
                            ) : change > 0 ? (
                                <div className="text-center">
                                    <p className="text-sm text-muted-foreground">الباقي:</p>
                                    <p className="text-2xl font-bold text-green-600">
                                        {formatPrice({ amount: change, currency: 'IQD' })}
                                    </p>
                                </div>
                            ) : (
                                <div className="text-center">
                                    <p className="text-sm font-medium text-green-600">
                                        ✓ المبلغ المضبوط
                                    </p>
                                </div>
                            )}
                        </div>
                    )}
                </div>

                <DialogFooter className="gap-2">
                    <Button
                        variant="outline"
                        onClick={() => handleOpenChange(false)}
                        disabled={isProcessing}
                    >
                        إلغاء
                    </Button>
                    <Button
                        onClick={handleCompleteSale}
                        disabled={amountPaidNum < total || isProcessing || completeSale.isPending || hasSubmitted || !draftSale}
                        className="flex-1 gap-2"
                        size="lg"
                    >
                        {(isProcessing || completeSale.isPending || hasSubmitted) ? (
                            <>
                                <Loader2 className="h-5 w-5 animate-spin" />
                                جاري المعالجة...
                            </>
                        ) : (
                            <>
                                <CheckCircle2 className="h-5 w-5" />
                                إتمام البيع
                            </>
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
