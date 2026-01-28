import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { posService } from '../services/posService';
import { PosProductFilter, CreateSaleRequest } from '../lib/types';
import { toast } from 'sonner';

export const useInventoryProducts = (filters: PosProductFilter) => {
    return useQuery({
        queryKey: ['pos', 'inventory-products', filters],
        queryFn: () => posService.getInventoryProducts(filters),
        placeholderData: keepPreviousData,
        enabled: !!filters.inventoryId,
    });
};

export const useSearchByBarcode = () => {
    return useMutation({
        mutationFn: ({ barcode, inventoryId }: { barcode: string; inventoryId: string }) =>
            posService.searchByBarcode(barcode, inventoryId),
    });
};

export const useGetSale = (saleId: string) => {
    return useQuery({
        queryKey: ['pos', 'sale', saleId],
        queryFn: () => posService.getSale(saleId),
        enabled: !!saleId,
    });
};

export const useCreateSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateSaleRequest) => posService.createSale(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['pos', 'sales'] });
            toast.success('تم حفظ البيع بنجاح');
        },
    });
};

export const useUpdateSale = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ saleId, data }: { saleId: string; data: CreateSaleRequest }) =>
            posService.updateSale(saleId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['pos', 'sales'] });
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
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['pos'] });
            queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إتمام عملية البيع بنجاح');
        },
    });
};
