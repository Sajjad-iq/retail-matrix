import { useQuery, keepPreviousData } from '@tanstack/react-query';
import { productService } from '../services/productService';
import { PaginationParams } from '@/app/lib/types/global';

export const useMyProducts = (params: PaginationParams) => {
    return useQuery({
        queryKey: ['products', 'my', params],
        queryFn: () => productService.getMyProducts(params),
        placeholderData: keepPreviousData,
    });
};
