import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult, PaginationParams } from '@/app/lib/types/global';
import { ProductWithPackagingsDto } from '../lib/types';

export const productService = {
    getMyProducts: async (params: PaginationParams = {}) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<ProductWithPackagingsDto>>>('/api/ProductPackaging/products/my', {
            params,
        });
        return response.data.data;
    }
};
