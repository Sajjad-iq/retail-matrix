import { Price } from "@/app/lib/types/global";

export enum StockStatus {
    All = 0,
    LowStock = 1,
    OutOfStock = 2
}

export enum BatchStatus {
    All = 0,
    Expired = 1,
    NearExpiry = 2
}

export enum StockCondition {
    New = 0,
    Damaged = 1,
    Refurbished = 2,
    Used = 3
}

export interface StockBatchDto {
    id: string;
    stockId: string;
    batchNumber: string;
    quantity: number;
    reservedQuantity: number;
    availableQuantity: number;
    expiryDate?: string;
    condition: StockCondition;
    costPrice?: Price;
    insertDate: string;
    isExpired: boolean;
    daysUntilExpiry?: number;
}

export interface StockDto {
    id: string;
    productPackagingId: string;
    inventoryId: string;
    totalQuantity: number;
    totalAvailableQuantity: number;
    insertDate: string;
    batchesPromise?: StockBatchDto[];
}

export interface StockListDto {
    id: string;
    productPackagingId: string;
    inventoryId: string;
    totalQuantity: number;
    totalAvailableQuantity: number;
    insertDate: string;
    productName?: string;
    packagingName?: string;
    inventoryName?: string;
    batches: StockBatchDto[];
}

export interface CreateStockRequest {
    productPackagingId: string;
    inventoryId: string;
    initialBatchNumber?: string;
    initialQuantity?: number;
    initialExpiryDate?: string;
    initialCondition?: StockCondition;
    initialCostPrice?: Price;
}

export interface AddStockBatchRequest {
    batchNumber: string;
    quantity: number;
    expiryDate?: string;
    condition?: StockCondition;
    costPrice?: Price;
}
