import * as z from 'zod';

export const loginFormSchema = z.object({
    email: z.string().min(1, 'البريد الإلكتروني مطلوب').email('البريد الإلكتروني غير صالح'),
    password: z.string().min(8, 'كلمة المرور يجب أن تكون 8 أحرف على الأقل').max(100, 'كلمة المرور طويلة جداً'),
});

export type LoginFormValues = z.infer<typeof loginFormSchema>;
