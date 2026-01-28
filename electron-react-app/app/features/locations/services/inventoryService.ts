import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult, PaginationParams } from '@/app/lib/types/global';
import { InventoryDto, CreateInventoryRequest, UpdateInventoryRequest, InventoryOperationDto, CreateInventoryOperationRequest } from '../lib/types';

export const inventoryLocationService = {
    getMyInventories: async (params: PaginationParams = {}) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<InventoryDto>>>('/api/Inventory/my', {
            params,
        });
        return response.data.data;
    },

    createInventory: async (data: CreateInventoryRequest) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<string>>('/api/Inventory', data);
        return response.data.data;
    },

    updateInventory: async (data: UpdateInventoryRequest) => {
        const response = await httpService.getAxiosInstance().put<ApiResponse<boolean>>(`/api/Inventory/${data.id}`, data);
        return response.data.data;
    },

    getInventoryById: async (id: string) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<InventoryDto>>(`/api/Inventory/${id}`);
        return response.data.data;
    },

    // Inventory Operations
    getMyInventoryOperations: async (params: PaginationParams) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<InventoryOperationDto>>>('/api/Inventory/operations/my', {
            params
        });
        return response.data.data;
    },

    createInventoryOperation: async (data: CreateInventoryOperationRequest) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<string>>('/api/Inventory/operations', data);
        return response.data.data;
    },

    getInventoryOperationById: async (id: string) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<InventoryOperationDto>>(`/api/Inventory/operations/${id}`);
        return response.data.data;
    },

    getOperationItems: async (operationId: string) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<any[]>>(`/api/Inventory/operations/${operationId}/items`);
        return response.data.data;
    },
};
