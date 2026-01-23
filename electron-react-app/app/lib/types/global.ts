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
