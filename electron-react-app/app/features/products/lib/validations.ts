import * as z from 'zod';

export enum UnitOfMeasure {
    Piece = 0,
    Dozen = 1,
    Pair = 2,
    Set = 3,
    Kilogram = 4,
    Gram = 5,
    Milligram = 6,
    MetricTon = 7,
    Pound = 8,
    Ounce = 9,
    Liter = 10,
    Milliliter = 11,
    CubicMeter = 12,
    CubicCentimeter = 13,
    Gallon = 14,
    Quart = 15,
    Pint = 16,
    FluidOunce = 17,
    Meter = 18,
    Centimeter = 19,
    Millimeter = 20,
    Kilometer = 21,
    Foot = 22,
    Inch = 23,
    Yard = 24,
    SquareMeter = 25,
    SquareFoot = 26,
    SquareCentimeter = 27,
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
    Container = 41,
}

export const unitOfMeasureOptions = [
    { label: 'قطعة', value: UnitOfMeasure.Piece.toString() },
    { label: 'دزينة (12 قطعة)', value: UnitOfMeasure.Dozen.toString() },
    { label: 'زوج', value: UnitOfMeasure.Pair.toString() },
    { label: 'طقم', value: UnitOfMeasure.Set.toString() },
    { label: 'كيلوغرام', value: UnitOfMeasure.Kilogram.toString() },
    { label: 'غرام', value: UnitOfMeasure.Gram.toString() },
    { label: 'لتر', value: UnitOfMeasure.Liter.toString() },
    { label: 'ملليلتر', value: UnitOfMeasure.Milliliter.toString() },
    { label: 'متر', value: UnitOfMeasure.Meter.toString() },
    { label: 'سنتيمتر', value: UnitOfMeasure.Centimeter.toString() },
    { label: 'علبة', value: UnitOfMeasure.Box.toString() },
    { label: 'باكيت', value: UnitOfMeasure.Pack.toString() },
    { label: 'كيس', value: UnitOfMeasure.Bag.toString() },
    { label: 'زجاجة', value: UnitOfMeasure.Bottle.toString() },
    { label: 'علبة معدنية', value: UnitOfMeasure.Can.toString() },
    { label: 'كرتون', value: UnitOfMeasure.Carton.toString() },
];

export const weightUnitOptions = [
    { label: 'كيلوغرام', value: UnitOfMeasure.Kilogram.toString() },
    { label: 'غرام', value: UnitOfMeasure.Gram.toString() },
    { label: 'ملليغرام', value: UnitOfMeasure.Milligram.toString() },
    { label: 'طن', value: UnitOfMeasure.MetricTon.toString() },
    { label: 'باوند', value: UnitOfMeasure.Pound.toString() },
    { label: 'أونصة', value: UnitOfMeasure.Ounce.toString() },
];

export const createProductSchema = z.object({
    // Product-level properties
    productName: z.string()
        .min(1, 'اسم المنتج مطلوب')
        .min(2, 'يجب أن يكون الاسم حرفين على الأقل')
        .max(200, 'يجب ألا يتجاوز الاسم 200 حرف'),

    categoryId: z.string().optional().or(z.literal('')),

    // Packaging-level properties
    name: z.string()
        .min(1, 'اسم وحدة البيع مطلوب')
        .min(2, 'يجب أن يكون الاسم حرفين على الأقل')
        .max(200, 'يجب ألا يتجاوز الاسم 200 حرف'),

    sellingPrice: z.object({
        amount: z.coerce.number()
            .min(0, 'السعر يجب أن يكون أكبر من أو يساوي صفر')
            .max(999999999, 'السعر كبير جداً'),
        currency: z.string().default('IQD'),
    }),

    unitOfMeasure: z.coerce.number()
        .min(0, 'وحدة القياس مطلوبة')
        .max(41, 'وحدة القياس غير صالحة'),

    barcode: z.string()
        .max(50, 'الباركود يجب ألا يتجاوز 50 حرف')
        .optional()
        .or(z.literal('')),

    description: z.string()
        .max(1000, 'الوصف يجب ألا يتجاوز 1000 حرف')
        .optional()
        .or(z.literal('')),

    unitsPerPackage: z.coerce.number()
        .min(1, 'عدد الوحدات يجب أن يكون 1 على الأقل')
        .default(1),

    isDefault: z.boolean().default(true),

    // Optional fields
    dimensions: z.string()
        .max(100, 'الأبعاد يجب ألا تتجاوز 100 حرف')
        .optional()
        .or(z.literal('')),

    weight: z.object({
        value: z.coerce.number().min(0, 'الوزن يجب أن يكون أكبر من أو يساوي صفر'),
        unit: z.coerce.number(),
    }).optional(),

    color: z.string()
        .max(50, 'اللون يجب ألا يتجاوز 50 حرف')
        .optional()
        .or(z.literal('')),
});

export const createPackagingSchema = z.object({
    productId: z.string().optional(), // Will be passed from parent

    name: z.string()
        .min(1, 'اسم وحدة البيع مطلوب')
        .min(2, 'يجب أن يكون الاسم حرفين على الأقل')
        .max(200, 'يجب ألا يتجاوز الاسم 200 حرف'),

    sellingPrice: z.object({
        amount: z.coerce.number()
            .min(0, 'السعر يجب أن يكون أكبر من أو يساوي صفر')
            .max(999999999, 'السعر كبير جداً'),
        currency: z.string().default('IQD'),
    }),

    unitOfMeasure: z.coerce.number()
        .min(0, 'وحدة القياس مطلوبة')
        .max(41, 'وحدة القياس غير صالحة'),

    barcode: z.string()
        .max(50, 'الباركود يجب ألا يتجاوز 50 حرف')
        .optional()
        .or(z.literal('')),

    description: z.string()
        .max(1000, 'الوصف يجب ألا يتجاوز 1000 حرف')
        .optional()
        .or(z.literal('')),

    unitsPerPackage: z.coerce.number()
        .min(1, 'عدد الوحدات يجب أن يكون 1 على الأقل')
        .default(1),

    isDefault: z.boolean().default(false),

    imageUrls: z.array(z.string()).optional(),

    dimensions: z.string()
        .max(100, 'الأبعاد يجب ألا تتجاوز 100 حرف')
        .optional()
        .or(z.literal('')),

    weight: z.object({
        value: z.coerce.number().min(0, 'الوزن يجب أن يكون أكبر من أو يساوي صفر'),
        unit: z.coerce.number(),
    }).optional(),

    color: z.string()
        .max(50, 'اللون يجب ألا يتجاوز 50 حرف')
        .optional()
        .or(z.literal('')),
});

export type CreateProductFormValues = z.infer<typeof createProductSchema>;
export type CreatePackagingFormValues = z.infer<typeof createPackagingSchema>;

export interface CreateProductRequest {
    // Product-level
    productId?: string;
    categoryId?: string;
    productName: string;
    productImageUrls?: string[];

    // Packaging-level
    name: string;
    sellingPrice: {
        amount: number;
        currency: string;
    };
    unitOfMeasure: number;
    barcode?: string;
    description?: string;
    unitsPerPackage: number;
    isDefault: boolean;
    imageUrls?: string[];
    dimensions?: string;
    weight?: {
        value: number;
        unit: number;
    };
    color?: string;
}
