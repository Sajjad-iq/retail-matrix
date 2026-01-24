'use client';

import { useRef } from 'react';
import { useCreateProduct } from '../hooks/useProductActions';
import {
    createProductSchema,
    type CreateProductFormValues,
    type CreateProductRequest,
    unitOfMeasureOptions,
    currencyOptions,
    weightUnitOptions,
    UnitOfMeasure,
} from '../lib/validations';
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

interface CreateProductDialogProps {
    children: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
}

export function CreateProductDialog({ children, open, onOpenChange }: CreateProductDialogProps) {
    const { mutate: createProduct, isPending } = useCreateProduct();
    const closeRef = useRef<HTMLButtonElement>(null);

    const handleSubmit = (values: CreateProductFormValues) => {
        const request: CreateProductRequest = {
            productName: values.productName,
            categoryId: values.categoryId || undefined,
            name: values.name,
            sellingPrice: {
                amount: values.sellingPrice.amount,
                currency: values.sellingPrice.currency,
            },
            unitOfMeasure: values.unitOfMeasure,
            barcode: values.barcode || undefined,
            description: values.description || undefined,
            unitsPerPackage: values.unitsPerPackage,
            isDefault: values.isDefault,
            dimensions: values.dimensions || undefined,
            weight: values.weight?.value ? {
                value: values.weight.value,
                unit: values.weight.unit,
            } : undefined,
            color: values.color || undefined,
        };

        createProduct(request, {
            onSuccess: () => {
                closeRef.current?.click();
                if (onOpenChange) onOpenChange(false);
            },
        });
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[500px]" dir="rtl">
                <DialogHeader className="text-right">
                    <DialogTitle>إضافة منتج جديد</DialogTitle>
                    <DialogDescription>
                        قم بإضافة منتج جديد مع وحدة البيع الأساسية.
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    onSubmit={handleSubmit}
                    schema={createProductSchema}
                    defaultValues={{
                        productName: '',
                        categoryId: '',
                        name: '',
                        sellingPrice: { amount: 0, currency: 'IQD' },
                        unitOfMeasure: UnitOfMeasure.Piece,
                        barcode: '',
                        description: '',
                        unitsPerPackage: 1,
                        isDefault: true,
                        weight: { value: 0, unit: UnitOfMeasure.Kilogram },
                    }}
                    loading={isPending}
                    className="space-y-4 py-4 max-h-[60vh] overflow-y-auto"
                >
                    <div className="space-y-4">
                        <p className="text-sm font-medium text-muted-foreground">معلومات المنتج</p>
                        <FormBuilder.Text
                            name="productName"
                            label="اسم المنتج"
                            placeholder="مثال: حليب طازج"
                            required
                        />
                    </div>

                    <div className="border-t pt-4 space-y-4">
                        <p className="text-sm font-medium text-muted-foreground">معلومات وحدة البيع</p>

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
                            placeholder="وصف المنتج أو وحدة البيع"
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
                        <FormBuilder.Submit loadingText="جاري الإضافة...">
                            إضافة المنتج
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
