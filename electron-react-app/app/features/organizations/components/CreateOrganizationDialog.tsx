'use client';

import { useRef } from 'react';
import { useCreateOrganization } from '../hooks/useOrganizationActions';
import { createOrganizationSchema, type CreateOrganizationFormValues } from '../lib/validations';
import { FormBuilder } from '@/app/components/form';
import { Button } from '@/app/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
    DialogClose,
} from '@/app/components/ui/dialog';

interface CreateOrganizationDialogProps {
    children: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
}

export function CreateOrganizationDialog({ children, open, onOpenChange }: CreateOrganizationDialogProps) {
    const { mutate: createOrganization, isPending } = useCreateOrganization();
    const closeRef = useRef<HTMLButtonElement>(null);

    const handleSubmit = (values: CreateOrganizationFormValues) => {
        createOrganization(values, {
            onSuccess: () => {
                closeRef.current?.click();
                if (onOpenChange) onOpenChange(false);
            },
        });
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[425px]" dir="rtl">
                <DialogHeader className="text-right">
                    <DialogTitle>إنشاء مؤسسة جديدة</DialogTitle>
                    <DialogDescription>
                        قم بإنشاء مؤسسة جديدة لإدارة متجرك.
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    onSubmit={handleSubmit}
                    schema={createOrganizationSchema}
                    defaultValues={{
                        name: '',
                        domain: '',
                        phone: '',
                        description: '',
                        address: '',
                    }}
                    loading={isPending}
                    className="space-y-4 py-4"
                >
                    <FormBuilder.Text
                        name="name"
                        label="اسم المؤسسة"
                        placeholder="متجري"
                        required
                    />
                    <FormBuilder.Text
                        name="domain"
                        label="النطاق (Domain)"
                        placeholder="example.com"
                        required
                    />
                    <FormBuilder.Phone
                        name="phone"
                        label="رقم الهاتف"
                        placeholder="07XXXXXXXXX"
                        defaultCountry="IQ"
                        required
                    />
                    <FormBuilder.Text
                        name="address"
                        label="العنوان"
                        placeholder="المدينة، الحي"
                    />
                    <FormBuilder.Textarea
                        name="description"
                        label="الوصف (اختياري)"
                        placeholder="وصف للمؤسسة"
                    />

                    <DialogFooter className="gap-2 sm:gap-0">
                        <DialogClose ref={closeRef} className="hidden" />
                        <Button type="button" variant="outline" onClick={() => closeRef.current?.click()}>
                            إلغاء
                        </Button>
                        <FormBuilder.Submit loadingText="جاري الإنشاء...">
                            إنشاء
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
