import { useState, useEffect } from 'react';
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
import { PosCartItemDto, SaleDto } from '../lib/types';

interface CartDialogProps {
    open: boolean;
    onClose: () => void;
    onCheckout: (draftSale: SaleDto) => void;
}

export function CartDialog({ open, onClose, onCheckout }: CartDialogProps) {
    const inventoryId = useCartStore(state => state.inventoryId);
    const { data: draftSale, isLoading, refetch } = useDraftSale(inventoryId);
    const updateSale = useUpdateSale();
    const cancelSale = useCancelSale();
    
    // Local state for optimistic UI updates
    const [localItems, setLocalItems] = useState<PosCartItemDto[]>([]);
    const [hasChanges, setHasChanges] = useState(false);
    const [isSyncing, setIsSyncing] = useState(false);

    // Sync local state with backend data when dialog opens
    useEffect(() => {
        if (open && draftSale?.items) {
            setLocalItems(draftSale.items);
            setHasChanges(false);
        }
    }, [open, draftSale?.items]);

    // Sync changes to backend
    const syncToBackend = async (): Promise<boolean> => {
        if (!draftSale || !inventoryId || isSyncing || !hasChanges) {
            return true;
        }

        setIsSyncing(true);

        try {
            const saleItems = localItems.map(item => ({
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
                    items: saleItems,
                    notes: draftSale.notes,
                },
            });

            setHasChanges(false);
            return true;
        } catch {
            // On error, revert to backend state
            if (draftSale?.items) {
                setLocalItems(draftSale.items);
                setHasChanges(false);
            }
            return false;
        } finally {
            setIsSyncing(false);
        }
    };

    const handleUpdateQuantity = (itemId: string, newQuantity: number) => {
        if (!draftSale) return;

        // Update local state immediately for instant UI feedback
        const updatedItems = localItems
            .map(item => {
                if (item.itemId === itemId) {
                    return newQuantity > 0
                        ? { ...item, quantity: newQuantity }
                        : null;
                }
                return item;
            })
            .filter((item): item is PosCartItemDto => item !== null);

        setLocalItems(updatedItems);
        setHasChanges(true);
    };

    const handleRemoveItem = (itemId: string) => {
        if (!draftSale) return;

        // Update local state immediately
        const updatedItems = localItems.filter(item => item.itemId !== itemId);
        
        setLocalItems(updatedItems);
        setHasChanges(true);
    };

    const handleClearCart = async () => {
        // Prevent duplicate submissions
        if (cancelSale.isPending || isSyncing) return;
        
        if (!draftSale) return;

        try {
            await cancelSale.mutateAsync(draftSale.saleId);
            setLocalItems([]);
            setHasChanges(false);
            toast.success('تم مسح السلة');
        } catch {
            // Error handled by interceptor
        }
    };

    const handleCheckout = async () => {
        // Prevent multiple simultaneous checkout attempts
        if (isSyncing || updateSale.isPending) {
            toast.error('جاري الحفظ، يرجى الانتظار');
            return;
        }

        if (!draftSale) {
            toast.error('لا يوجد بيع نشط');
            return;
        }

        try {
            console.log('🛒 Cart Checkout - Start', { 
                hasChanges, 
                draftSaleId: draftSale.saleId,
                itemsCount: localItems.length 
            });

            // Sync changes to backend before checkout
            if (hasChanges) {
                console.log('🛒 Cart - Syncing changes to backend...');
                const success = await syncToBackend();
                if (!success) {
                    toast.error('فشل حفظ التغييرات');
                    return;
                }
                console.log('🛒 Cart - Sync completed successfully');
            }

            // Wait for query invalidation to complete
            await new Promise(resolve => setTimeout(resolve, 300));

            // DON'T refetch - use the SAME draft sale we already have!
            // Refetching might return a different draft sale due to backend bug
            console.log('🛒 Cart - Using existing draft sale:', draftSale.saleId);
            
            // Create a fresh copy with updated items from local state
            const checkoutData: SaleDto = {
                ...draftSale,
                items: localItems,
                totalItems: localItems.length
            };
            
            console.log('🛒 Cart - Passing data to checkout:', checkoutData.saleId);
            onCheckout(checkoutData);
            onClose();
        } catch (error) {
            console.error('🛒 Cart Checkout error:', error);
            toast.error('حدث خطأ أثناء إتمام العملية');
        }
    };

    const handleClose = () => {
        // Discard local changes and close
        if (draftSale?.items) {
            setLocalItems(draftSale.items);
        }
        setHasChanges(false);
        onClose();
    };

    // Calculate line total for an item
    const calculateLineTotal = (item: PosCartItemDto) => {
        const baseAmount = item.unitPrice.amount * item.quantity;
        let itemDiscount = 0;

        if (item.discount && item.discount.value > 0) {
            if (item.discount.type === 1) {
                // Percentage discount
                itemDiscount = (baseAmount * item.discount.value) / 100;
            } else {
                // Fixed discount
                itemDiscount = item.discount.value * item.quantity;
            }
        }

        return { amount: baseAmount - itemDiscount, currency: item.unitPrice.currency };
    };

    // Calculate totals from local items with updated quantities
    const calculateTotals = () => {
        let subtotal = 0;
        let totalDiscount = 0;

        localItems.forEach(item => {
            const baseAmount = item.unitPrice.amount * item.quantity;
            let itemDiscount = 0;

            if (item.discount && item.discount.value > 0) {
                if (item.discount.type === 1) {
                    // Percentage discount
                    itemDiscount = (baseAmount * item.discount.value) / 100;
                } else {
                    // Fixed discount
                    itemDiscount = item.discount.value * item.quantity;
                }
            }

            totalDiscount += itemDiscount;
            subtotal += baseAmount - itemDiscount;
        });
        
        return {
            subtotal: { amount: subtotal, currency: 'IQD' },
            totalDiscount: { amount: totalDiscount, currency: 'IQD' },
        };
    };

    const { subtotal, totalDiscount } = calculateTotals();
    const items = localItems;

    return (
        <Dialog open={open} onOpenChange={(isOpen) => !isOpen && handleClose()}>
            <DialogContent className="max-w-3xl max-h-[90vh] flex flex-col">
                <DialogHeader>
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                            <DialogTitle className="text-2xl">السلة</DialogTitle>
                            {hasChanges && !isSyncing && (
                                <Badge variant="secondary" className="text-xs">
                                    تغييرات غير محفوظة
                                </Badge>
                            )}
                            {isSyncing && (
                                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                                    <Loader2 className="h-4 w-4 animate-spin" />
                                    <span>جاري الحفظ...</span>
                                </div>
                            )}
                        </div>
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
                                    disabled={cancelSale.isPending || isSyncing}
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
                                                    disabled={isSyncing || updateSale.isPending}
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
                                                    onClick={() => handleUpdateQuantity(item.itemId, item.quantity - 1)}
                                                    disabled={item.quantity <= 1 || isSyncing || updateSale.isPending}
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
                                                    onClick={() => handleUpdateQuantity(item.itemId, item.quantity + 1)}
                                                    disabled={item.quantity >= item.availableStock || isSyncing || updateSale.isPending}
                                                >
                                                    <Plus className="h-3 w-3" />
                                                </Button>
                                                <span className={`text-xs ${item.availableStock < item.quantity ? 'text-destructive font-semibold' : 'text-muted-foreground'}`}>
                                                    متوفر: {item.availableStock}
                                                </span>
                                            </div>

                                            {/* Line Total */}
                                            <div className="text-left">
                                                <div className="font-bold text-primary">
                                                    {formatPrice(calculateLineTotal(item))}
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

                        <DialogFooter className="gap-2 flex-col sm:flex-row">
                            {hasChanges && (
                                <p className="text-xs text-muted-foreground w-full text-center sm:text-right">
                                    * سيتم حفظ التغييرات عند إتمام الشراء
                                </p>
                            )}
                            <Button variant="outline" onClick={handleClose} disabled={isSyncing || updateSale.isPending}>
                                {hasChanges ? 'إلغاء' : 'إغلاق'}
                            </Button>
                            <Button onClick={handleCheckout} size="lg" className="gap-2" disabled={isSyncing || updateSale.isPending}>
                                {(isSyncing || updateSale.isPending) ? (
                                    <>
                                        <Loader2 className="h-5 w-5 animate-spin" />
                                        جاري الحفظ...
                                    </>
                                ) : (
                                    <>
                                        <ShoppingBag className="h-5 w-5" />
                                        إتمام الشراء ({items.length})
                                    </>
                                )}
                            </Button>
                        </DialogFooter>
                    </>
                )}
            </DialogContent>
        </Dialog>
    );
}
