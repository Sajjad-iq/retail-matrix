'use client';

import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router';
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
import {
    createStockSchema,
    CreateStockFormValues,
    stockConditionOptions,
} from '@/app/features/stock/lib/validations';
import { useCreateStock } from '@/app/features/stock/hooks/useStock';
import { useMyInventories } from '@/app/features/locations/hooks/useInventoryActions';
import { currencyOptions } from '@/app/features/products/lib/validations';

interface CreateStockDialogProps {
    children?: React.ReactNode;
}

export function CreateStockDialog({ children }: CreateStockDialogProps) {
    const [open, setOpen] = useState(false);
    const [searchParams] = useSearchParams();

    // Check for pre-filled data from URL
    const prefilledPackagingId = searchParams.get('packagingId');
    const prefilledPackagingName = searchParams.get('packagingName');

    // Auto-open if redirected with intent
    useEffect(() => {
        if (prefilledPackagingId) {
            setOpen(true);
        }
    }, [prefilledPackagingId]);

    const { mutate: createStock, isPending } = useCreateStock();

    // Fetch inventories for the select
    const { data: inventoriesData } = useMyInventories({ pageNumber: 1, pageSize: 100 });
    const inventoryItems = inventoriesData?.items || [];

    const inventoryOptions = inventoryItems.map((inv: any) => ({
        label: inv.name || 'مخزن بدون اسم',
        value: inv.id
    })) ?? [];

    const onSubmit = (values: CreateStockFormValues) => {
        createStock(values, {
            onSuccess: () => {
                setOpen(false);
            }
        });
    };

    // Form setup can remain, but we replace the Select with a hidden input or just handle it via defaultValues
    // We already set defaultValues.productPackagingId = prefilledPackagingId
    // We can display the Packaging Name as read-only info.

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[600px]" dir="rtl">
                <DialogHeader>
                    <DialogTitle>إضافة مخزون جديد</DialogTitle>
                    <DialogDescription>
                        إضافة مخزون للمنتج: <span className="font-semibold text-primary">{prefilledPackagingName}</span>
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    schema={createStockSchema}
                    onSubmit={onSubmit}
                    defaultValues={{
                        productPackagingId: prefilledPackagingId || '',
                        initialQuantity: 1,
                        initialCondition: 0, // New
                        initialCostPrice: { amount: 0, currency: 'IQD' }
                    }}
                    loading={isPending}
                    className="space-y-4"
                >
                    <div className="space-y-4">
                        {/* Hidden field for Packaging ID is managed by react-hook-form via defaultValues */}
                        {/* We don't render an input for it, or we render a hidden one if needed by FormBuilder structure? 
                            FormBuilder usually expects inputs for schema fields. 
                            If we omit it, we must ensure it's submitted. 
                            Let's use a hidden input using a simple custom component or just trust defaultValues if valid.
                            Better to include a hidden input or read-only text to be safe and clear.
                        */}
                        <div className="hidden">
                            <FormBuilder.Text name="productPackagingId" label="" className="hidden" />
                        </div>

                        {/* Inventory Select - ideally fetched from API */}
                        <FormBuilder.Select
                            name="inventoryId"
                            label="اختر المخزن"
                            placeholder="اختر المخزن..."
                            options={inventoryOptions}
                            required
                        />

                        <div className="border-t pt-4 mt-4">
                            <h4 className="text-sm font-medium mb-4">تفاصيل الدفعة الأولية (اختياري)</h4>

                            <div className="grid grid-cols-2 gap-4">
                                <FormBuilder.Text
                                    name="initialBatchNumber"
                                    label="رقم الدفعة"
                                    placeholder="مثال: BAT-001"
                                />

                                <FormBuilder.Number
                                    name="initialQuantity"
                                    label="الكمية"
                                    min={0}
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <FormBuilder.Date
                                    name="initialExpiryDate"
                                    label="تاريخ الصلاحية"
                                />

                                <FormBuilder.Select
                                    name="initialCondition"
                                    label="حالة المخزون"
                                    options={stockConditionOptions}
                                />
                            </div>

                            <FormBuilder.InputWithUnit
                                name="initialCostPrice"
                                label="سعر التكلفة"
                                options={currencyOptions}
                                valueKey="amount"
                                unitKey="currency"
                            />
                        </div>
                    </div>

                    <DialogFooter className="gap-2 sm:gap-0 pt-4">
                        <DialogClose asChild>
                            <Button variant="outline" type="button">إلغاء</Button>
                        </DialogClose>
                        <FormBuilder.Submit loadingText="جاري الإضافة...">
                            إضافة المخزون
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
