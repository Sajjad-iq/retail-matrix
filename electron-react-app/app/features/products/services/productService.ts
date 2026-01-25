import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult, PaginationParams } from '@/app/lib/types/global';
import { ProductWithPackagingsDto } from '../lib/types';
import { CreateProductRequest } from '../lib/validations';

export const productService = {
    getMyProducts: async (params: PaginationParams = {}) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<ProductWithPackagingsDto>>>('/api/ProductPackaging/products/my', {
            params,
        });
        return response.data.data;
    },

    createProduct: async (data: CreateProductRequest) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<string>>('/api/ProductPackaging', data);
        return response.data.data;
    },

    updateProduct: async (data: any) => {
        const response = await httpService.getAxiosInstance().put<ApiResponse<string>>('/api/ProductPackaging', data);
        return response.data.data;
    },

    deleteProduct: async (id: string) => {
        const response = await httpService.getAxiosInstance().delete<ApiResponse<string>>(`/api/ProductPackaging/${id}`);
        return response.data.data;
    },

    createPackaging: async (data: any) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<string>>('/api/ProductPackaging', data);
        return response.data.data;
    },

    updatePackaging: async (data: any) => {
        const response = await httpService.getAxiosInstance().put<ApiResponse<string>>('/api/ProductPackaging/packaging', data);
        return response.data.data;
    },

    deletePackaging: async (id: string) => {
        const response = await httpService.getAxiosInstance().delete<ApiResponse<string>>(`/api/ProductPackaging/packaging/${id}`);
        return response.data.data;
    },
};
