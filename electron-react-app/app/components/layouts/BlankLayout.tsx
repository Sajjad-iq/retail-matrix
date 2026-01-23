import { ReactNode } from 'react';

interface BlankLayoutProps {
    children: ReactNode;
}

/**
 * Blank Layout
 * Used for authentication pages (login, register)
 * No navigation, just centered content
 */
export function BlankLayout({ children }: BlankLayoutProps) {
    return (
        <div className="min-h-screen w-full bg-background">
            {children}
        </div>
    );
}
