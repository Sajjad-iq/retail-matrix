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
import { Minus, Plus, Trash2, ShoppingBag, X } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { formatPrice } from '@/lib/utils';
import { ConfirmDialog } from '@/app/components/ui/confirm-dialog';

interface CartDialogProps {
    open: boolean;
    onClose: () => void;
    onCheckout: () => void;
}

export function CartDialog({ open, onClose, onCheckout }: CartDialogProps) {
    const items = useCartStore(state => state.items);
    const updateQuantity = useCartStore(state => state.updateQuantity);
    const removeItem = useCartStore(state => state.removeItem);
    const clearCart = useCartStore(state => state.clearCart);
    const getSubtotal = useCartStore(state => state.getSubtotal);
    const getTotalDiscount = useCartStore(state => state.getTotalDiscount);
    const getTotal = useCartStore(state => state.getTotal);

    const subtotal = getSubtotal();
    const totalDiscount = getTotalDiscount();
    const total = getTotal();

    const handleCheckout = () => {
        onCheckout();
        onClose();
    };

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
                                onConfirm={clearCart}
                                variant="destructive"
                            >
                                <Button variant="ghost" size="sm" className="gap-2">
                                    <Trash2 className="h-4 w-4" />
                                    مسح الكل
                                </Button>
                            </ConfirmDialog>
                        )}
                    </div>
                </DialogHeader>

                {items.length === 0 ? (
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
                                    key={item.id}
                                    className="flex gap-4 p-4 border rounded-lg hover:bg-muted/50 transition-colors"
                                >
                                    {/* Product Image */}
                                    <div className="w-20 h-20 bg-muted rounded-lg overflow-hidden shrink-0">
                                        {item.imageUrl ? (
                                            <img
                                                src={item.imageUrl}
                                                alt={item.productName}
                                                className="w-full h-full object-cover"
                                            />
                                        ) : (
                                            <div className="flex items-center justify-center h-full">
                                                <ShoppingBag className="h-8 w-8 text-muted-foreground" />
                                            </div>
                                        )}
                                    </div>

                                    {/* Product Details */}
                                    <div className="flex-1 space-y-2">
                                        <div className="flex items-start justify-between gap-2">
                                            <div>
                                                <h4 className="font-semibold">{item.productName}</h4>
                                                <p className="text-sm text-muted-foreground">
                                                    {item.packagingName}
                                                </p>
                                                {item.barcode && (
                                                    <p className="text-xs text-muted-foreground">
                                                        {item.barcode}
                                                    </p>
                                                )}
                                            </div>
                                            <ConfirmDialog
                                                title="حذف العنصر"
                                                description={`هل أنت متأكد من حذف "${item.productName}" من السلة؟`}
                                                confirmText="حذف"
                                                cancelText="إلغاء"
                                                onConfirm={() => removeItem(item.id)}
                                                variant="destructive"
                                            >
                                                <Button
                                                    variant="ghost"
                                                    size="icon"
                                                    className="h-8 w-8 text-muted-foreground hover:text-destructive shrink-0"
                                                >
                                                    <X className="h-4 w-4" />
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
                                                    onClick={() => updateQuantity(item.id, item.quantity - 1)}
                                                    disabled={item.quantity <= 1}
                                                >
                                                    <Minus className="h-3 w-3" />
                                                </Button>
                                                <span className="w-12 text-center font-semibold">
                                                    {item.quantity}
                                                </span>
                                                <Button
                                                    variant="outline"
                                                    size="icon"
                                                    className="h-8 w-8"
                                                    onClick={() => updateQuantity(item.id, item.quantity + 1)}
                                                    disabled={item.quantity >= item.availableStock}
                                                >
                                                    <Plus className="h-3 w-3" />
                                                </Button>
                                                <span className="text-xs text-muted-foreground mr-2">
                                                    (متوفر: {item.availableStock})
                                                </span>
                                            </div>

                                            {/* Line Total */}
                                            <div className="text-left">
                                                <div className="font-bold text-primary">
                                                    {formatPrice(item.lineTotal)}
                                                </div>
                                                {item.discount && item.discount.value > 0 && (
                                                    <div className="text-xs text-muted-foreground">
                                                        <span className="line-through">
                                                            {formatPrice({
                                                                amount: item.unitPrice.amount * item.quantity,
                                                                currency: item.unitPrice.currency
                                                            })}
                                                        </span>
                                                        <Badge variant="destructive" className="mr-1 text-xs">
                                                            خصم
                                                        </Badge>
                                                    </div>
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
                            <div className="flex justify-between text-sm">
                                <span className="text-muted-foreground">المجموع الفرعي:</span>
                                <span className="font-medium">{formatPrice(subtotal)}</span>
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
                            <div className="flex justify-between text-lg font-bold">
                                <span>الإجمالي:</span>
                                <span className="text-primary">{formatPrice(total)}</span>
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
