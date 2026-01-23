import { FormBuilder } from '@/app/components/form';
import { Button } from '@/app/components/ui/button';
import { useNavigate } from 'react-router-dom';
import { AuthCard } from '../components/AuthCard';
import { useLogin } from '../hooks/useAuthActions';
import { loginFormSchema, type LoginFormValues } from '../lib/loginFormConfig';

export default function LoginPage() {
    const navigate = useNavigate();
    const loginMutation = useLogin();

    const handleSubmit = async (values: LoginFormValues) => {
        loginMutation.mutate(
            {
                email: values.email,
                password: values.password,
            },
            {
                onSuccess: (result) => {
                    if (result.success) {
                        navigate('/');
                    }
                },
            }
        );
    };

    return (
        <AuthCard
            title="تسجيل الدخول"
            subtitle="أدخل بياناتك للوصول إلى حسابك"
            footer={
                <>
                    <span className="text-muted-foreground">ليس لديك حساب؟ </span>
                    <Button
                        type="button"
                        variant="link"
                        size="sm"
                        onClick={() => router.push('/register')}
                        className="px-0 h-auto"
                    >
                        إنشاء حساب جديد
                    </Button>
                </>
            }
        >
            <FormBuilder
                onSubmit={handleSubmit}
                schema={loginFormSchema}
                defaultValues={{ email: '', password: '' }}
                loading={loginMutation.isPending}
            >
                <FormBuilder.Email
                    name="email"
                    label="البريد الإلكتروني"
                    placeholder="أدخل البريد الإلكتروني"
                    required
                />
                <FormBuilder.Password
                    name="password"
                    label="كلمة المرور"
                    placeholder="أدخل كلمة المرور"
                    required
                />
                <FormBuilder.Submit className="w-full" loadingText="جاري تسجيل الدخول...">
                    تسجيل الدخول
                </FormBuilder.Submit>
            </FormBuilder>
        </AuthCard>
    );
}
