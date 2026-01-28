import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { CartItem, PosPackagingDto } from '../lib/types';
import { Price } from '@/app/lib/types/global';

interface CartState {
    items: CartItem[];
    inventoryId: string | null;
    draftSaleId: string | null;
    
    // Actions
    setInventoryId: (inventoryId: string) => void;
    setDraftSaleId: (draftSaleId: string | null) => void;
    addItem: (product: { productId: string; productName: string; imageUrl?: string }, packaging: PosPackagingDto, quantity: number) => void;
    updateQuantity: (itemId: string, quantity: number) => void;
    removeItem: (itemId: string) => void;
    clearCart: () => void;
    
    // Computed
    getSubtotal: () => Price;
    getTotalDiscount: () => Price;
    getTotal: () => Price;
    getItemCount: () => number;
}

const calculateLineTotal = (price: Price, quantity: number): Price => {
    return {
        amount: price.amount * quantity,
        currency: price.currency
    };
};

export const useCartStore = create<CartState>()(
    persist(
        (set, get) => ({
            items: [],
            inventoryId: null,
            draftSaleId: null,

            setInventoryId: (inventoryId) => set({ inventoryId }),
            
            setDraftSaleId: (draftSaleId) => set({ draftSaleId }),

            addItem: (product, packaging, quantity) => {
                const items = get().items;
                
                // Check if item already exists
                const existingItemIndex = items.findIndex(
                    item => item.packagingId === packaging.packagingId
                );

                if (existingItemIndex >= 0) {
                    // Update quantity
                    const updatedItems = [...items];
                    const newQuantity = updatedItems[existingItemIndex].quantity + quantity;
                    updatedItems[existingItemIndex] = {
                        ...updatedItems[existingItemIndex],
                        quantity: newQuantity,
                        lineTotal: calculateLineTotal(packaging.discountedPrice, newQuantity)
                    };
                    set({ items: updatedItems });
                } else {
                    // Add new item
                    const newItem: CartItem = {
                        id: `${packaging.packagingId}-${Date.now()}`,
                        productId: product.productId,
                        productName: product.productName,
                        packagingId: packaging.packagingId,
                        packagingName: packaging.packagingName,
                        barcode: packaging.barcode,
                        quantity,
                        unitPrice: packaging.sellingPrice,
                        discountedPrice: packaging.discountedPrice,
                        discount: packaging.discount,
                        lineTotal: calculateLineTotal(packaging.discountedPrice, quantity),
                        availableStock: packaging.availableStock,
                        imageUrl: product.imageUrl || packaging.imageUrls[0]
                    };
                    set({ items: [...items, newItem] });
                }
            },

            updateQuantity: (itemId, quantity) => {
                if (quantity <= 0) {
                    get().removeItem(itemId);
                    return;
                }

                const items = get().items;
                const updatedItems = items.map(item =>
                    item.id === itemId
                        ? {
                            ...item,
                            quantity,
                            lineTotal: calculateLineTotal(item.discountedPrice, quantity)
                        }
                        : item
                );
                set({ items: updatedItems });
            },

            removeItem: (itemId) => {
                const items = get().items;
                set({ items: items.filter(item => item.id !== itemId) });
            },

            clearCart: () => set({ items: [], draftSaleId: null }),

            getSubtotal: () => {
                const items = get().items;
                if (items.length === 0) return { amount: 0, currency: 'IQD' };
                
                const total = items.reduce((sum, item) => sum + item.lineTotal.amount, 0);
                return { amount: total, currency: items[0].lineTotal.currency };
            },

            getTotalDiscount: () => {
                const items = get().items;
                if (items.length === 0) return { amount: 0, currency: 'IQD' };
                
                const discount = items.reduce((sum, item) => {
                    const originalTotal = item.unitPrice.amount * item.quantity;
                    const discountedTotal = item.lineTotal.amount;
                    return sum + (originalTotal - discountedTotal);
                }, 0);
                
                return { amount: discount, currency: items[0].lineTotal.currency };
            },

            getTotal: () => {
                return get().getSubtotal();
            },

            getItemCount: () => {
                const items = get().items;
                return items.reduce((sum, item) => sum + item.quantity, 0);
            }
        }),
        {
            name: 'pos-cart-storage',
        }
    )
);
