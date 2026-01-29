import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { posService } from '../services/posService';
import { PosProductFilter, CreateSaleRequest } from '../lib/types';
import { toast } from 'sonner';

// Note: useSearchByBarcode is kept for future barcode scanner implementation

export const useInventoryProducts = (filters: PosProductFilter) => {
    return useQuery({
        queryKey: ['pos', 'inventory-products', filters],
        queryFn: () => posService.getInventoryProducts(filters),
        placeholderData: keepPreviousData,
        enabled: !!filters.inventoryId,
    });
};

export const useDraftSale = (inventoryId: string | null) => {
    return useQuery({
        queryKey: ['pos', 'draft-sale', inventoryId],
        queryFn: () => posService.getOrCreateDraftSale(inventoryId!),
        enabled: !!inventoryId,
        staleTime: 0, // Always fetch fresh data
        refetchOnWindowFocus: false, // Prevent automatic refetch that could cause race conditions
    });
};

export const useSearchByBarcode = () => {
    return useMutation({
        mutationFn: ({ barcode, inventoryId }: { barcode: string; inventoryId: string }) =>
            posService.searchByBarcode(barcode, inventoryId),
    });
};

export const useUpdateSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ saleId, data }: { saleId: string; data: CreateSaleRequest }) =>
            posService.updateSale(saleId, data),
        onSuccess: async (_data, variables) => {
            // Await invalidation to ensure cache is updated before proceeding
            await queryClient.invalidateQueries({ queryKey: ['pos', 'sale', variables.saleId] });
            await queryClient.invalidateQueries({ queryKey: ['pos', 'draft-sale'] });
            toast.success('تم تحديث البيع بنجاح');
        },
    });
};

export const useCancelSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (saleId: string) => posService.cancelSale(saleId),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['pos', 'sales'] });
            toast.success('تم إلغاء البيع');
        },
    });
};

export const useCompleteSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ saleId, inventoryId, amountPaid }: { saleId: string; inventoryId: string; amountPaid: number }) =>
            posService.completeSale(saleId, { inventoryId, amountPaid }),
        onSuccess: async () => {
            // CRITICAL: Completely clear all POS-related queries to force fresh draft sale creation
            await queryClient.invalidateQueries({ queryKey: ['pos'] });
            await queryClient.invalidateQueries({ queryKey: ['stocks'] });
            // Remove all cached draft sale data to force a new draft on next interaction
            queryClient.removeQueries({ queryKey: ['pos', 'draft-sale'] });
            toast.success('تم إتمام عملية البيع بنجاح');
        },
    });
};
