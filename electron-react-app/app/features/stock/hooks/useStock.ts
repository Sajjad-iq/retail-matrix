import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { stockService, StockQueryParams } from '../services/stockService';
import { AddStockBatchRequest, CreateStockRequest } from '../lib/types';
import { toast } from 'sonner';

export const useMyStocks = (params: StockQueryParams) => {
    return useQuery({
        queryKey: ['stocks', 'my', params],
        queryFn: () => stockService.getMyStocks(params),
        placeholderData: keepPreviousData,
    });
};

export const useCreateStock = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateStockRequest) => stockService.createStock(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إنشاء المخزون بنجاح');
        }
    });
};

export const useAddStockBatch = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ stockId, data }: { stockId: string; data: AddStockBatchRequest }) => stockService.addBatch(stockId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['stocks'] });
            toast.success('تم إضافة الدفعة بنجاح');
        }
    });
};
