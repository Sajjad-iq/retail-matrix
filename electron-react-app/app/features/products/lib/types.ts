import { Barcode, Price } from "@/app/lib/types/global";

export enum ProductStatus {
    Active = 0,
    Inactive = 1,
    OutOfStock = 2,
    Discontinued = 3
}

export interface ProductPackagingListDto {
    id: string;
    name: string;
    barcode?: Barcode;
    sellingPrice: Price;
    status: ProductStatus;
}

export interface ProductWithPackagingsDto {
    id: string;
    name: string;
    imageUrls: string[];
    status: ProductStatus;
    organizationId: string;
    categoryId?: string;
    insertDate: string;
    packagings: ProductPackagingListDto[];
}


