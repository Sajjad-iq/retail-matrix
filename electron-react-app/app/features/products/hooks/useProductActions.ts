import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { productService } from '../services/productService';
import { PaginationParams } from '@/app/lib/types/global';
import { CreateProductRequest } from '../lib/validations';
import { toast } from 'sonner';

export const useMyProducts = (params: PaginationParams) => {
    return useQuery({
        queryKey: ['products', 'my', params],
        queryFn: () => productService.getMyProducts(params),
        placeholderData: keepPreviousData,
    });
};

export const useCreateProduct = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateProductRequest) => productService.createProduct(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['products'] });
            toast.success('تم إضافة المنتج بنجاح');
        }
    });
};

export const useUpdateProduct = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: any) => productService.updateProduct(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['products'] });
            toast.success('تم تحديث المنتج بنجاح');
        }
    });
};

export const useDeleteProduct = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => productService.deleteProduct(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['products'] });
            toast.success('تم حذف المنتج بنجاح');
        }
    });
};
