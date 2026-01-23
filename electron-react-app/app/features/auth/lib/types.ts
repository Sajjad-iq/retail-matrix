// Auth Types - Matching Backend DTOs

import type { User, ApiResponse } from '@/app/lib/types/global';

export type { User, ApiResponse };

export interface TokenDto {
    accessToken: string;
    refreshToken: string;
    expiresAt: string;
    user: User;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
    phoneNumber: string;
    address?: string;
}
