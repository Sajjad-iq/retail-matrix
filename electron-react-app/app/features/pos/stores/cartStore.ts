import { create } from 'zustand';

interface CartState {
    inventoryId: string | null;
    draftSaleId: string | null;
    
    // Actions
    setInventoryId: (inventoryId: string) => void;
    setDraftSaleId: (draftSaleId: string | null) => void;
}

export const useCartStore = create<CartState>()((set) => ({
    inventoryId: null,
    draftSaleId: null,

    setInventoryId: (inventoryId) => set({ inventoryId, draftSaleId: null }),
    
    setDraftSaleId: (draftSaleId) => set({ draftSaleId }),
}));
