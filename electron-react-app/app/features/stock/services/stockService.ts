import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult, PaginationParams } from '@/app/lib/types/global';
import { AddStockBatchRequest, CreateStockRequest, StockListDto, StockStatus } from '../lib/types';

export const stockService = {
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
};
