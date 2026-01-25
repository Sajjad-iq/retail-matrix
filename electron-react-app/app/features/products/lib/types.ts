import { Barcode, Price } from "@/app/lib/types/global";

export enum ProductStatus {
    Active = 0,
    Inactive = 1,
    OutOfStock = 2,
    Discontinued = 3
}

export enum DiscountType {
    None = 0,
    Percentage = 1,
    FixedAmount = 2
}

export enum UnitOfMeasure {
    // Count/Quantity
    Piece = 0,
    Dozen = 1,
    Pair = 2,
    Set = 3,
    // Weight - Metric
    Kilogram = 4,
    Gram = 5,
    Milligram = 6,
    MetricTon = 7,
    // Weight - Imperial
    Pound = 8,
    Ounce = 9,
    // Volume - Metric
    Liter = 10,
    Milliliter = 11,
    CubicMeter = 12,
    CubicCentimeter = 13,
    // Volume - Imperial
    Gallon = 14,
    Quart = 15,
    Pint = 16,
    FluidOunce = 17,
    // Length - Metric
    Meter = 18,
    Centimeter = 19,
    Millimeter = 20,
    Kilometer = 21,
    // Length - Imperial
    Foot = 22,
    Inch = 23,
    Yard = 24,
    // Area
    SquareMeter = 25,
    SquareFoot = 26,
    SquareCentimeter = 27,
    // Packaging
    Box = 28,
    Pack = 29,
    Bag = 30,
    Bottle = 31,
    Can = 32,
    Jar = 33,
    Tube = 34,
    Roll = 35,
    Sheet = 36,
    Pallet = 37,
    Crate = 38,
    Bundle = 39,
    Carton = 40,
    Container = 41
}

export interface Discount {
    value: number;
    type: DiscountType;
}

export interface Weight {
    value: number;
    unit: UnitOfMeasure;
}

export interface ProductPackagingListDto {
    id: string;
    name: string;
    description?: string | null;
    barcode?: Barcode | null;
    unitsPerPackage: number;
    unitOfMeasure: UnitOfMeasure;
    sellingPrice: Price;
    discount: Discount;
    isDefault: boolean;
    status: ProductStatus;
    imageUrls: string[];
    dimensions?: string | null;
    weight?: Weight | null;
    color?: string | null;
    insertDate: string;
}

export interface ProductWithPackagingsDto {
    id: string;
    name: string;
    imageUrls: string[];
    status: ProductStatus;
    organizationId: string;
    categoryId?: string;
    categoryName?: string;
    insertDate: string;
    packagings: ProductPackagingListDto[];
}


