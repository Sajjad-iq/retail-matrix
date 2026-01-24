import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuthStore } from '@/app/stores/auth';

export const ProtectedRoute = () => {
    const { isAuthenticated, accessToken } = useAuthStore();
    const location = useLocation();

    // Check if authenticated or if there's a token (optimistic check before validation finishes)
    // If there is no token at all, we can definitely redirect.
    if (!isAuthenticated && !accessToken) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <Outlet />;
};
