import { RouterProvider } from 'react-router';
import { Toaster } from '@/app/components/ui/sonner';
import { router } from './routes';
import './styles/app.css';

export default function App() {
    return (
        <>
            <RouterProvider router={router} />
            <Toaster position="top-center" richColors />
        </>
    );
}
