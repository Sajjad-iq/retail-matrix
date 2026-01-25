'use client';

import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from '@/app/components/ui/alert-dialog';

interface ConfirmDialogProps {
    children?: React.ReactNode;
    title: string;
    description: string;
    confirmText?: string;
    cancelText?: string;
    onConfirm: () => void;
    variant?: 'default' | 'destructive';
}

export function ConfirmDialog({
    children,
    title,
    description,
    confirmText = 'تأكيد',
    cancelText = 'إلغاء',
    onConfirm,
    variant = 'default',
}: ConfirmDialogProps) {
    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>{children}</AlertDialogTrigger>
            <AlertDialogContent dir="rtl">
                <AlertDialogHeader className="text-right">
                    <AlertDialogTitle>{title}</AlertDialogTitle>
                    <AlertDialogDescription>{description}</AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter className="gap-2 sm:gap-0">
                    <AlertDialogCancel>{cancelText}</AlertDialogCancel>
                    <AlertDialogAction
                        onClick={(e) => {
                            e.preventDefault();
                            onConfirm();
                        }}
                        className={variant === 'destructive' ? 'bg-destructive hover:bg-destructive/90' : ''}
                    >
                        {confirmText}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}

// Ensure AlertDialog components are available in @/app/components/ui/alert-dialog
// If not, they need to be created or imported from shadcn/ui
