import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { LocalCartItem } from '../lib/types';

interface CartState {
    inventoryId: string | null;
    items: LocalCartItem[];
    
    // Actions
    setInventoryId: (inventoryId: string) => void;
    addItem: (item: LocalCartItem) => void;
    updateQuantity: (productPackagingId: string, quantity: number) => void;
    removeItem: (productPackagingId: string) => void;
    clearCart: () => void;
    
    // Computed
    getTotalItems: () => number;
    getSubtotal: () => number;
    getTotalDiscount: () => number;
    getTotal: () => number;
    getCurrency: () => string;
}

export const useCartStore = create<CartState>()(
    persist(
        (set, get) => ({
            inventoryId: null,
            items: [],

            setInventoryId: (inventoryId) => {
                const currentInventoryId = get().inventoryId;
                if (currentInventoryId !== inventoryId) {
                    // Clear cart when switching inventory
                    set({ inventoryId, items: [] });
                } else {
                    set({ inventoryId });
                }
            },
            
            addItem: (item) => {
                const existingItemIndex = get().items.findIndex(
                    i => i.productPackagingId === item.productPackagingId
                );
                
                if (existingItemIndex >= 0) {
                    // Update existing item quantity
                    const newItems = [...get().items];
                    newItems[existingItemIndex] = {
                        ...newItems[existingItemIndex],
                        quantity: newItems[existingItemIndex].quantity + item.quantity,
                    };
                    set({ items: newItems });
                } else {
                    // Add new item
                    set({ items: [...get().items, item] });
                }
            },
            
            updateQuantity: (productPackagingId, quantity) => {
                if (quantity <= 0) {
                    get().removeItem(productPackagingId);
                    return;
                }
                
                const newItems = get().items.map(item =>
                    item.productPackagingId === productPackagingId
                        ? { ...item, quantity }
                        : item
                );
                set({ items: newItems });
            },
            
            removeItem: (productPackagingId) => {
                set({ items: get().items.filter(i => i.productPackagingId !== productPackagingId) });
            },
            
            clearCart: () => {
                set({ items: [] });
            },
            
            getTotalItems: () => {
                return get().items.reduce((sum, item) => sum + item.quantity, 0);
            },
            
            getSubtotal: () => {
                return get().items.reduce((sum, item) => {
                    const price = item.discount 
                        ? item.unitPrice.amount * (1 - item.discount.value / 100)
                        : item.unitPrice.amount;
                    return sum + (price * item.quantity);
                }, 0);
            },
            
            getTotalDiscount: () => {
                return get().items.reduce((sum, item) => {
                    if (!item.discount) return sum;
                    const discountAmount = item.unitPrice.amount * (item.discount.value / 100) * item.quantity;
                    return sum + discountAmount;
                }, 0);
            },
            
            getTotal: () => {
                return get().getSubtotal();
            },
            
            getCurrency: () => {
                const firstItem = get().items[0];
                return firstItem?.unitPrice?.currency || 'IQD';
            },
        }),
        {
            name: 'pos-cart-storage',
            partialize: (state) => ({ 
                inventoryId: state.inventoryId,
                items: state.items 
            }),
        }
    )
);
