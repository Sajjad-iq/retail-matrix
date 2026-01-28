import { useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Badge } from '@/app/components/ui/badge';
import { Separator } from '@/app/components/ui/separator';
import { Minus, Plus, Trash2, ShoppingBag, X, Loader2 } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { useDraftSale, useUpdateSale, useCancelSale } from '../hooks/usePosActions';
import { formatPrice } from '@/lib/utils';
import { ConfirmDialog } from '@/app/components/ui/confirm-dialog';
import { toast } from 'sonner';

interface CartDialogProps {
    open: boolean;
    onClose: () => void;
    onCheckout: () => void;
}

export function CartDialog({ open, onClose, onCheckout }: CartDialogProps) {
    const inventoryId = useCartStore(state => state.inventoryId);
    const { data: draftSale, isLoading } = useDraftSale(inventoryId);
    const updateSale = useUpdateSale();
    const cancelSale = useCancelSale();
    const [updatingItemId, setUpdatingItemId] = useState<string | null>(null);

    const handleUpdateQuantity = async (itemId: string, newQuantity: number) => {
        if (!draftSale || !inventoryId) return;

        setUpdatingItemId(itemId);

        try {
            const updatedItems = draftSale.items
                .map(item => {
                    if (item.itemId === itemId) {
                        return newQuantity > 0
                            ? {
                                  productPackagingId: item.productPackagingId,
                                  quantity: newQuantity,
                                  discount: item.discount
                                      ? {
                                            amount: item.discount.value,
                                            isPercentage: item.discount.type === 1,
                                        }
                                      : undefined,
                              }
                            : null;
                    }
                    return {
                        productPackagingId: item.productPackagingId,
                        quantity: item.quantity,
                        discount: item.discount
                            ? {
                                  amount: item.discount.value,
                                  isPercentage: item.discount.type === 1,
                              }
                            : undefined,
                    };
                })
                .filter(Boolean);

            await updateSale.mutateAsync({
                saleId: draftSale.saleId,
                data: {
                    inventoryId,
                    items: updatedItems as any,
                    notes: draftSale.notes,
                },
            });
        } catch {
            // Error handled by interceptor
        } finally {
            setUpdatingItemId(null);
        }
    };

    const handleRemoveItem = async (itemId: string) => {
        if (!draftSale || !inventoryId) return;

        setUpdatingItemId(itemId);

        try {
            const updatedItems = draftSale.items
                .filter(item => item.itemId !== itemId)
                .map(item => ({
                    productPackagingId: item.productPackagingId,
                    quantity: item.quantity,
                    discount: item.discount
                        ? {
                              amount: item.discount.value,
                              isPercentage: item.discount.type === 1,
                          }
                        : undefined,
                }));

            await updateSale.mutateAsync({
                saleId: draftSale.saleId,
                data: {
                    inventoryId,
                    items: updatedItems,
                    notes: draftSale.notes,
                },
            });
        } catch {
            // Error handled by interceptor
        } finally {
            setUpdatingItemId(null);
        }
    };

    const handleClearCart = async () => {
        if (!draftSale) return;

        try {
            await cancelSale.mutateAsync(draftSale.saleId);
            toast.success('تم مسح السلة');
        } catch {
            // Error handled by interceptor
        }
    };

    const handleCheckout = () => {
        onCheckout();
        onClose();
    };

    const items = draftSale?.items || [];
    const subtotal = draftSale?.grandTotal || { amount: 0, currency: 'IQD' };
    const totalDiscount = draftSale?.totalDiscount || { amount: 0, currency: 'IQD' };

    return (
        <Dialog open={open} onOpenChange={onClose}>
            <DialogContent className="max-w-3xl max-h-[90vh] flex flex-col">
                <DialogHeader>
                    <div className="flex items-center justify-between">
                        <DialogTitle className="text-2xl">السلة</DialogTitle>
                        {items.length > 0 && (
                            <ConfirmDialog
                                title="مسح السلة"
                                description="هل أنت متأكد من مسح جميع العناصر من السلة؟"
                                confirmText="مسح"
                                cancelText="إلغاء"
                                onConfirm={handleClearCart}
                                variant="destructive"
                            >
                                <Button 
                                    variant="ghost" 
                                    size="sm" 
                                    className="gap-2"
                                    disabled={cancelSale.isPending}
                                >
                                    {cancelSale.isPending ? (
                                        <Loader2 className="h-4 w-4 animate-spin" />
                                    ) : (
                                        <Trash2 className="h-4 w-4" />
                                    )}
                                    مسح الكل
                                </Button>
                            </ConfirmDialog>
                        )}
                    </div>
                </DialogHeader>

                {isLoading ? (
                    <div className="flex flex-col items-center justify-center py-12">
                        <Loader2 className="h-12 w-12 animate-spin text-muted-foreground mb-4" />
                        <p className="text-muted-foreground">جاري التحميل...</p>
                    </div>
                ) : items.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-12 text-center">
                        <ShoppingBag className="h-16 w-16 text-muted-foreground mb-4" />
                        <h3 className="text-lg font-semibold mb-2">السلة فارغة</h3>
                        <p className="text-muted-foreground text-sm">
                            ابدأ بإضافة منتجات إلى السلة
                        </p>
                    </div>
                ) : (
                    <>
                        {/* Cart Items */}
                        <div className="flex-1 overflow-y-auto space-y-4 py-4">
                            {items.map((item) => (
                                <div
                                    key={item.itemId}
                                    className="flex gap-4 p-4 border rounded-lg hover:bg-muted/50 transition-colors"
                                >
                                    {/* Product Details */}
                                    <div className="flex-1 space-y-2">
                                        <div className="flex items-start justify-between gap-2">
                                            <div>
                                                <h4 className="font-semibold">{item.productName}</h4>
                                                <p className="text-sm text-muted-foreground">
                                                    سعر الوحدة: {formatPrice(item.unitPrice)}
                                                </p>
                                            </div>
                                            <ConfirmDialog
                                                title="حذف العنصر"
                                                description={`هل أنت متأكد من حذف "${item.productName}" من السلة؟`}
                                                confirmText="حذف"
                                                cancelText="إلغاء"
                                                onConfirm={() => handleRemoveItem(item.itemId)}
                                                variant="destructive"
                                            >
                                                <Button
                                                    variant="ghost"
                                                    size="icon"
                                                    className="h-8 w-8 text-muted-foreground hover:text-destructive shrink-0"
                                                    disabled={updatingItemId === item.itemId}
                                                >
                                                    {updatingItemId === item.itemId ? (
                                                        <Loader2 className="h-4 w-4 animate-spin" />
                                                    ) : (
                                                        <X className="h-4 w-4" />
                                                    )}
                                                </Button>
                                            </ConfirmDialog>
                                        </div>

                                        {/* Quantity Controls */}
                                        <div className="flex items-center justify-between">
                                            <div className="flex items-center gap-2">
                                                <Button
                                                    variant="outline"
                                                    size="icon"
                                                    className="h-8 w-8"
                                                    onClick={() => handleUpdateQuantity(item.itemId, item.quantity - 1)}
                                                    disabled={item.quantity <= 1 || updatingItemId === item.itemId}
                                                >
                                                    <Minus className="h-3 w-3" />
                                                </Button>
                                                <span className="w-12 text-center font-semibold">
                                                    {updatingItemId === item.itemId ? (
                                                        <Loader2 className="h-4 w-4 animate-spin inline" />
                                                    ) : (
                                                        item.quantity
                                                    )}
                                                </span>
                                                <Button
                                                    variant="outline"
                                                    size="icon"
                                                    className="h-8 w-8"
                                                    onClick={() => handleUpdateQuantity(item.itemId, item.quantity + 1)}
                                                    disabled={updatingItemId === item.itemId}
                                                >
                                                    <Plus className="h-3 w-3" />
                                                </Button>
                                            </div>

                                            {/* Line Total */}
                                            <div className="text-left">
                                                <div className="font-bold text-primary">
                                                    {formatPrice(item.lineTotal)}
                                                </div>
                                                {item.discount && item.discount.value > 0 && (
                                                    <Badge variant="destructive" className="text-xs">
                                                        خصم {item.discount.value}
                                                        {item.discount.type === 1 ? '%' : ''}
                                                    </Badge>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>

                        <Separator />

                        {/* Summary */}
                        <div className="space-y-3 py-4">
                            {totalDiscount.amount > 0 && (
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground">الخصم:</span>
                                    <span className="font-medium text-green-600">
                                        -{formatPrice(totalDiscount)}
                                    </span>
                                </div>
                            )}
                            <Separator />
                            <div className="flex justify-between text-lg font-bold">
                                <span>الإجمالي:</span>
                                <span className="text-primary">{formatPrice(subtotal)}</span>
                            </div>
                        </div>

                        <DialogFooter className="gap-2">
                            <Button variant="outline" onClick={onClose}>
                                إغلاق
                            </Button>
                            <Button onClick={handleCheckout} size="lg" className="gap-2">
                                <ShoppingBag className="h-5 w-5" />
                                إتمام الشراء ({items.length})
                            </Button>
                        </DialogFooter>
                    </>
                )}
            </DialogContent>
        </Dialog>
    );
}
