import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult, PaginationParams } from '@/app/lib/types/global';
import { AddStockBatchRequest, CreateStockRequest, InventoryOperationDto, StockListDto, StockStatus } from '../lib/types';

export const inventoryService = {
    getMyStocks: async (params: PaginationParams & { stockStatus?: StockStatus }) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<PagedResult<StockListDto>>>('/api/Stock/my', {
            params: {
                pageNumber: params.pageNumber,
                pageSize: params.pageSize,
                stockStatus: params.stockStatus
            }
        });
        return response.data.data;
    },

    createStock: async (data: CreateStockRequest) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.post<ApiResponse<string>>('/api/Stock', data);
        return response.data.data;
    },

    addBatch: async (stockId: string, data: AddStockBatchRequest) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.post<ApiResponse<string>>(`/api/Stock/${stockId}/batch`, data);
        return response.data.data;
    },

    getMyInventoryOperations: async (params: PaginationParams) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<PagedResult<InventoryOperationDto>>>('/api/InventoryOperation/my', {
            params
        });
        return response.data.data;
    },

    createInventoryOperation: async (data: any) => { // Type as CreateInventoryOperationRequest
        const axios = httpService.getAxiosInstance();
        const response = await axios.post<ApiResponse<string>>('/api/InventoryOperation', data);
        return response.data.data;
    },

    getMyInventories: async () => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<any>>('/api/InventoryOperation/inventories/my');
        return response.data.data; // Adjust return type if PagedResult
    }
};
