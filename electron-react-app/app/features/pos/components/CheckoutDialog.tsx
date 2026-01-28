import { useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Input } from '@/app/components/ui/input';
import { Badge } from '@/app/components/ui/badge';
import { Separator } from '@/app/components/ui/separator';
import { CheckCircle2, Loader2, Receipt, X } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { useCreateSale, useCompleteSale } from '../hooks/usePosActions';
import { formatPrice } from '@/lib/utils';
import { CompletedSaleDto } from '../lib/types';

interface CheckoutDialogProps {
    open: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

export function CheckoutDialog({ open, onClose, onSuccess }: CheckoutDialogProps) {
    const [amountPaid, setAmountPaid] = useState('');
    const [completedSale, setCompletedSale] = useState<CompletedSaleDto | null>(null);
    const [isProcessing, setIsProcessing] = useState(false);

    const items = useCartStore(state => state.items);
    const inventoryId = useCartStore(state => state.inventoryId);
    const clearCart = useCartStore(state => state.clearCart);
    const getTotal = useCartStore(state => state.getTotal);
    const getTotalDiscount = useCartStore(state => state.getTotalDiscount);

    const createSale = useCreateSale();
    const completeSale = useCompleteSale();

    const total = getTotal();
    const totalDiscount = getTotalDiscount();
    const amountPaidNum = parseFloat(amountPaid) || 0;
    const change = amountPaidNum - total.amount;

    const handleOpenChange = (isOpen: boolean) => {
        if (!isOpen && !isProcessing) {
            setAmountPaid('');
            setCompletedSale(null);
            onClose();
        }
    };

    const handleCompleteSale = async () => {
        if (!inventoryId || items.length === 0) return;

        if (amountPaidNum < total.amount) {
            return; // Amount paid is less than total
        }

        setIsProcessing(true);

        try {
            // Step 1: Create sale (draft)
            const saleId = await createSale.mutateAsync({
                inventoryId,
                items: items.map(item => ({
                    productPackagingId: item.packagingId,
                    quantity: item.quantity,
                    discount: item.discount ? {
                        amount: item.discount.value,
                        isPercentage: item.discount.type === 1
                    } : undefined
                })),
                notes: undefined
            });

            // Step 2: Complete sale with payment
            const completed = await completeSale.mutateAsync({
                saleId: saleId ?? '',
                inventoryId,
                amountPaid: amountPaidNum
            });

            // Step 3: Show success
            setCompletedSale(completed ?? null);
            clearCart();

        } catch (error) {
            // Error is handled by the interceptor
            setIsProcessing(false);
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

    // Success view
    if (completedSale) {
        return (
            <Dialog open={open} onOpenChange={handleOpenChange}>
                <DialogContent className="max-w-md">
                    <DialogHeader>
                        <div className="flex items-center justify-center mb-4">
                            <div className="rounded-full bg-green-100 p-3">
                                <CheckCircle2 className="h-12 w-12 text-green-600" />
                            </div>
                        </div>
                        <DialogTitle className="text-center text-2xl">تمت عملية البيع بنجاح</DialogTitle>
                    </DialogHeader>

                    <div className="space-y-4 py-4">
                        {/* Sale Info */}
                        <div className="text-center space-y-2">
                            <div className="flex items-center justify-center gap-2 text-muted-foreground">
                                <Receipt className="h-4 w-4" />
                                <span>رقم البيع: {completedSale.saleNumber}</span>
                            </div>
                            <p className="text-sm text-muted-foreground">
                                {new Date(completedSale.completedAt).toLocaleString('ar-SA')}
                            </p>
                        </div>

                        <Separator />

                        {/* Summary */}
                        <div className="space-y-2">
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">عدد الأصناف:</span>
                                <span className="font-medium">{completedSale.totalItems}</span>
                            </div>
                            {completedSale.totalDiscount.amount > 0 && (
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">الخصم:</span>
                                    <span className="font-medium text-green-600">
                                        -{formatPrice(completedSale.totalDiscount)}
                                    </span>
                                </div>
                            )}
                            <div className="flex justify-between text-lg font-bold">
                                <span>الإجمالي:</span>
                                <span className="text-primary">{formatPrice(completedSale.grandTotal)}</span>
                            </div>
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">المدفوع:</span>
                                <span className="font-medium">{formatPrice(completedSale.amountPaid)}</span>
                            </div>
                            {change > 0 && (
                                <div className="flex justify-between text-lg font-semibold">
                                    <span>الباقي:</span>
                                    <span className="text-green-600">
                                        {formatPrice({ amount: change, currency: total.currency })}
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>

                    <DialogFooter>
                        <Button onClick={handleFinish} className="w-full" size="lg">
                            إنهاء
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        );
    }

    // Checkout view
    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle className="text-2xl">إتمام عملية البيع</DialogTitle>
                </DialogHeader>

                <div className="space-y-6 py-4">
                    {/* Sale Summary */}
                    <div className="space-y-3 p-4 bg-muted/30 rounded-lg">
                        <div className="flex justify-between text-sm">
                            <span className="text-muted-foreground">عدد الأصناف:</span>
                            <span className="font-medium">{items.length}</span>
                        </div>
                        {totalDiscount.amount > 0 && (
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">الخصم:</span>
                                <span className="font-medium text-green-600">
                                    -{formatPrice(totalDiscount)}
                                </span>
                            </div>
                        )}
                        <Separator />
                        <div className="flex justify-between text-xl font-bold">
                            <span>الإجمالي:</span>
                            <span className="text-primary">{formatPrice(total)}</span>
                        </div>
                    </div>

                    {/* Amount Paid Input */}
                    <div className="space-y-2">
                        <label className="text-sm font-medium">المبلغ المدفوع:</label>
                        <Input
                            type="number"
                            placeholder="أدخل المبلغ المدفوع"
                            value={amountPaid}
                            onChange={(e) => setAmountPaid(e.target.value)}
                            className="text-2xl font-bold text-center"
                            min={0}
                            step={0.01}
                            autoFocus
                        />
                        {amountPaidNum > 0 && amountPaidNum < total.amount && (
                            <p className="text-xs text-destructive">
                                المبلغ المدفوع أقل من الإجمالي
                            </p>
                        )}
                    </div>

                    {/* Quick Amount Buttons */}
                    <div className="grid grid-cols-3 gap-2">
                        {[1000, 5000, 10000, 20000, 50000, 100000].map((amount) => (
                            <Button
                                key={amount}
                                variant="outline"
                                size="sm"
                                onClick={() => setAmountPaid(amount.toString())}
                            >
                                {amount.toLocaleString()}
                            </Button>
                        ))}
                    </div>

                    {/* Exact Amount Button */}
                    <Button
                        variant="secondary"
                        className="w-full"
                        onClick={() => setAmountPaid(total.amount.toString())}
                    >
                        المبلغ بالضبط ({formatPrice(total)})
                    </Button>

                    {/* Change Display */}
                    {amountPaidNum >= total.amount && change > 0 && (
                        <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
                            <div className="flex justify-between items-center">
                                <span className="text-sm font-medium text-green-900">الباقي:</span>
                                <span className="text-2xl font-bold text-green-600">
                                    {formatPrice({ amount: change, currency: total.currency })}
                                </span>
                            </div>
                        </div>
                    )}
                </div>

                <DialogFooter className="gap-2">
                    <Button variant="outline" onClick={() => handleOpenChange(false)} disabled={isProcessing}>
                        <X className="h-4 w-4 ml-2" />
                        إلغاء
                    </Button>
                    <Button
                        onClick={handleCompleteSale}
                        disabled={amountPaidNum < total.amount || isProcessing}
                        className="flex-1"
                        size="lg"
                    >
                        {isProcessing ? (
                            <>
                                <Loader2 className="h-4 w-4 ml-2 animate-spin" />
                                جاري المعالجة...
                            </>
                        ) : (
                            <>
                                <CheckCircle2 className="h-4 w-4 ml-2" />
                                إتمام البيع
                            </>
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
