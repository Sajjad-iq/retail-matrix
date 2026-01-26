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
    DialogClose,
    DialogFooter,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { addStockBatchSchema, AddStockBatchFormValues, stockConditionOptions } from '../lib/validations';
import { useAddStockBatch } from '../hooks/useStock';
import { StockCondition } from '../lib/types';
import { currencyOptions } from '@/app/lib/constants';

interface AddBatchDialogProps {
    children?: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
    stockId: string;
    productName: string;
}

export function AddBatchDialog({
    children,
    open,
    onOpenChange,
    stockId,
    productName,
}: AddBatchDialogProps) {
    const closeRef = useRef<HTMLButtonElement>(null);
    const { mutate: addBatch, isPending } = useAddStockBatch();

    const handleSubmit = (data: AddStockBatchFormValues) => {
        addBatch(
            {
                stockId,
                data: {
                    batchNumber: data.batchNumber,
                    quantity: data.quantity,
                    expiryDate: data.expiryDate,
                    condition: data.condition ?? StockCondition.New,
                    costPrice: data.costPrice,
                },
            },
            {
                onSuccess: () => {
                    closeRef.current?.click();
                    onOpenChange?.(false);
                },
            }
        );
    };

    const defaultValues: Partial<AddStockBatchFormValues> = {
        batchNumber: '',
        quantity: 1,
        condition: StockCondition.New,
        costPrice: { amount: 0, currency: 'IQD' },
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogTrigger asChild>{children}</DialogTrigger>
            <DialogContent className="sm:max-w-[500px]" dir="rtl">
                <DialogHeader>
                    <DialogTitle>إضافة دفعة جديدة</DialogTitle>
                    <DialogDescription>
                        إضافة دفعة جديدة للمخزون: {productName}
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    schema={addStockBatchSchema}
                    onSubmit={handleSubmit}
                    defaultValues={defaultValues}
                    loading={isPending}
                    className="space-y-4"
                >
                    <div className="space-y-4">
                        <FormBuilder.Text
                            name="batchNumber"
                            label="رقم الدفعة"
                            placeholder="مثال: BATCH-001"
                            required
                        />

                        <FormBuilder.Number
                            name="quantity"
                            label="الكمية"
                            placeholder="0"
                            min={1}
                            required
                        />

                        <FormBuilder.Date
                            name="expiryDate"
                            label="تاريخ الصلاحية (اختياري)"
                            placeholder="اختر التاريخ"
                        />

                        <FormBuilder.Select
                            name="condition"
                            label="حالة المنتج"
                            options={stockConditionOptions}
                        />

                        <FormBuilder.InputWithUnit
                            name="costPrice"
                            label="سعر التكلفة (اختياري)"
                            placeholder="0"
                            options={currencyOptions}
                            valueKey="amount"
                            unitKey="currency"
                        />
                    </div>

                    <DialogFooter className="gap-2 sm:gap-0 pt-4">
                        <DialogClose ref={closeRef} className="hidden" />
                        <Button type="button" variant="outline" onClick={() => closeRef.current?.click()}>
                            إلغاء
                        </Button>
                        <FormBuilder.Submit loadingText="جاري الإضافة...">
                            إضافة الدفعة
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
