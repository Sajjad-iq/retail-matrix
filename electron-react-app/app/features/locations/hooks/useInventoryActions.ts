import { useMutation, useQuery, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { toast } from 'sonner';
import { inventoryLocationService } from '../services/inventoryService';
import { CreateInventoryRequest, UpdateInventoryRequest, CreateInventoryOperationRequest } from '../lib/types';
import { PaginationParams } from '@/app/lib/types/global';

const INVENTORY_QUERY_KEY = 'inventories';
const OPERATIONS_QUERY_KEY = 'inventory-operations';

// Inventory Location Hooks
export function useMyInventories(params: PaginationParams) {
    return useQuery({
        queryKey: [INVENTORY_QUERY_KEY, 'my', params],
        queryFn: () => inventoryLocationService.getMyInventories(params),
    });
}

export function useInventoryById(id: string, enabled = true) {
    return useQuery({
        queryKey: [INVENTORY_QUERY_KEY, id],
        queryFn: () => inventoryLocationService.getInventoryById(id),
        enabled: enabled && !!id,
    });
}

export function useCreateInventory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateInventoryRequest) => inventoryLocationService.createInventory(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [INVENTORY_QUERY_KEY] });
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
        mutationFn: (data: UpdateInventoryRequest) => inventoryLocationService.updateInventory(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [INVENTORY_QUERY_KEY] });
            toast.success('تم تحديث المخزن بنجاح');
        },
        onError: () => {
            // Error handled by http interceptor
        },
    });
}

// Inventory Operations Hooks
export function useMyInventoryOperations(params: PaginationParams) {
    return useQuery({
        queryKey: [OPERATIONS_QUERY_KEY, 'my', params],
        queryFn: () => inventoryLocationService.getMyInventoryOperations(params),
        placeholderData: keepPreviousData,
    });
}

export function useCreateInventoryOperation() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateInventoryOperationRequest) => inventoryLocationService.createInventoryOperation(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [OPERATIONS_QUERY_KEY] });
            toast.success('تم تسجيل العملية بنجاح');
        },
        onError: () => {
            // Error handled by http interceptor
        },
    });
}

export function useInventoryOperationById(id: string, enabled = true) {
    return useQuery({
        queryKey: [OPERATIONS_QUERY_KEY, id],
        queryFn: () => inventoryLocationService.getInventoryOperationById(id),
        enabled: enabled && !!id,
    });
}
