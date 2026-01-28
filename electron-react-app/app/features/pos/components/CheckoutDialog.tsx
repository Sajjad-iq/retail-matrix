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
import { Separator } from '@/app/components/ui/separator';
import { CheckCircle2, Loader2, Receipt, X } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { useCompleteSale, useDraftSale } from '../hooks/usePosActions';
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

    const inventoryId = useCartStore(state => state.inventoryId);
    const { data: draftSale } = useDraftSale(inventoryId);
    const completeSale = useCompleteSale();

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
        if (!inventoryId || !draftSale) return;

        if (amountPaidNum < total) {
            return; // Amount paid is less than total
        }

        setIsProcessing(true);

        try {
            const completed = await completeSale.mutateAsync({
                saleId: draftSale.saleId,
                inventoryId,
                amountPaid: amountPaidNum
            });

            setCompletedSale(completed ?? null);
        } catch {
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
                        disabled={amountPaidNum < total || isProcessing || !draftSale}
                        className="flex-1 gap-2"
                        size="lg"
                    >
                        {isProcessing ? (
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
