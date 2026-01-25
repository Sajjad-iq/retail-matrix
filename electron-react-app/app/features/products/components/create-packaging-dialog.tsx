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

interface CreatePackagingDialogProps {
    children?: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
    productId: string;
    productName: string;
}

export function CreatePackagingDialog({
    children,
    open,
    onOpenChange,
    productId,
    productName,
}: CreatePackagingDialogProps) {
    const queryClient = useQueryClient();
    const closeRef = useRef<HTMLButtonElement>(null);

    const { mutate, isPending } = useMutation({
        mutationFn: (data: CreatePackagingFormValues) =>
            productService.createPackaging({ ...data, productId }),
        onSuccess: () => {
            toast.success('تم إضافة وحدة البيع بنجاح');
            queryClient.invalidateQueries({ queryKey: ['my-products'] });
            closeRef.current?.click();
            onOpenChange?.(false);
        },
        onError: (error: any) => {
            console.error('Failed to create packaging', error);
        },
    });

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[600px]" dir="rtl">
                <DialogHeader>
                    <DialogTitle>إضافة وحدة بيع جديدة</DialogTitle>
                    <DialogDescription>
                        إضافة وحدة بيع جديدة للمنتج: {productName}
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    schema={createPackagingSchema}
                    onSubmit={mutate}
                    defaultValues={{
                        name: '',
                        sellingPrice: { amount: 0, currency: 'IQD' },
                        unitOfMeasure: 0,
                        unitsPerPackage: 1,
                        isDefault: false,
                        weight: { value: 0, unit: 4 }, // Default to Kilogram (4)
                    }}
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
                        <FormBuilder.Submit loadingText="جاري الإضافة...">
                            إضافة وحدة البيع
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
