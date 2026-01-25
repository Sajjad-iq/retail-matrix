'use client';

import { useRef } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormBuilder } from '@/app/components/form';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
    DialogClose,
    DialogFooter,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { toast } from 'sonner';
import {
    createPackagingSchema,
    CreatePackagingFormValues,
    unitOfMeasureOptions,
    currencyOptions,
    weightUnitOptions,
} from '../lib/validations';
import { productService } from '../services/productService';
import { ProductPackagingListDto } from '../lib/types';

interface CreatePackagingDialogProps {
    children?: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
    productId: string;
    productName: string;
    packaging?: ProductPackagingListDto; // If provided, we are in Edit mode
}

export function CreatePackagingDialog({
    children,
    open,
    onOpenChange,
    productId,
    productName,
    packaging,
}: CreatePackagingDialogProps) {
    const queryClient = useQueryClient();
    const closeRef = useRef<HTMLButtonElement>(null);
    const isEditMode = !!packaging;

    const { mutate, isPending } = useMutation({
        mutationFn: (data: CreatePackagingFormValues) => {
            if (isEditMode && packaging) {
                return productService.updatePackaging({ ...data, id: packaging.id });
            }
            return productService.createPackaging({ ...data, productId });
        },
        onSuccess: () => {
            toast.success(isEditMode ? 'تم تحديث وحدة البيع بنجاح' : 'تم إضافة وحدة البيع بنجاح');
            queryClient.invalidateQueries({ queryKey: ['my-products'] });
            closeRef.current?.click();
            onOpenChange?.(false);
        },
        onError: (error: any) => {
            console.error(isEditMode ? 'Failed to update packaging' : 'Failed to create packaging', error);
            toast.error(isEditMode ? 'فشل تحديث وحدة البيع' : 'فشل إضافة وحدة البيع');
        },
    });

    const defaultValues: Partial<CreatePackagingFormValues> = packaging ? {
        name: packaging.name,
        sellingPrice: {
            amount: packaging.sellingPrice.amount,
            currency: packaging.sellingPrice.currency
        },
        unitOfMeasure: packaging.unitOfMeasure,
        unitsPerPackage: packaging.unitsPerPackage,
        barcode: packaging.barcode?.value || '',
        description: packaging.description || '',
        isDefault: packaging.isDefault,
        weight: packaging.weight ? { value: packaging.weight.value, unit: packaging.weight.unit } : { value: 0, unit: 4 },
        dimensions: packaging.dimensions || '',
        color: packaging.color || '',
    } : {
        name: '',
        sellingPrice: { amount: 0, currency: 'IQD' },
        unitOfMeasure: 0,
        unitsPerPackage: 1,
        isDefault: false,
        weight: { value: 0, unit: 4 },
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[600px]" dir="rtl">
                <DialogHeader>
                    <DialogTitle>{isEditMode ? 'تعديل وحدة البيع' : 'إضافة وحدة بيع جديدة'}</DialogTitle>
                    <DialogDescription>
                        {isEditMode
                            ? `تعديل وحدة البيع للمنتج: ${productName}`
                            : `إضافة وحدة بيع جديدة للمنتج: ${productName}`}
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    schema={createPackagingSchema}
                    onSubmit={mutate}
                    defaultValues={defaultValues}
                    loading={isPending}
                    className="space-y-4"
                >
                    <div className="space-y-4">
                        <FormBuilder.Text
                            name="name"
                            label="اسم وحدة البيع"
                            placeholder="مثال: علبة 1 لتر"
                            required
                        />

                        <FormBuilder.InputWithUnit
                            name="sellingPrice"
                            label="سعر البيع"
                            placeholder="0"
                            options={currencyOptions}
                            valueKey="amount"
                            unitKey="currency"
                            required
                        />

                        <FormBuilder.Select
                            name="unitOfMeasure"
                            label="وحدة القياس"
                            options={unitOfMeasureOptions}
                            required
                        />

                        <FormBuilder.Text
                            name="barcode"
                            label="الباركود (اختياري)"
                            placeholder="أدخل الباركود"
                        />

                        <FormBuilder.Number
                            name="unitsPerPackage"
                            label="عدد الوحدات في العبوة"
                            placeholder="1"
                            min={1}
                        />

                        <FormBuilder.Textarea
                            name="description"
                            label="الوصف (اختياري)"
                            placeholder="وصف وحدة البيع"
                            rows={2}
                        />

                        <FormBuilder.InputWithUnit
                            name="weight"
                            label="الوزن (اختياري)"
                            placeholder="0"
                            options={weightUnitOptions}
                            valueKey="value"
                            unitKey="unit"
                            unitPlaceholder="الوحدة"
                        />

                        <div className="grid grid-cols-2 gap-4">
                            <FormBuilder.Text
                                name="dimensions"
                                label="الأبعاد (اختياري)"
                                placeholder="مثال: 10x20x5 سم"
                            />
                            <FormBuilder.Text
                                name="color"
                                label="اللون (اختياري)"
                                placeholder="مثال: أحمر"
                            />
                        </div>

                        <FormBuilder.Checkbox
                            name="isDefault"
                            label="وحدة البيع الافتراضية"
                        />
                    </div>

                    <DialogFooter className="gap-2 sm:gap-0 pt-4">
                        <DialogClose ref={closeRef} className="hidden" />
                        <Button type="button" variant="outline" onClick={() => closeRef.current?.click()}>
                            إلغاء
                        </Button>
                        <FormBuilder.Submit loadingText={isEditMode ? "جاري التحديث..." : "جاري الإضافة..."}>
                            {isEditMode ? 'حفظ التعديلات' : 'إضافة وحدة البيع'}
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
