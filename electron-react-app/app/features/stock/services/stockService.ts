import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult } from '@/app/lib/types/global';
import { AddStockBatchRequest, CreateStockRequest, StockListDto } from '../lib/types';

export interface StockQueryParams {
    pageNumber: number;
    pageSize: number;
    inventoryId?: string;
    productId?: string;
    productPackagingId?: string;
    productName?: string;
    stockStatus?: number;
    reorderLevel?: number;
}

export const stockService = {
    getMyStocks: async (params: StockQueryParams) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<PagedResult<StockListDto>>>('/api/Stock/my', {
            params
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

    getMyBatches: async (params: {
        pageNumber: number;
        pageSize: number;
        batchStatus?: number;
        daysThreshold?: number;
        condition?: number;
    }) => {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<PagedResult<unknown>>>('/api/Stock/batches/my', {
            params
        });
        return response.data.data;
    },
};
