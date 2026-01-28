'use client';

import { FormBuilder } from '@/app/components/form';
import { Button } from '@/app/components/ui/button';
import { X, Search } from 'lucide-react';
import { useMyInventories } from '@/app/features/locations/hooks/useInventoryActions';
import { stockFiltersSchema, StockFiltersFormValues } from '../lib/validations';

export interface StockFilters {
    inventoryId?: string;
    productId?: string;
    productPackagingId?: string;
    productName?: string;
}

interface StockFiltersProps {
    filters: StockFilters;
    onFiltersChange: (filters: StockFilters) => void;
}

function FiltersContent({ onClear, hasActiveFilters }: { onClear: () => void; hasActiveFilters: boolean }) {
    const { data: inventoriesData } = useMyInventories({ pageNumber: 1, pageSize: 100 });

    const inventoryOptions = [
        { label: 'جميع المخازن', value: 'all' },
        ...(inventoriesData?.items.map(inv => ({
            label: inv.name,
            value: inv.id
        })) || [])
    ];

    return (
        <div className="border rounded-lg p-4 bg-card">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <FormBuilder.Select
                    name="inventoryId"
                    label="المخزن"
                    options={inventoryOptions}
                />

                <FormBuilder.Text
                    name="productName"
                    label="اسم المنتج"
                    placeholder="ابحث عن منتج..."
                />
            </div>

            <div className="mt-4 flex gap-2 justify-end">
                {hasActiveFilters && (
                    <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={onClear}
                    >
                        <X className="h-4 w-4 ml-2" />
                        مسح الفلاتر
                    </Button>
                )}
                <FormBuilder.Submit className="gap-2">
                    <Search className="h-4 w-4" />
                    بحث
                </FormBuilder.Submit>
            </div>
        </div>
    );
}

export function StockFiltersComponent({ filters, onFiltersChange }: StockFiltersProps) {
    const handleSubmit = (values: StockFiltersFormValues) => {
        onFiltersChange({
            inventoryId: values.inventoryId && values.inventoryId !== 'all' ? values.inventoryId : undefined,
            productId: filters.productId, // Keep from URL params
            productPackagingId: filters.productPackagingId, // Keep from URL params
            productName: values.productName || undefined,
        });
    };

    const handleClear = () => {
        onFiltersChange({});
    };

    const hasActiveFilters = Boolean(
        (filters.inventoryId && filters.inventoryId !== 'all') ||
        filters.productName ||
        filters.productId ||
        filters.productPackagingId
    );

    return (
        <FormBuilder
            schema={stockFiltersSchema}
            onSubmit={handleSubmit}
            defaultValues={{
                inventoryId: filters.inventoryId || 'all',
                productName: filters.productName || '',
            }}
            className="space-y-4"
        >
            <FiltersContent onClear={handleClear} hasActiveFilters={hasActiveFilters} />
        </FormBuilder>
    );
}
