import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { posService } from '../services/posService';
import { PosProductFilter, CreateAndCompleteSaleRequest } from '../lib/types';
import { toast } from 'sonner';

// Fetch products for inventory
export const useInventoryProducts = (filters: PosProductFilter) => {
    return useQuery({
        queryKey: ['pos', 'inventory-products', filters],
        queryFn: () => posService.getInventoryProducts(filters),
        placeholderData: keepPreviousData,
        enabled: !!filters.inventoryId,
    });
};

// Search by barcode (for future barcode scanner implementation)
export const useSearchByBarcode = () => {
    return useMutation({
        mutationFn: ({ barcode, inventoryId }: { barcode: string; inventoryId: string }) =>
            posService.searchByBarcode(barcode, inventoryId),
    });
};

// Create and complete sale in one transaction
export const useCreateAndCompleteSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateAndCompleteSaleRequest) =>
            posService.createAndCompleteSale(data),
        onSuccess: async () => {
            // Refresh inventory products to show updated stock
            await queryClient.invalidateQueries({ queryKey: ['pos', 'inventory-products'] });
            await queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إتمام عملية البيع بنجاح');
        },
        onError: () => {
            toast.error('حدث خطأ أثناء إتمام البيع');
        },
    });
};
