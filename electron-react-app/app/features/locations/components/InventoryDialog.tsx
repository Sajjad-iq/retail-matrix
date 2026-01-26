'use client';

import { useEffect, useState } from 'react';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/app/components/ui/dialog';
import { Button } from '@/app/components/ui/button';
import { FormBuilder } from '@/app/components/form';
import { inventorySchema, inventoryTypeOptions, type InventoryFormValues } from '../lib/validations';
import { useCreateInventory, useUpdateInventory, useMyInventories, useInventoryById } from '../hooks/useInventoryActions';
import { InventoryType } from '../lib/types';
import { Spinner } from '@/app/components/common/Spinner';

interface InventoryDialogProps {
    inventoryId?: string;
    trigger?: React.ReactNode;
    open?: boolean;
    onOpenChange?: (open: boolean) => void;
}

export function InventoryDialog({ inventoryId, trigger, open: controlledOpen, onOpenChange }: InventoryDialogProps) {
    const [internalOpen, setInternalOpen] = useState(false);
    const isControlled = controlledOpen !== undefined;
    const open = isControlled ? controlledOpen : internalOpen;
    const setOpen = isControlled ? (onOpenChange || (() => {})) : setInternalOpen;

    const isEditMode = !!inventoryId;
    
    // Fetch inventory data when in edit mode
    const { data: inventory, isLoading: isLoadingInventory } = useInventoryById(inventoryId || '', open && isEditMode);
    
    const { mutate: createInventory, isPending: isCreating } = useCreateInventory();
    const { mutate: updateInventory, isPending: isUpdating } = useUpdateInventory();

    // Fetch parent inventories for selection (warehouses and aisles)
    const { data: inventoriesData } = useMyInventories({ pageNumber: 1, pageSize: 100 });

    const isPending = isCreating || isUpdating;

    const handleSubmit = (values: InventoryFormValues) => {
        if (isEditMode && inventoryId) {
            updateInventory(
                {
                    id: inventoryId,
                    name: values.name,
                    code: values.code,
                    type: values.type,
                    parentId: values.parentId || undefined,
                    isActive: values.isActive,
                },
                {
                    onSuccess: () => {
                        setOpen(false);
                    },
                }
            );
        } else {
            createInventory(
                {
                    name: values.name,
                    code: values.code,
                    type: values.type,
                    parentId: values.parentId || undefined,
                },
                {
                    onSuccess: () => {
                        setOpen(false);
                    },
                }
            );
        }
    };

    const defaultValues: InventoryFormValues = {
        name: inventory?.name || '',
        code: inventory?.code || '',
        type: inventory?.type ?? InventoryType.Warehouse,
        parentId: inventory?.parentId || undefined,
        isActive: inventory?.isActive ?? true,
    };

    // Show loading state while fetching inventory data
    if (isEditMode && isLoadingInventory) {
        return (
            <Dialog open={open} onOpenChange={setOpen}>
                {trigger && <DialogTrigger asChild>{trigger}</DialogTrigger>}
                <DialogContent dir="rtl" className="sm:max-w-[500px]">
                    <div className="flex items-center justify-center py-8">
                        <Spinner />
                    </div>
                </DialogContent>
            </Dialog>
        );
    }

    // Filter parent inventories (only Warehouse and Aisle can be parents)
    const parentOptions = (inventoriesData?.items || [])
        .filter((inv) => inv.type === InventoryType.Warehouse || inv.type === InventoryType.Aisle)
        .map((inv) => ({
            label: `${inv.name} (${inv.code})`,
            value: inv.id,
        }));

    return (
        <Dialog open={open} onOpenChange={setOpen}>
            {trigger && <DialogTrigger asChild>{trigger}</DialogTrigger>}
            <DialogContent dir="rtl" className="sm:max-w-[500px]">
                <DialogHeader className="text-right">
                    <DialogTitle>
                        {isEditMode ? 'تعديل المخزن' : 'إضافة مخزن جديد'}
                    </DialogTitle>
                    <DialogDescription>
                        {isEditMode
                            ? 'قم بتعديل بيانات المخزن أو موقع التخزين'
                            : 'قم بإضافة مخزن أو موقع تخزين جديد (ممر، رف، صندوق، إلخ)'}
                    </DialogDescription>
                </DialogHeader>

                <FormBuilder
                    onSubmit={handleSubmit}
                    schema={inventorySchema}
                    defaultValues={defaultValues}
                    loading={isPending}
                    className="space-y-4 py-4"
                >
                    <FormBuilder.Text
                        name="name"
                        label="اسم المخزن"
                        placeholder="مثال: مستودع الرئيسي، ممر A، رف 5"
                        required
                        dir="rtl"
                    />

                    <FormBuilder.Text
                        name="code"
                        label="رمز المخزن"
                        placeholder="مثال: WH-01، A-5، BIN-12"
                        required
                        dir="ltr"
                        onChange={(e) => {
                            // Only allow letters, numbers, dash, and underscore
                            const filtered = e.target.value
                                .split('')
                                .filter((char) => /[A-Za-z0-9-_]/.test(char))
                                .join('');
                            e.target.value = filtered.toUpperCase();
                        }}
                    />

                    <FormBuilder.Select
                        name="type"
                        label="نوع المخزن"
                        options={inventoryTypeOptions}
                        required
                    />

                    <FormBuilder.Select
                        name="parentId"
                        label="المخزن الأب (اختياري)"
                        options={parentOptions}
                        placeholder="اختر المخزن الأب"
                    />

                    {isEditMode && (
                        <FormBuilder.Switch
                            name="isActive"
                            label="المخزن نشط"
                        />
                    )}

                    <DialogFooter className="gap-2 sm:gap-0 pt-4">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => setOpen(false)}
                            disabled={isPending}
                        >
                            إلغاء
                        </Button>
                        <FormBuilder.Submit loadingText={isEditMode ? 'جاري التحديث...' : 'جاري الإضافة...'}>
                            {isEditMode ? 'حفظ التعديلات' : 'إضافة المخزن'}
                        </FormBuilder.Submit>
                    </DialogFooter>
                </FormBuilder>
            </DialogContent>
        </Dialog>
    );
}
