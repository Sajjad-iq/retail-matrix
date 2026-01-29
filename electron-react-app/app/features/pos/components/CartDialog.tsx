import { useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
} from '@/app/components/ui/dialog';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/app/components/ui/alert-dialog';
import { Button } from '@/app/components/ui/button';
import { Separator } from '@/app/components/ui/separator';
import { Minus, Plus, Trash2, ShoppingBag, X } from 'lucide-react';
import { useCartStore } from '../stores/cartStore';
import { formatPrice } from '@/lib/utils';
import { toast } from 'sonner';

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
    const getTotalItems = useCartStore(state => state.getTotalItems);
    const getTotal = useCartStore(state => state.getTotal);
    const getTotalDiscount = useCartStore(state => state.getTotalDiscount);
    const getCurrency = useCartStore(state => state.getCurrency);
    
    const [showClearConfirm, setShowClearConfirm] = useState(false);

    const totalItems = getTotalItems();
    const subtotal = getTotal();
    const totalDiscount = getTotalDiscount();
    const currency = getCurrency();

    const handleUpdateQuantity = (productPackagingId: string, delta: number) => {
        const item = items.find(i => i.productPackagingId === productPackagingId);
        if (!item) return;

        const newQuantity = item.quantity + delta;
        
        if (newQuantity > item.availableStock) {
            toast.error(`الكمية المتوفرة: ${item.availableStock}`);
            return;
        }

        if (newQuantity <= 0) {
            removeItem(productPackagingId);
        } else {
            updateQuantity(productPackagingId, newQuantity);
        }
    };

    const handleClearCart = () => {
        clearCart();
        setShowClearConfirm(false);
        toast.success('تم مسح السلة');
        onClose();
    };

    const handleCheckout = () => {
        if (items.length === 0) {
            toast.error('السلة فارغة');
            return;
        }
        onClose();
        onCheckout();
    };

    const calculateLineTotal = (item: typeof items[0]) => {
        return item.unitPrice.amount * item.quantity;
    };

    return (
        <>
            <Dialog open={open} onOpenChange={onClose}>
                <DialogContent className="max-w-2xl max-h-[90vh] overflow-hidden flex flex-col">
                    <DialogHeader>
                        <div className="flex items-center justify-between">
                            <DialogTitle className="text-2xl">
                                السلة ({totalItems})
                            </DialogTitle>
                            {items.length > 0 && (
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => setShowClearConfirm(true)}
                                    className="text-destructive hover:text-destructive"
                                >
                                    <Trash2 className="ml-2 h-4 w-4" />
                                    مسح الكل
                                </Button>
                            )}
                        </div>
                    </DialogHeader>

                    {/* Cart Items */}
                    <div className="flex-1 overflow-y-auto space-y-4 py-4">
                        {items.length === 0 ? (
                            <div className="flex flex-col items-center justify-center h-64 text-muted-foreground">
                                <ShoppingBag className="h-16 w-16 mb-4 opacity-50" />
                                <p className="text-lg">السلة فارغة</p>
                                <p className="text-sm">أضف منتجات للمتابعة</p>
                            </div>
                        ) : (
                            items.map((item) => (
                                <div
                                    key={item.productPackagingId}
                                    className="flex gap-4 p-4 bg-muted rounded-lg"
                                >
                                    {/* Product Image */}
                                    {item.imageUrl && (
                                        <img
                                            src={item.imageUrl}
                                            alt={item.productName}
                                            className="w-20 h-20 object-cover rounded-md"
                                        />
                                    )}

                                    {/* Product Info */}
                                    <div className="flex-1">
                                        <div className="flex items-start justify-between mb-2">
                                            <div>
                                                <h3 className="font-semibold">
                                                    {item.productName}
                                                </h3>
                                                <p className="text-sm text-muted-foreground">
                                                    {item.packagingName}
                                                </p>
                                            </div>
                                            <Button
                                                variant="ghost"
                                                size="icon"
                                                onClick={() => removeItem(item.productPackagingId)}
                                                className="text-destructive hover:text-destructive"
                                            >
                                                <X className="h-4 w-4" />
                                            </Button>
                                        </div>

                                        <div className="flex items-center justify-between">
                                            <div className="text-sm">
                                                <div>
                                                    سعر الوحدة: {formatPrice(item.unitPrice)}
                                                </div>
                                                {item.discount && (
                                                    <div className="text-green-600">
                                                        خصم {item.discount.value}%
                                                    </div>
                                                )}
                                                <div className="text-muted-foreground">
                                                    متوفر: {item.availableStock}
                                                </div>
                                            </div>

                                            {/* Quantity Controls */}
                                            <div className="flex items-center gap-2">
                                                <Button
                                                    size="icon"
                                                    variant="outline"
                                                    onClick={() => handleUpdateQuantity(item.productPackagingId, -1)}
                                                >
                                                    <Minus className="h-4 w-4" />
                                                </Button>
                                                <span className="w-12 text-center font-semibold">
                                                    {item.quantity}
                                                </span>
                                                <Button
                                                    size="icon"
                                                    variant="outline"
                                                    onClick={() => handleUpdateQuantity(item.productPackagingId, 1)}
                                                    disabled={item.quantity >= item.availableStock}
                                                >
                                                    <Plus className="h-4 w-4" />
                                                </Button>
                                            </div>
                                        </div>

                                        {/* Line Total */}
                                        <div className="mt-2 text-right">
                                            <span className="text-lg font-bold text-primary">
                                                {formatPrice({
                                                    amount: calculateLineTotal(item),
                                                    currency: item.unitPrice.currency,
                                                })}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {/* Cart Summary */}
                    {items.length > 0 && (
                        <>
                            <Separator />
                            <div className="space-y-2">
                                {totalDiscount > 0 && (
                                    <div className="flex justify-between text-sm">
                                        <span className="text-muted-foreground">الخصم:</span>
                                        <span className="text-green-600 font-medium">
                                            -{formatPrice({ amount: totalDiscount, currency })}
                                        </span>
                                    </div>
                                )}
                                <div className="flex justify-between items-center text-lg">
                                    <span className="font-semibold">الإجمالي:</span>
                                    <span className="text-2xl font-bold text-primary">
                                        {formatPrice({ amount: subtotal, currency })}
                                    </span>
                                </div>
                            </div>
                        </>
                    )}

                    {/* Actions */}
                    <DialogFooter className="gap-2">
                        <Button variant="outline" onClick={onClose}>
                            إغلاق
                        </Button>
                        <Button
                            onClick={handleCheckout}
                            disabled={items.length === 0}
                            size="lg"
                            className="flex-1"
                        >
                            <ShoppingBag className="ml-2 h-5 w-5" />
                            إتمام الشراء ({totalItems})
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>

            {/* Clear Cart Confirmation */}
            <AlertDialog open={showClearConfirm} onOpenChange={setShowClearConfirm}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>مسح السلة</AlertDialogTitle>
                        <AlertDialogDescription>
                            هل أنت متأكد من رغبتك في مسح جميع المنتجات من السلة؟
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>إلغاء</AlertDialogCancel>
                        <AlertDialogAction
                            onClick={handleClearCart}
                            className="bg-destructive hover:bg-destructive/90"
                        >
                            مسح
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
}
