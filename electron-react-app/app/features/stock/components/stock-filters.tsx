'use client';

import { useEffect } from 'react';
import { useFormContext } from 'react-hook-form';
import { FormBuilder } from '@/app/components/form';
import { Button } from '@/app/components/ui/button';
import { X, Search } from 'lucide-react';
import { useMyInventories } from '@/app/features/locations/hooks/useInventoryActions';
import { useMyProducts } from '@/app/features/products/hooks/useProductActions';
import { stockFiltersSchema, StockFiltersFormValues } from '../lib/validations';

export interface StockFilters {
    inventoryId?: string;
    productPackagingId?: string;
    productName?: string;
}

interface StockFiltersProps {
    filters: StockFilters;
    onFiltersChange: (filters: StockFilters) => void;
}

function FiltersContent({ onClear }: { onClear: () => void }) {
    const { watch, setValue } = useFormContext<StockFiltersFormValues>();
    
    const { data: inventoriesData } = useMyInventories({ pageNumber: 1, pageSize: 100 });
    const { data: productsData } = useMyProducts({ pageNumber: 1, pageSize: 100 });

    const productId = watch('productId');
    const selectedProduct = productsData?.items.find(p => p.id === productId);
    const packagings = selectedProduct?.packagings || [];

    const inventoryOptions = [
        { label: 'جميع المخازن', value: 'all' },
        ...(inventoriesData?.items.map(inv => ({
            label: inv.name,
            value: inv.id
        })) || [])
    ];

    const productOptions = [
        { label: 'جميع المنتجات', value: 'all' },
        ...(productsData?.items.map(prod => ({
            label: prod.name,
            value: prod.id
        })) || [])
    ];

    const packagingOptions = [
        { label: 'جميع التعبئات', value: 'all' },
        ...packagings.map(pkg => ({
            label: pkg.name,
            value: pkg.id
        }))
    ];

    // Reset packaging when product changes
    useEffect(() => {
        if (productId && productId !== 'all') {
            const currentPackagingId = watch('productPackagingId');
            if (currentPackagingId && currentPackagingId !== 'all') {
                const isValid = packagings.some(p => p.id === currentPackagingId);
                if (!isValid) {
                    setValue('productPackagingId', 'all');
                }
            }
        }
    }, [productId, packagings, watch, setValue]);

    const hasActiveFilters = (watch('inventoryId') && watch('inventoryId') !== 'all') || 
                             (watch('productPackagingId') && watch('productPackagingId') !== 'all') || 
                             watch('productName');

    return (
        <div className="border rounded-lg p-4 bg-card">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
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

                <FormBuilder.Select
                    name="productId"
                    label="المنتج"
                    options={productOptions}
                />

                <FormBuilder.Select
                    name="productPackagingId"
                    label="التعبئة"
                    options={packagingOptions}
                    disabled={!productId}
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
            productPackagingId: values.productPackagingId && values.productPackagingId !== 'all' ? values.productPackagingId : undefined,
            productName: values.productName || undefined,
        });
    };

    const handleClear = () => {
        onFiltersChange({});
    };

    return (
        <FormBuilder
            schema={stockFiltersSchema}
            onSubmit={handleSubmit}
            defaultValues={{
                inventoryId: filters.inventoryId || 'all',
                productId: 'all',
                productPackagingId: filters.productPackagingId || 'all',
                productName: filters.productName || '',
            }}
            className="space-y-4"
        >
            <FiltersContent onClear={handleClear} />
        </FormBuilder>
    );
}
