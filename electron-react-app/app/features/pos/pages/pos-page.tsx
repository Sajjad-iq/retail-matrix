import { useState, useEffect } from 'react';
import { Button } from '@/app/components/ui/button';
import { Input } from '@/app/components/ui/input';
import { Search, Grid3x3, Loader2 } from 'lucide-react';
import { ProductCard } from '../components/ProductCard';
import { ProductDialog } from '../components/ProductDialog';
import { CartDialog } from '../components/CartDialog';
import { CheckoutDialog } from '../components/CheckoutDialog';
import { FloatingCartButton } from '../components/FloatingCartButton';
import { useInventoryProducts, useDraftSale } from '../hooks/usePosActions';
import { useCartStore } from '../stores/cartStore';
import { PosProductDto, SaleDto } from '../lib/types';
import { useMyInventories } from '@/app/features/locations/hooks/useInventoryActions';

export default function PosPage() {
    const [selectedProduct, setSelectedProduct] = useState<PosProductDto | null>(null);
    const [isProductDialogOpen, setIsProductDialogOpen] = useState(false);
    const [isCartDialogOpen, setIsCartDialogOpen] = useState(false);
    const [isCheckoutDialogOpen, setIsCheckoutDialogOpen] = useState(false);
    const [checkoutDraftSale, setCheckoutDraftSale] = useState<SaleDto | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [page, setPage] = useState(0);
    const [pageSize] = useState(50);
    const [selectedInventoryId, setSelectedInventoryId] = useState<string>('');

    const setInventoryId = useCartStore(state => state.setInventoryId);
    const setDraftSaleId = useCartStore(state => state.setDraftSaleId);

    // Get inventories for selection
    const { data: inventoriesData } = useMyInventories({ pageNumber: 1, pageSize: 100 });

    // Initialize draft sale for selected inventory
    const { data: draftSale } = useDraftSale(selectedInventoryId || null);

    // Auto-select first inventory
    useEffect(() => {
        if (!selectedInventoryId && inventoriesData?.items && inventoriesData.items.length > 0) {
            const firstInventory = inventoriesData.items[0];
            setSelectedInventoryId(firstInventory.id);
            setInventoryId(firstInventory.id);
        }
    }, [selectedInventoryId, inventoriesData, setInventoryId]);

    // Set draft sale ID when it's loaded
    useEffect(() => {
        if (draftSale?.saleId) {
            setDraftSaleId(draftSale.saleId);
        }
    }, [draftSale, setDraftSaleId]);

    // Fetch products
    const { data: productsData, isLoading } = useInventoryProducts({
        inventoryId: selectedInventoryId,
        pageNumber: page + 1,
        pageSize,
        searchTerm: searchTerm || undefined,
        inStock: true, // Only show items in stock
    });

    const handleProductClick = (product: PosProductDto) => {
        setSelectedProduct(product);
        setIsProductDialogOpen(true);
    };

    const handleCheckout = (draftSaleData: SaleDto) => {
        setCheckoutDraftSale(draftSaleData);
        setIsCartDialogOpen(false);
        setIsCheckoutDialogOpen(true);
    };

    const handleCheckoutSuccess = () => {
        setIsCheckoutDialogOpen(false);
        setCheckoutDraftSale(null);
    };

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        setPage(0);
    };

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            {/* Header */}
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">نقاط البيع</h2>
                    <p className="text-muted-foreground">
                        إدارة عمليات البيع والمبيعات
                    </p>
                </div>
            </div>

            {/* Inventory Selector */}
            {inventoriesData && inventoriesData.items.length > 0 && (
                <div className="flex gap-4 items-center">
                    <label className="text-sm font-medium">المخزن:</label>
                    <div className="flex gap-2">
                        {inventoriesData.items.map((inventory) => (
                            <Button
                                key={inventory.id}
                                variant={selectedInventoryId === inventory.id ? 'default' : 'outline'}
                                onClick={() => {
                                    setSelectedInventoryId(inventory.id);
                                    setInventoryId(inventory.id);
                                }}
                            >
                                {inventory.name}
                            </Button>
                        ))}
                    </div>
                </div>
            )}

            {/* Search */}
            <form onSubmit={handleSearch} className="flex gap-2">
                <div className="relative flex-1">
                    <Search className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                        placeholder="ابحث عن منتج..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="pr-10"
                    />
                </div>
                <Button type="submit">
                    بحث
                </Button>
            </form>

            {/* Products Grid */}
            {!selectedInventoryId ? (
                <div className="flex items-center justify-center h-64 text-muted-foreground">
                    <div className="text-center">
                        <Grid3x3 className="h-16 w-16 mx-auto mb-4 opacity-50" />
                        <p>الرجاء اختيار مخزن للبدء</p>
                    </div>
                </div>
            ) : isLoading ? (
                <div className="flex items-center justify-center h-64">
                    <Loader2 className="h-8 w-8 animate-spin text-primary" />
                </div>
            ) : productsData && productsData.items.length > 0 ? (
                <>
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                        {productsData.items.map((product) => (
                            <ProductCard
                                key={product.productId}
                                product={product}
                                onClick={() => handleProductClick(product)}
                            />
                        ))}
                    </div>

                    {/* Pagination */}
                    {productsData.totalPages > 1 && (
                        <div className="flex items-center justify-center gap-2 mt-4">
                            <Button
                                variant="outline"
                                onClick={() => setPage(Math.max(0, page - 1))}
                                disabled={page === 0}
                            >
                                السابق
                            </Button>
                            <span className="text-sm text-muted-foreground">
                                صفحة {page + 1} من {productsData.totalPages}
                            </span>
                            <Button
                                variant="outline"
                                onClick={() => setPage(Math.min(productsData.totalPages - 1, page + 1))}
                                disabled={page >= productsData.totalPages - 1}
                            >
                                التالي
                            </Button>
                        </div>
                    )}
                </>
            ) : (
                <div className="flex items-center justify-center h-64 text-muted-foreground">
                    <div className="text-center">
                        <Grid3x3 className="h-16 w-16 mx-auto mb-4 opacity-50" />
                        <p>لا توجد منتجات متاحة</p>
                        {searchTerm && <p className="text-sm mt-2">جرب البحث بكلمة مختلفة</p>}
                    </div>
                </div>
            )}

            {/* Product Dialog */}
            <ProductDialog
                product={selectedProduct}
                open={isProductDialogOpen}
                onClose={() => {
                    setIsProductDialogOpen(false);
                    setSelectedProduct(null);
                }}
            />

            {/* Cart Dialog */}
            <CartDialog
                open={isCartDialogOpen}
                onClose={() => setIsCartDialogOpen(false)}
                onCheckout={handleCheckout}
            />

            {/* Checkout Dialog */}
            <CheckoutDialog
                open={isCheckoutDialogOpen}
                onClose={() => {
                    setIsCheckoutDialogOpen(false);
                    setCheckoutDraftSale(null);
                }}
                onSuccess={handleCheckoutSuccess}
                draftSale={checkoutDraftSale}
            />

            {/* Floating Cart Button */}
            <FloatingCartButton onClick={() => setIsCartDialogOpen(true)} />
        </div>
    );
}
