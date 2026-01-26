import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { inventoryService } from '../services/inventoryService';
import { PaginationParams } from '@/app/lib/types/global';
import { AddStockBatchRequest, CreateStockRequest, StockStatus } from '../lib/types';
import { toast } from 'sonner';

export const useMyStocks = (params: PaginationParams & { stockStatus?: StockStatus }) => {
    return useQuery({
        queryKey: ['stocks', 'my', params],
        queryFn: () => inventoryService.getMyStocks(params),
        placeholderData: keepPreviousData,
    });
};

export const useCreateStock = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateStockRequest) => inventoryService.createStock(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إنشاء المخزون بنجاح');
        }
    });
};

export const useAddStockBatch = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ stockId, data }: { stockId: string; data: AddStockBatchRequest }) => inventoryService.addBatch(stockId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إضافة الدفعة بنجاح');
        }
    });
};

export const useMyInventoryOperations = (params: PaginationParams) => {
    return useQuery({
        queryKey: ['inventory-operations', 'my', params],
        queryFn: () => inventoryService.getMyInventoryOperations(params),
        placeholderData: keepPreviousData,
    });
};

export const useCreateInventoryOperation = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: any) => inventoryService.createInventoryOperation(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['inventory-operations'] });
            toast.success('تم تسجيل العملية بنجاح');
        }
    });
};

export const useMyInventories = () => {
    return useQuery({
        queryKey: ['inventories', 'my'],
        queryFn: () => inventoryService.getMyInventories(),
    });
};
