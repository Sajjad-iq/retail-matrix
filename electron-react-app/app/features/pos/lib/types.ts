import { Price, PaginationParams } from "@/app/lib/types/global";
import { ProductStatus, UnitOfMeasure, Discount, Weight } from "@/app/features/products/lib/types";

// POS Product DTOs
export interface PosPackagingDto {
    packagingId: string;
    packagingName: string;
    description?: string;
    barcode?: string;
    unitsPerPackage: number;
    unitOfMeasure: UnitOfMeasure;
    isDefault: boolean;
    status: ProductStatus;
    imageUrls: string[];
    dimensions?: string;
    weight?: Weight;
    color?: string;
    sellingPrice: Price;
    discount?: Discount;
    discountedPrice: Price;
    availableStock: number;
    inStock: boolean;
    hasDiscount: boolean;
    discountPercentage: number;
}

export interface PosProductDto {
    productId: string;
    productName: string;
    categoryId?: string;
    categoryName?: string;
    imageUrls: string[];
    packagings: PosPackagingDto[];
    totalPackagings: number;
    inStockPackagings: number;
    hasStock: boolean;
    totalAvailableStock: number;
}

// Cart Item
export interface CartItem {
    id: string; // Unique cart item ID (packaging + temp ID)
    productId: string;
    productName: string;
    packagingId: string;
    packagingName: string;
    barcode?: string;
    quantity: number;
    unitPrice: Price;
    discountedPrice: Price;
    discount?: Discount;
    lineTotal: Price;
    availableStock: number;
    imageUrl?: string;
}

// Sale DTO (matches backend SaleDto)
export interface SaleDto {
    saleId: string;
    saleNumber: string;
    saleDate: string;
    status: string;
    items: PosCartItemDto[];
    totalDiscount: Price;
    grandTotal: Price;
    amountPaid: Price;
    notes?: string;
    totalItems: number;
}

// Completed Sale DTO (matches backend CompletedSaleDto)
export interface CompletedSaleDto {
    saleId: string;
    saleNumber: string;
    saleDate: string;
    completedAt: string;
    items: PosCartItemDto[];
    totalDiscount: Price;
    grandTotal: Price;
    amountPaid: Price;
    totalItems: number;
}

// POS Cart Item DTO (matches backend)
export interface PosCartItemDto {
    productPackagingId: string;
    productName: string;
    packagingName: string;
    barcode?: string;
    quantity: number;
    unitPrice: Price;
    discount?: Discount;
    lineTotal: Price;
}

// API Requests (matches backend Commands)
export interface DiscountInput {
    amount: number;
    isPercentage: boolean;
}

export interface SaleItemInput {
    barcode?: string;
    productPackagingId?: string;
    quantity: number;
    discount?: DiscountInput;
}

export interface CreateSaleRequest {
    inventoryId: string;
    items: SaleItemInput[];
    notes?: string;
}

export interface CompleteSaleRequest {
    inventoryId: string;
    amountPaid: number;  // Decimal, not Price object
}

// Filters
export interface PosProductFilter extends PaginationParams {
    inventoryId: string;
    categoryId?: string;
    searchTerm?: string;
    barcode?: string;
    status?: ProductStatus;
    inStock?: boolean;
    minQuantity?: number;
}
