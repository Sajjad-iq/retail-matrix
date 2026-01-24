import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuthStore } from '@/app/stores/auth';
import { useOrganizationStore } from '@/app/stores/organization';

export const ProtectedRoute = () => {
    const { isAuthenticated, accessToken } = useAuthStore();
    const { selectedOrganizationId } = useOrganizationStore();
    const location = useLocation();

    // Check if authenticated or if there's a token (optimistic check before validation finishes)
    // If there is no token at all, we can definitely redirect.
    if (!isAuthenticated && !accessToken) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    // Organization check logic
    if (isAuthenticated) {
        // If no organization selected and not already on setup page, redirect to setup
        if (!selectedOrganizationId && location.pathname !== '/setup-organization') {
            return <Navigate to="/setup-organization" replace />;
        }

        // If organization selected but user tries to go to setup page, redirect to home
        if (selectedOrganizationId && location.pathname === '/setup-organization') {
            return <Navigate to="/" replace />;
        }
    }

    return <Outlet />;
};
