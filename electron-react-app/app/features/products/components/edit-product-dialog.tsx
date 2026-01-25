'use client';

import { useRef } from 'react';
import { FormBuilder } from '@/app/components/form';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
    DialogFooter,
    DialogClose,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { useUpdateProduct } from '../hooks/useProductActions';
import { ProductWithPackagingsDto } from '../lib/types';

interface EditProductDialogProps {
    children?: React.ReactNode;
    product: ProductWithPackagingsDto;
}

export function EditProductDialog({ children, product }: EditProductDialogProps) {
    const { mutate: updateProduct, isPending } = useUpdateProduct();
    const closeRef = useRef<HTMLButtonElement>(null);

    // We only edit basic product info here for now
    // NOTE: This assumes the backend update endpoint accepts partial updates or we send existing data back
    // The current create schema might be too strict (e.g. requires packagings), so we might need a specific edit schema later.
    // For this task, I'm creating a simplified edit form.

    const handleSubmit = (values: any) => {
        updateProduct({
            ...values,
            id: product.id,
            organizationId: product.organizationId,
        }, {
            onSuccess: () => {
                closeRef.current?.click();
            }
        });
    };

    return (
        <Dialog>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[500px]" dir="rtl">
                <DialogHeader className="text-right">
                    <DialogTitle>تعديل المنتج</DialogTitle>
                    <DialogDescription>
                        تعديل بيانات المنتج الأساسية.
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    onSubmit={handleSubmit}
                    // Using partial schema or just fields we want to edit.
                    // If we pass the full createSchema it might require packaging fields which we might not want to re-submit here or handle differently
                    defaultValues={{
                        productName: product.name,
                        categoryId: product.categoryId,
                    }}
                    loading={isPending}
                    className="space-y-4"
                >
                    <FormBuilder.Text
                        name="productName"
                        label="اسم المنتج"
                        required
                    />

                    {/* Add more fields here as needed e.g. Category */}

                    <DialogFooter className="gap-2 sm:gap-0 pt-4">
                        <DialogClose ref={closeRef} className="hidden" />
                        <Button type="button" variant="outline" onClick={() => closeRef.current?.click()}>
                            إلغاء
                        </Button>
                        <FormBuilder.Submit loadingText="جاري التحديث...">
                            حفظ التعديلات
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
