import { PaginationParams } from "@/app/lib/types/global";

export enum CurrencyStatus {
    Active = 0,
    Inactive = 1
}

export interface CurrencyDto {
    id: string;
    code: string;
    name: string;
    symbol: string;
    exchangeRate: number;
    isBaseCurrency: boolean;
    status: CurrencyStatus;
    organizationId: string;
    insertDate: string;
}

export interface CreateCurrencyRequest {
    code: string;
    name: string;
    symbol: string;
    exchangeRate: number;
    isBaseCurrency: boolean;
    status: CurrencyStatus;
}

export interface UpdateCurrencyRequest {
    name: string;
    symbol: string;
    exchangeRate: number;
    status: CurrencyStatus;
}

export interface CurrencyFilter extends PaginationParams {
    status?: CurrencyStatus;
    searchTerm?: string;
}
