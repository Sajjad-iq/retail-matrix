import { httpService } from '@/app/lib/config/http';
import type {
    ApiResponse,
    TokenDto,
    LoginRequest,
    RegisterRequest,
    User
} from '../lib/types';

export const authService = {
    async login(credentials: LoginRequest): Promise<ApiResponse<TokenDto>> {
        try {
            const axios = httpService.getAxiosInstance();
            const response = await axios.post<ApiResponse<TokenDto>>('/api/auth/login', credentials);
            return response.data;
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Login failed',
                errorCode: error.response?.data?.errorCode || 'LOGIN_ERROR',
                errors: error.response?.data?.errors,
            };
        }
    },

    async register(data: RegisterRequest): Promise<ApiResponse<string>> {
        try {
            const axios = httpService.getAxiosInstance();
            const response = await axios.post<ApiResponse<string>>('/api/auth/register', data);
            return response.data;
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Registration failed',
                errorCode: error.response?.data?.errorCode || 'REGISTER_ERROR',
                errors: error.response?.data?.errors,
            };
        }
    },

    async getCurrentUser(): Promise<ApiResponse<User>> {
        try {
            const axios = httpService.getAxiosInstance();
            const response = await axios.get<ApiResponse<User>>('/api/auth/me');
            return response.data;
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || 'Failed to get user',
                errorCode: error.response?.data?.errorCode || 'GET_USER_ERROR',
            };
        }
    },

    // Client-side logout - just clear tokens
    logout(): void {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
    },
};

