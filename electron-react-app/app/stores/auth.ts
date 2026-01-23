import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, TokenDto } from '@/app/features/auth/lib/types';

interface AuthState {
    user: User | null;
    accessToken: string | null;
    refreshToken: string | null;
    expiresAt: string | null;
    isAuthenticated: boolean;
    setAuth: (tokenDto: TokenDto) => void;
    setUser: (user: User) => void;
    clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            accessToken: null,
            refreshToken: null,
            expiresAt: null,
            isAuthenticated: false,

            setAuth: (tokenDto: TokenDto) => {
                // Backend sends tokens as JSON in response body (not HTTP-only cookies)
                // Store in localStorage for HTTP interceptor to attach to requests
                // Note: For production Electron app, consider using Electron's safeStorage API
                localStorage.setItem('accessToken', tokenDto.accessToken);
                localStorage.setItem('refreshToken', tokenDto.refreshToken);
                localStorage.setItem('user', JSON.stringify(tokenDto.user));

                set({
                    user: tokenDto.user,
                    accessToken: tokenDto.accessToken,
                    refreshToken: tokenDto.refreshToken,
                    expiresAt: tokenDto.expiresAt,
                    isAuthenticated: true,
                });
            },

            setUser: (user: User) => {
                localStorage.setItem('user', JSON.stringify(user));
                set({ user });
            },

            clearAuth: () => {
                // Clear localStorage
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
                localStorage.removeItem('user');

                set({
                    user: null,
                    accessToken: null,
                    refreshToken: null,
                    expiresAt: null,
                    isAuthenticated: false,
                });
            },
        }),
        {
            name: 'auth-storage',
        }
    )
);
