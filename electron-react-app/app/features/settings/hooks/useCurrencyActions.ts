import { useQuery, useMutation, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { currencyService } from '../services/currencyService';
import { CurrencyFilter, CreateCurrencyRequest, UpdateCurrencyRequest } from '../lib/types';
import { toast } from 'sonner';

export const useMyCurrencies = (filters: CurrencyFilter) => {
    return useQuery({
        queryKey: ['currencies', 'my', filters],
        queryFn: () => currencyService.getMyCurrencies(filters),
        placeholderData: keepPreviousData,
    });
};

export const useCreateCurrency = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateCurrencyRequest) => currencyService.createCurrency(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            toast.success('تم إنشاء العملة بنجاح');
        },
    });
};

export const useUpdateCurrency = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateCurrencyRequest }) =>
            currencyService.updateCurrency(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            toast.success('تم تحديث العملة بنجاح');
        },
    });
};

export const useSetBaseCurrency = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => currencyService.setBaseCurrency(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            toast.success('تم تعيين العملة الأساسية بنجاح');
        },
    });
};

export const useDeleteCurrency = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => currencyService.deleteCurrency(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['currencies'] });
            toast.success('تم حذف العملة بنجاح');
        },
    });
};
