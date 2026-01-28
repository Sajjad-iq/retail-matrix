import { z } from 'zod';

export const currencySchema = z.object({
    code: z.string().min(2, 'الرمز مطلوب حرفين على الأقل').max(5, 'الرمز طويل جداً'),
    name: z.string().min(2, 'الاسم مطلوب'),
    symbol: z.string().min(1, 'الرمز مطلوب'),
    exchangeRate: z.coerce.number().min(0.0001, 'سعر الصرف يجب أن يكون أكبر من 0'),
    isBaseCurrency: z.boolean(),
    status: z.coerce.number(),
});

export const updateCurrencySchema = currencySchema.omit({ code: true, isBaseCurrency: true });

export type CurrencyFormValues = z.infer<typeof currencySchema>;
export type UpdateCurrencyFormValues = z.infer<typeof updateCurrencySchema>;
