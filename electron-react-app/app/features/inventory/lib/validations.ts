import { z } from 'zod';
import { StockCondition } from './types';

export const createStockSchema = z.object({
    productPackagingId: z.string().min(1, 'يرجى اختيار العبوة'),
    inventoryId: z.string().min(1, 'يرجى اختيار المخزن'),
    initialBatchNumber: z.string().optional(),
    initialQuantity: z.number().min(0, 'الكمية يجب أن لا تكون سالبة').optional(),
    initialExpiryDate: z.string().optional(), // Date string from input
    initialCondition: z.coerce.number().pipe(z.nativeEnum(StockCondition)).optional(),
    initialCostPrice: z.object({
        amount: z.number().min(0, 'السعر يجب أن لا يكون سالب'),
        currency: z.string().min(1, 'العملة مطلوبة')
    }).optional()
});

export const addStockBatchSchema = z.object({
    batchNumber: z.string().min(1, 'رقم الدفعة مطلوب'),
    quantity: z.number().min(1, 'الكمية يجب أن تكون أكبر من صفر'),
    expiryDate: z.string().optional(),
    condition: z.coerce.number().pipe(z.nativeEnum(StockCondition)).optional(),
    costPrice: z.object({
        amount: z.number().min(0, 'السعر يجب أن لا يكون سالب'),
        currency: z.string().min(1, 'العملة مطلوبة')
    }).optional()
});

export type CreateStockFormValues = z.infer<typeof createStockSchema>;
export type AddStockBatchFormValues = z.infer<typeof addStockBatchSchema>;

export const stockConditionOptions = [
    { label: 'جديد', value: StockCondition.New.toString() },
    { label: 'تالف', value: StockCondition.Damaged.toString() },
    { label: 'مجدد', value: StockCondition.Refurbished.toString() },
    { label: 'مستعمل', value: StockCondition.Used.toString() },
];
