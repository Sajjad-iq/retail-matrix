import { createHashRouter, Outlet } from 'react-router';
import { BlankLayout } from '@/app/components/layouts/BlankLayout';
import { MainLayout } from '@/app/components/layouts/MainLayout';
import LoginPage from '@/app/features/auth/pages/login';
import RegisterPage from '@/app/features/auth/pages/register';

// Home page placeholder
function HomePage() {
    return (
        <div className="flex flex-col items-center justify-center h-full">
            <h1 className="text-3xl font-bold mb-4">مرحباً بك في ريتيل ماتركس</h1>
            <p className="text-muted-foreground">نظام إدارة المتاجر</p>
        </div>
    );
}

// Auth layout wrapper
function AuthLayout() {
    return (
        <BlankLayout>
            <Outlet />
        </BlankLayout>
    );
}

// Main app layout wrapper
function AppLayout() {
    return (
        <MainLayout>
            <Outlet />
        </MainLayout>
    );
}

export const router = createHashRouter([
    {
        // Auth routes (no sidebar)
        element: <AuthLayout />,
        children: [
            {
                path: '/login',
                element: <LoginPage />,
            },
            {
                path: '/register',
                element: <RegisterPage />,
            },
        ],
    },
    {
        // Protected routes (with sidebar)
        element: <AppLayout />,
        children: [
            {
                path: '/',
                element: <HomePage />,
            },
        ],
    },
]);
