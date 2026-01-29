import { useState, useEffect, useRef } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogTrigger,
    DialogClose,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { Badge } from '@/app/components/ui/badge';
import { Minus, Plus, ShoppingCart } from 'lucide-react';
import { PosProductDto, PosPackagingDto } from '../lib/types';
import { formatPrice } from '@/lib/utils';
import { useCartStore } from '../stores/cartStore';
import { toast } from 'sonner';

interface ProductDialogProps {
    product: PosProductDto;
    children: React.ReactNode;
}

export function ProductDialog({ product, children }: ProductDialogProps) {
    const [selectedPackaging, setSelectedPackaging] = useState<PosPackagingDto | null>(null);
    const [quantity, setQuantity] = useState(1);
    const closeButtonRef = useRef<HTMLButtonElement>(null);
    
    const addItem = useCartStore(state => state.addItem);

    // Set default packaging when product changes
    useEffect(() => {
        if (product.packagings.length > 0) {
            const defaultPkg = product.packagings.find(p => p.isDefault) || product.packagings[0];
            setSelectedPackaging(defaultPkg);
            setQuantity(1);
        }
    }, [product]);

    const handleAddToCart = () => {
        if (!product || !selectedPackaging) return;

        if (quantity > selectedPackaging.availableStock) {
            toast.error(`الكمية المتوفرة: ${selectedPackaging.availableStock}`);
            return;
        }

        // Add to local cart
        addItem({
            productPackagingId: selectedPackaging.packagingId,
            productName: product.productName,
            packagingName: selectedPackaging.packagingName,
            quantity,
            unitPrice: selectedPackaging.discountedPrice,
            discount: selectedPackaging.discount,
            availableStock: selectedPackaging.availableStock,
            imageUrl: selectedPackaging.imageUrls[0] || product.imageUrls[0],
        });

        toast.success('تمت الإضافة إلى السلة');
        closeButtonRef.current?.click();
    };

    const handleQuantityChange = (delta: number) => {
        if (!selectedPackaging) return;
        const newQuantity = Math.max(1, Math.min(quantity + delta, selectedPackaging.availableStock));
        setQuantity(newQuantity);
    };

    return (
        <Dialog>
            <DialogTrigger asChild>
                {children}
            </DialogTrigger>
            <DialogClose ref={closeButtonRef} className="hidden" />
            <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-2xl">{product.productName}</DialogTitle>
                    {product.categoryName && (
                        <DialogDescription>
                            {product.categoryName}
                        </DialogDescription>
                    )}
                </DialogHeader>

                <div className="space-y-6">
                    {/* Product Images */}
                    {product.imageUrls.length > 0 && (
                        <div className="grid grid-cols-3 gap-2">
                            {product.imageUrls.slice(0, 3).map((url, index) => (
                                <img
                                    key={index}
                                    src={url}
                                    alt={`${product.productName} ${index + 1}`}
                                    className="w-full h-32 object-cover rounded-lg border"
                                />
                            ))}
                        </div>
                    )}

                    {/* Packaging Selection */}
                    <div>
                        <label className="text-sm font-medium mb-2 block">
                            التغليف:
                        </label>
                        <div className="grid grid-cols-2 gap-2">
                            {product.packagings.map((pkg) => (
                                <button
                                    key={pkg.packagingId}
                                    onClick={() => setSelectedPackaging(pkg)}
                                    className={`p-3 border rounded-lg text-right transition-colors ${
                                        selectedPackaging?.packagingId === pkg.packagingId
                                            ? 'border-primary bg-primary/10'
                                            : 'border-gray-300 hover:border-primary/50'
                                    }`}
                                    disabled={!pkg.inStock}
                                >
                                    <div className="flex items-start justify-between">
                                        <div className="flex-1">
                                            <div className="font-medium">{pkg.packagingName}</div>
                                            <div className="text-sm text-muted-foreground">
                                                {formatPrice(pkg.discountedPrice)}
                                            </div>
                                            {pkg.hasDiscount && (
                                                <div className="text-xs text-red-600">
                                                    خصم {pkg.discountPercentage}%
                                                </div>
                                            )}
                                        </div>
                                        {!pkg.inStock && (
                                            <Badge variant="destructive" className="text-xs">
                                                نفذت الكمية
                                            </Badge>
                                        )}
                                    </div>
                                </button>
                            ))}
                        </div>
                    </div>

                    {/* Selected Packaging Details */}
                    {selectedPackaging && (
                        <div className="space-y-4 p-4 bg-muted rounded-lg">
                            <div className="flex items-center justify-between">
                                <div>
                                    <div className="text-lg font-semibold">
                                        {formatPrice(selectedPackaging.discountedPrice)}
                                    </div>
                                    {selectedPackaging.hasDiscount && (
                                        <div className="text-sm text-muted-foreground line-through">
                                            {formatPrice(selectedPackaging.sellingPrice)}
                                        </div>
                                    )}
                                </div>
                                <Badge variant={selectedPackaging.inStock ? 'default' : 'destructive'}>
                                    متوفر: {selectedPackaging.availableStock}
                                </Badge>
                            </div>

                            {/* Packaging Info */}
                            <div className="grid grid-cols-2 gap-2 text-sm">
                                {selectedPackaging.barcode && (
                                    <div>
                                        <span className="text-muted-foreground">الباركود: </span>
                                        <span>{selectedPackaging.barcode}</span>
                                    </div>
                                )}
                                <div>
                                    <span className="text-muted-foreground">الوحدات: </span>
                                    <span>{selectedPackaging.unitsPerPackage}</span>
                                </div>
                            </div>

                            {selectedPackaging.description && (
                                <p className="text-sm text-muted-foreground">
                                    {selectedPackaging.description}
                                </p>
                            )}

                            {/* Quantity Selector */}
                            <div className="flex items-center justify-between">
                                <span className="font-medium">الكمية:</span>
                                <div className="flex items-center gap-2">
                                    <Button
                                        size="icon"
                                        variant="outline"
                                        onClick={() => handleQuantityChange(-1)}
                                        disabled={quantity <= 1}
                                    >
                                        <Minus className="h-4 w-4" />
                                    </Button>
                                    <span className="w-12 text-center font-semibold text-lg">
                                        {quantity}
                                    </span>
                                    <Button
                                        size="icon"
                                        variant="outline"
                                        onClick={() => handleQuantityChange(1)}
                                        disabled={quantity >= selectedPackaging.availableStock}
                                    >
                                        <Plus className="h-4 w-4" />
                                    </Button>
                                </div>
                            </div>

                            {/* Total */}
                            <div className="flex items-center justify-between pt-2 border-t">
                                <span className="font-medium">الإجمالي:</span>
                                <span className="text-xl font-bold text-primary">
                                    {formatPrice({
                                        amount: selectedPackaging.discountedPrice.amount * quantity,
                                        currency: selectedPackaging.discountedPrice.currency,
                                    })}
                                </span>
                            </div>
                        </div>
                    )}

                    {/* Add to Cart Button */}
                    <Button
                        className="w-full"
                        size="lg"
                        onClick={handleAddToCart}
                        disabled={!selectedPackaging || !selectedPackaging.inStock}
                    >
                        <ShoppingCart className="ml-2 h-5 w-5" />
                        إضافة إلى السلة
                    </Button>
                </div>
            </DialogContent>
        </Dialog>
    );
}
