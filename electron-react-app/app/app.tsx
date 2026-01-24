import { RouterProvider } from 'react-router';
import { Toaster } from '@/app/components/ui/sonner';
import { useAuthInit } from '@/app/features/auth/hooks/useAuthActions';
import { router } from './routes';
import './styles/app.css';

export default function App() {
    // Initialize auth state on mount - validates token and fetches user from /me
    useAuthInit();

    return (
        <>
            <RouterProvider router={router} />
            <Toaster position="bottom-left" richColors />
        </>
    );
}
