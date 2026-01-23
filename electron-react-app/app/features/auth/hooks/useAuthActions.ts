import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { useAuthStore } from '@/app/stores/auth';
import { authService } from '../services/authService';
import type { LoginRequest, RegisterRequest } from '../lib/types';

/**
 * Hook for login mutation
 */
export function useLogin() {
    const { setAuth } = useAuthStore();
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (credentials: LoginRequest) => authService.login(credentials),
        onSuccess: (result) => {
            if (result.success && result.data) {
                setAuth(result.data);
                toast.success(result.message || 'تم تسجيل الدخول بنجاح');

                // Invalidate and refetch user query
                queryClient.invalidateQueries({ queryKey: ['currentUser'] });
            } else {
                toast.error(result.message || 'فشل تسجيل الدخول');
            }
        },
        onError: (error: any) => {
            console.error('[useLogin] Login error:', error);
            toast.error(error.message || 'فشل تسجيل الدخول');
        },
    });
}

/**
 * Hook for register mutation
 */
export function useRegister() {
    return useMutation({
        mutationFn: (data: RegisterRequest) => authService.register(data),
        onSuccess: (result) => {
            if (result.success) {
                toast.success(result.message || 'تم التسجيل بنجاح! يمكنك الآن تسجيل الدخول');
            } else {
                toast.error(result.message || 'فشل التسجيل');

                // Show validation errors if any
                if (result.errors) {
                    Object.entries(result.errors).forEach(([field, messages]) => {
                        messages.forEach(msg => toast.error(`${field}: ${msg}`));
                    });
                }
            }
        },
        onError: (error: any) => {
            console.error('[useRegister] Register error:', error);
            toast.error(error.message || 'فشل التسجيل');
        },
    });
}

/**
 * Hook for getting current user
 */
export function useCurrentUser() {
    const { isAuthenticated } = useAuthStore();

    return useQuery({
        queryKey: ['currentUser'],
        queryFn: async () => {
            const result = await authService.getCurrentUser();
            if (result.success && result.data) {
                return result.data;
            }
            throw new Error(result.message || 'Failed to get user');
        },
        enabled: isAuthenticated,
        staleTime: 5 * 60 * 1000, // 5 minutes
        retry: false,
    });
}

/**
 * Hook for logout
 */
export function useLogout() {
    const { clearAuth } = useAuthStore();
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => {
            authService.logout();
            return Promise.resolve();
        },
        onSuccess: () => {
            clearAuth();
            queryClient.clear(); // Clear all queries
            toast.success('تم تسجيل الخروج بنجاح');
        },
    });
}
