import * as z from 'zod';
import { InventoryType } from './types';

export const inventoryTypeOptions = [
    { label: 'مستودع', value: InventoryType.Warehouse.toString() },
    { label: 'ممر', value: InventoryType.Aisle.toString() },
    { label: 'رف كبير', value: InventoryType.Rack.toString() },
    { label: 'رف', value: InventoryType.Shelf.toString() },
    { label: 'صندوق', value: InventoryType.Bin.toString() },
    { label: 'درج', value: InventoryType.Drawer.toString() },
    { label: 'أرضية', value: InventoryType.Floor.toString() },
];

export const inventorySchema = z.object({
    name: z.string()
        .min(1, 'اسم المخزن مطلوب')
        .min(2, 'يجب أن يكون الاسم حرفين على الأقل')
        .max(100, 'يجب ألا يتجاوز الاسم 100 حرف'),

    code: z.string()
        .min(1, 'رمز المخزن مطلوب')
        .min(1, 'يجب أن يكون الرمز حرف واحد على الأقل')
        .max(50, 'يجب ألا يتجاوز الرمز 50 حرف')
        .regex(/^[A-Za-z0-9-_]+$/, 'الرمز يجب أن يحتوي على أحرف إنجليزية وأرقام وشرطات فقط'),

    type: z.coerce.number()
        .min(0, 'نوع المخزن مطلوب')
        .max(6, 'نوع المخزن غير صالح'),

    parentId: z.string()
        .optional()
        .nullable()
        .transform(val => val || undefined),

    isActive: z.boolean()
        .default(true),
});

export type InventoryFormValues = z.infer<typeof inventorySchema>;
