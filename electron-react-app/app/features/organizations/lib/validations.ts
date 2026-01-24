import * as z from 'zod';
import { DOMAIN_REGEX, normalizePhone, PHONE_REGEX } from '@/app/lib/validations/regex';

export const createOrganizationSchema = z.object({
    name: z.string()
        .min(1, 'اسم المؤسسة مطلوب')
        .min(2, 'يجب أن يكون الاسم حرفين على الأقل')
        .max(200, 'يجب ألا يتجاوز الاسم 200 حرف'),

    domain: z.string()
        .min(1, 'نطاق المؤسسة مطلوب')
        .regex(DOMAIN_REGEX, 'صيغة النطاق غير صحيحة (مثال: example.com)')
        .max(253, 'النطاق يجب ألا يتجاوز 253 حرف'),

    phone: z.string()
        .min(1, 'رقم الهاتف مطلوب')
        .transform(val => normalizePhone(val))
        .refine(val => PHONE_REGEX.test(val), 'صيغة رقم الهاتف غير صحيحة'),

    description: z.string().optional(),
    address: z.string().optional(),
});

export type CreateOrganizationFormValues = z.infer<typeof createOrganizationSchema>;
