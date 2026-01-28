import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult } from '@/app/lib/types/global';
import { CurrencyDto, CurrencyFilter, CreateCurrencyRequest, UpdateCurrencyRequest } from '../lib/types';

export const currencyService = {
    getMyCurrencies: async (filters: CurrencyFilter = {}) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<CurrencyDto>>>(
            '/api/Currency/my',
            { params: filters }
        );
        return response.data.data;
    },

    createCurrency: async (data: CreateCurrencyRequest) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<string>>(
            '/api/Currency',
            data
        );
        return response.data.data;
    },

    updateCurrency: async (id: string, data: UpdateCurrencyRequest) => {
        const response = await httpService.getAxiosInstance().put<ApiResponse<boolean>>(
            `/api/Currency/${id}`,
            data
        );
        return response.data.data;
    },

    setBaseCurrency: async (id: string) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<boolean>>(
            `/api/Currency/${id}/set-base`,
            {}
        );
        return response.data.data;
    },

    deleteCurrency: async (id: string) => {
        const response = await httpService.getAxiosInstance().delete<ApiResponse<boolean>>(
            `/api/Currency/${id}`
        );
        return response.data.data;
    },
};
