import { ReactNode } from 'react';
import { Card, CardContent } from '@/app/components/ui/card';

interface AuthCardProps {
    title: string;
    subtitle: string;
    children: ReactNode;
    footer?: ReactNode;
}

export function AuthCard({ title, subtitle, children, footer }: AuthCardProps) {
    return (
        <div className="min-h-screen w-full flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
            <div className="w-full max-w-md space-y-8">
                {/* Logo/Brand */}
                <div className="text-center">
                    <h1 className="text-4xl font-bold text-foreground mb-2">ريتيل ماتركس</h1>
                    <p className="text-muted-foreground">نظام إدارة المتاجر</p>
                </div>

                {/* Card Container */}
                <Card className="shadow-lg">
                    <CardContent className="p-8">
                        <div className="space-y-6">
                            {/* Header */}
                            <div className="text-center">
                                <h2 className="text-2xl font-bold tracking-tight">{title}</h2>
                                <p className="text-sm text-muted-foreground mt-2">{subtitle}</p>
                            </div>

                            {/* Content */}
                            {children}

                            {/* Footer */}
                            {footer && <div className="text-center text-sm">{footer}</div>}
                        </div>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
}
