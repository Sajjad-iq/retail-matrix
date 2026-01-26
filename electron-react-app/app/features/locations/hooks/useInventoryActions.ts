import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { inventoryService } from '../services/inventoryService';
import { CreateInventoryRequest, UpdateInventoryRequest } from '../lib/types';
import { PaginationParams } from '@/app/lib/types/global';

const QUERY_KEY = 'inventories';

export function useMyInventories(params: PaginationParams) {
    return useQuery({
        queryKey: [QUERY_KEY, 'my', params],
        queryFn: () => inventoryService.getMyInventories(params),
    });
}

export function useInventoryById(id: string, enabled = true) {
    return useQuery({
        queryKey: [QUERY_KEY, id],
        queryFn: () => inventoryService.getInventoryById(id),
        enabled: enabled && !!id,
    });
}

export function useCreateInventory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateInventoryRequest) => inventoryService.createInventory(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [QUERY_KEY] });
            toast.success('تم إنشاء المخزن بنجاح');
        },
        onError: () => {
            // Error handled by http interceptor
        },
    });
}

export function useUpdateInventory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: UpdateInventoryRequest) => inventoryService.updateInventory(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [QUERY_KEY] });
            toast.success('تم تحديث المخزن بنجاح');
        },
        onError: () => {
            // Error handled by http interceptor
        },
    });
}
