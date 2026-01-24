import { useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { useAuthStore } from '@/app/stores/auth';
import { authService } from '../services/authService';
import type { LoginRequest, RegisterRequest } from '../lib/types';

/**
 * Hook to initialize auth state on app mount
 * Calls /me to validate token and fetch fresh user data
 */
export function useAuthInit() {
    const { accessToken, setUser, clearAuth } = useAuthStore();

    useEffect(() => {
        if (!accessToken) return;

        authService.getCurrentUser().then((result) => {
            if (result.success && result.data) {
                setUser(result.data);
            } else {
                // Token is invalid or expired, clear auth
                clearAuth();
            }
        });
    }, []);
}

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
            }
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
            }
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
