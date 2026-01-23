import { FormBuilder } from '@/app/components/form';
import { Button } from '@/app/components/ui/button';
import { useRouter } from 'next/navigation';
import { AuthCard } from '../components/AuthCard';
import { useRegister } from '../hooks/useAuthActions';
import { registerFormSchema, type RegisterFormValues } from '../lib/registerFormConfig';

export default function RegisterPage() {
    const router = useRouter();
    const registerMutation = useRegister();

    const handleSubmit = async (values: RegisterFormValues) => {
        registerMutation.mutate(
            {
                name: values.name,
                email: values.email,
                phoneNumber: values.phoneNumber,
                password: values.password,
                address: values.address,
            },
            {
                onSuccess: (result) => {
                    if (result.success) {
                        router.push('/login');
                    }
                },
            }
        );
    };

    return (
        <AuthCard
            title="إنشاء حساب جديد"
            subtitle="أدخل بياناتك لإنشاء حساب جديد"
            footer={
                <>
                    <span className="text-muted-foreground">لديك حساب بالفعل؟ </span>
                    <Button
                        type="button"
                        variant="link"
                        size="sm"
                        onClick={() => router.push('/login')}
                        className="px-0 h-auto"
                    >
                        تسجيل الدخول
                    </Button>
                </>
            }
        >
            <FormBuilder
                onSubmit={handleSubmit}
                schema={registerFormSchema}
                defaultValues={{
                    name: '',
                    email: '',
                    phoneNumber: '',
                    password: '',
                    confirmPassword: '',
                    address: '',
                }}
                loading={registerMutation.isPending}
            >
                <FormBuilder.Text
                    name="name"
                    label="الاسم الكامل"
                    placeholder="أدخل اسمك الكامل"
                    required
                />
                <FormBuilder.Email
                    name="email"
                    label="البريد الإلكتروني"
                    placeholder="أدخل البريد الإلكتروني"
                    required
                />
                <FormBuilder.Phone
                    name="phoneNumber"
                    label="رقم الهاتف"
                    placeholder="أدخل رقم الهاتف"
                    required
                />
                <FormBuilder.Password
                    name="password"
                    label="كلمة المرور"
                    placeholder="أدخل كلمة المرور"
                    required
                />
                <FormBuilder.Password
                    name="confirmPassword"
                    label="تأكيد كلمة المرور"
                    placeholder="أعد إدخال كلمة المرور"
                    required
                />
                <FormBuilder.Text
                    name="address"
                    label="العنوان"
                    placeholder="أدخل العنوان (اختياري)"
                />
                <FormBuilder.Submit className="w-full" loadingText="جاري إنشاء الحساب...">
                    إنشاء حساب
                </FormBuilder.Submit>
            </FormBuilder>
        </AuthCard>
    );
}
