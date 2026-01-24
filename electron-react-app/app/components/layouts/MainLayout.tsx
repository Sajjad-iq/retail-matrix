import { ReactNode } from 'react';
import { SidebarProvider, SidebarTrigger } from '@/app/components/ui/sidebar';
import { AppSidebar } from './AppSidebar';

interface MainLayoutProps {
    children: ReactNode;
}

/**
 * Main Layout
 * Used for authenticated pages
 * Includes sidebar with navigation
 */
export function MainLayout({ children }: MainLayoutProps) {
    return (
        <SidebarProvider className="min-h-full h-full">
            <div className="flex h-full w-full overflow-hidden">
                <AppSidebar />
                <main className="flex-1 flex flex-col overflow-hidden">
                    {/* Header with sidebar trigger */}
                    <header className="flex-shrink-0 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
                        <div className="flex h-14 items-center gap-4 px-4">
                            <SidebarTrigger />
                            <h1 className="text-lg font-semibold">Retail System</h1>
                        </div>
                    </header>

                    {/* Main Content */}
                    <div className="flex-1 overflow-auto p-6">
                        {children}
                    </div>
                </main>
            </div>
        </SidebarProvider>
    );
}
