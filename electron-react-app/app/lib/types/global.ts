// Global Types

export type AccountType = 'BusinessOwner' | 'Employee' | 'Admin';

export type UserRole =
    | 'SuperAdmin'
    | 'Admin'
    | 'Owner'
    | 'Coordinator'
    | 'Cashier'
    | 'Sales'
    | 'InventoryManager'
    | 'User';

export interface User {
    id: string;
    name: string;
    email: string;
    phoneNumber: string;
    address?: string;
    accountType: AccountType;
    memberOfOrganization?: string;
    userRoles: UserRole[];
    isActive: boolean;
    emailVerified: boolean;
    phoneVerified: boolean;
}

export interface ApiResponse<T = any> {
    success: boolean;
    data?: T;
    message?: string;
    errorCode?: string;
    errors?: Record<string, string[]>;
}

export enum OrganizationStatus {
    Active = 0,
    Suspended = 1,
    Pending = 2,
    Closed = 3
}

export interface Organization {
    id: string;
    name: string;
    domain: string;
    description: string;
    address: string;
    phone: string;
    status: OrganizationStatus;
    createdBy: string;
    logoUrl?: string;
    insertDate: string;
}

export interface Barcode {
    value: string;
}

export interface Price {
    amount: number;
    currency: string;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface PaginationParams {
    pageNumber?: number;
    pageSize?: number;
}
