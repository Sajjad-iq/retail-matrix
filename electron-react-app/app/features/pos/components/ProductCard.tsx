import { Card, CardContent } from '@/app/components/ui/card';
import { Badge } from '@/app/components/ui/badge';
import { Package, AlertCircle } from 'lucide-react';
import { PosProductDto } from '../lib/types';
import { formatPrice } from '@/lib/utils';

interface ProductCardProps {
    product: PosProductDto;
    onClick?: () => void;
}

export function ProductCard({ product, onClick }: ProductCardProps) {
    const defaultPackaging = product.packagings.find(p => p.isDefault) || product.packagings[0];
    const hasStock = product.hasStock;

    return (
        <Card
            className={`cursor-pointer transition-all hover:shadow-lg hover:scale-105 ${
                !hasStock ? 'opacity-60' : ''
            }`}
            {...(onClick && { onClick })}
        >
            <CardContent className="p-0">
                {/* Product Image */}
                <div className="relative h-48 bg-muted rounded-t-lg overflow-hidden">
                    {product.imageUrls.length > 0 ? (
                        <img
                            src={product.imageUrls[0]}
                            alt={product.productName}
                            className="w-full h-full object-cover"
                        />
                    ) : (
                        <div className="flex items-center justify-center h-full">
                            <Package className="h-16 w-16 text-muted-foreground" />
                        </div>
                    )}
                    
                    {/* Stock Badge */}
                    <div className="absolute top-2 left-2">
                        {hasStock ? (
                            <Badge variant="default" className="bg-green-500">
                                متوفر
                            </Badge>
                        ) : (
                            <Badge variant="destructive">
                                نفذ
                            </Badge>
                        )}
                    </div>

                    {/* Discount Badge */}
                    {defaultPackaging?.hasDiscount && (
                        <div className="absolute top-2 right-2">
                            <Badge variant="destructive" className="bg-red-500">
                                خصم {defaultPackaging.discountPercentage}%
                            </Badge>
                        </div>
                    )}
                </div>

                {/* Product Info */}
                <div className="p-4 space-y-2">
                    {/* Product Name */}
                    <h3 className="font-semibold text-lg line-clamp-2 min-h-[3.5rem]">
                        {product.productName}
                    </h3>

                    {/* Category */}
                    {product.categoryName && (
                        <p className="text-xs text-muted-foreground">
                            {product.categoryName}
                        </p>
                    )}

                    {/* Price */}
                    {defaultPackaging && (
                        <div className="flex items-baseline gap-2">
                            <span className="text-2xl font-bold text-primary">
                                {formatPrice(defaultPackaging.discountedPrice)}
                            </span>
                            {defaultPackaging.hasDiscount && (
                                <span className="text-sm text-muted-foreground line-through">
                                    {formatPrice(defaultPackaging.sellingPrice)}
                                </span>
                            )}
                        </div>
                    )}

                    {/* Packaging Info */}
                    <div className="flex items-center justify-between text-sm text-muted-foreground pt-2 border-t">
                        <span>{product.totalPackagings} وحدة بيع</span>
                        <span>{product.totalAvailableStock} قطعة</span>
                    </div>

                    {/* Out of Stock Warning */}
                    {!hasStock && (
                        <div className="flex items-center gap-1 text-xs text-destructive">
                            <AlertCircle className="h-3 w-3" />
                            <span>غير متوفر حالياً</span>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
