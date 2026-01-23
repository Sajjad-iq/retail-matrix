import * as z from 'zod';

export const registerFormSchema = z.object({
    name: z.string().min(3, 'الاسم يجب أن يكون 3 أحرف على الأقل').max(255, 'الاسم طويل جداً'),
    email: z.string().min(1, 'البريد الإلكتروني مطلوب').email('البريد الإلكتروني غير صالح'),
    phoneNumber: z.string().min(1, 'رقم الهاتف مطلوب'),
    password: z.string().min(8, 'كلمة المرور يجب أن تكون 8 أحرف على الأقل').max(100, 'كلمة المرور طويلة جداً'),
    confirmPassword: z.string(),
    address: z.string().optional(),
}).refine((data) => data.password === data.confirmPassword, {
    message: 'كلمات المرور غير متطابقة',
    path: ['confirmPassword'],
});

export type RegisterFormValues = z.infer<typeof registerFormSchema>;
