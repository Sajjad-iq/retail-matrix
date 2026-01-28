import { useState, useEffect } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Badge } from '@/app/components/ui/badge';
import { Minus, Plus, Package, ShoppingCart } from 'lucide-react';
import { PosProductDto, PosPackagingDto } from '../lib/types';
import { formatPrice } from '@/lib/utils';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'sonner';

interface ProductDialogProps {
    product: PosProductDto | null;
    open: boolean;
    onClose: () => void;
}

export function ProductDialog({ product, open, onClose }: ProductDialogProps) {
    const [selectedPackaging, setSelectedPackaging] = useState<PosPackagingDto | null>(null);
    const [quantity, setQuantity] = useState(1);
    const addItem = useCartStore(state => state.addItem);

    // Reset state when dialog opens/closes
    const handleOpenChange = (isOpen: boolean) => {
        if (!isOpen) {
            setSelectedPackaging(null);
            setQuantity(1);
            onClose();
        }
    };

    // Set default packaging when product changes
    useEffect(() => {
        if (product && open && product.packagings.length > 0) {
            const defaultPkg = product.packagings.find(p => p.isDefault) || product.packagings[0];
            setSelectedPackaging(defaultPkg);
        }
    }, [product, open]);

    const handleAddToCart = () => {
        if (!product || !selectedPackaging) return;

        if (quantity > selectedPackaging.availableStock) {
            toast.error(`الكمية المتوفرة: ${selectedPackaging.availableStock}`);
            return;
        }

        addItem(
            {
                productId: product.productId,
                productName: product.productName,
                imageUrl: product.imageUrls[0]
            },
            selectedPackaging,
            quantity
        );

        toast.success('تمت الإضافة إلى السلة');
        handleOpenChange(false);
    };

    if (!product) return null;

    return (
        <Dialog open={open} onOpenChange={handleOpenChange}>
            <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-2xl">{product.productName}</DialogTitle>
                    {product.categoryName && (
                        <DialogDescription>{product.categoryName}</DialogDescription>
                    )}
                </DialogHeader>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
                    {/* Product Images */}
                    <div className="space-y-4">
                        <div className="aspect-square bg-muted rounded-lg overflow-hidden">
                            {product.imageUrls.length > 0 ? (
                                <img
                                    src={product.imageUrls[0]}
                                    alt={product.productName}
                                    className="w-full h-full object-cover"
                                />
                            ) : (
                                <div className="flex items-center justify-center h-full">
                                    <Package className="h-24 w-24 text-muted-foreground" />
                                </div>
                            )}
                        </div>

                        {/* Thumbnail Gallery */}
                        {product.imageUrls.length > 1 && (
                            <div className="grid grid-cols-4 gap-2">
                                {product.imageUrls.slice(1, 5).map((url, index) => (
                                    <div
                                        key={index}
                                        className="aspect-square bg-muted rounded-lg overflow-hidden"
                                    >
                                        <img
                                            src={url}
                                            alt={`${product.productName} ${index + 2}`}
                                            className="w-full h-full object-cover"
                                        />
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    {/* Product Details */}
                    <div className="space-y-6">
                        {/* Packaging Selection */}
                        <div className="space-y-3">
                            <label className="text-sm font-medium">اختر وحدة البيع:</label>
                            <div className="grid grid-cols-1 gap-2">
                                {product.packagings.map((pkg) => (
                                    <button
                                        key={pkg.packagingId}
                                        onClick={() => {
                                            setSelectedPackaging(pkg);
                                            setQuantity(1);
                                        }}
                                        className={`p-4 rounded-lg border-2 text-right transition-all ${
                                            selectedPackaging?.packagingId === pkg.packagingId
                                                ? 'border-primary bg-primary/5'
                                                : 'border-border hover:border-primary/50'
                                        } ${!pkg.inStock ? 'opacity-50' : ''}`}
                                        disabled={!pkg.inStock}
                                    >
                                        <div className="flex items-start justify-between">
                                            <div className="flex-1">
                                                <div className="flex items-center gap-2 mb-1">
                                                    <span className="font-semibold">{pkg.packagingName}</span>
                                                    {pkg.isDefault && (
                                                        <Badge variant="outline" className="text-xs">
                                                            افتراضي
                                                        </Badge>
                                                    )}
                                                    {!pkg.inStock && (
                                                        <Badge variant="destructive" className="text-xs">
                                                            نفذ
                                                        </Badge>
                                                    )}
                                                </div>
                                                {pkg.description && (
                                                    <p className="text-xs text-muted-foreground mb-2">
                                                        {pkg.description}
                                                    </p>
                                                )}
                                                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                                                    <span>المخزون: {pkg.availableStock}</span>
                                                    {pkg.barcode && (
                                                        <span className="text-xs">• {pkg.barcode}</span>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="text-left">
                                                <div className="text-xl font-bold text-primary">
                                                    {formatPrice(pkg.discountedPrice)}
                                                </div>
                                                {pkg.hasDiscount && (
                                                    <div className="text-sm text-muted-foreground line-through">
                                                        {formatPrice(pkg.sellingPrice)}
                                                    </div>
                                                )}
                                                {pkg.hasDiscount && (
                                                    <Badge variant="destructive" className="mt-1">
                                                        خصم {pkg.discountPercentage}%
                                                    </Badge>
                                                )}
                                            </div>
                                        </div>
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* Quantity Selection */}
                        {selectedPackaging && selectedPackaging.inStock && (
                            <div className="space-y-3">
                                <label className="text-sm font-medium">الكمية:</label>
                                <div className="flex items-center gap-4">
                                    <Button
                                        variant="outline"
                                        size="icon"
                                        onClick={() => setQuantity(Math.max(1, quantity - 1))}
                                        disabled={quantity <= 1}
                                    >
                                        <Minus className="h-4 w-4" />
                                    </Button>
                                    <div className="flex-1">
                                        <input
                                            type="number"
                                            min="1"
                                            max={selectedPackaging.availableStock}
                                            value={quantity}
                                            onChange={(e) => {
                                                const val = parseInt(e.target.value) || 1;
                                                setQuantity(
                                                    Math.min(
                                                        Math.max(1, val),
                                                        selectedPackaging.availableStock
                                                    )
                                                );
                                            }}
                                            className="w-full text-center text-2xl font-bold border rounded-lg p-2"
                                        />
                                    </div>
                                    <Button
                                        variant="outline"
                                        size="icon"
                                        onClick={() =>
                                            setQuantity(
                                                Math.min(quantity + 1, selectedPackaging.availableStock)
                                            )
                                        }
                                        disabled={quantity >= selectedPackaging.availableStock}
                                    >
                                        <Plus className="h-4 w-4" />
                                    </Button>
                                </div>
                                <p className="text-xs text-muted-foreground text-center">
                                    المتوفر: {selectedPackaging.availableStock}
                                </p>
                            </div>
                        )}

                        {/* Total Price */}
                        {selectedPackaging && selectedPackaging.inStock && (
                            <div className="p-4 bg-muted rounded-lg">
                                <div className="flex items-center justify-between">
                                    <span className="text-sm font-medium">الإجمالي:</span>
                                    <span className="text-2xl font-bold text-primary">
                                        {formatPrice({
                                            amount: selectedPackaging.discountedPrice.amount * quantity,
                                            currency: selectedPackaging.discountedPrice.currency
                                        })}
                                    </span>
                                </div>
                            </div>
                        )}

                        {/* Add to Cart Button */}
                        <Button
                            className="w-full gap-2"
                            size="lg"
                            onClick={handleAddToCart}
                            disabled={!selectedPackaging || !selectedPackaging.inStock}
                        >
                            <ShoppingCart className="h-5 w-5" />
                            إضافة إلى السلة
                        </Button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    );
}
