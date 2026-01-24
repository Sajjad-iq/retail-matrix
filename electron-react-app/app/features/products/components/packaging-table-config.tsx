'use client';

import { ColumnDef } from '@tanstack/react-table';
import { Barcode as BarcodeIcon, Star, Package, Ruler, Scale, Palette, Percent, Tag } from 'lucide-react';
import { Badge } from '@/app/components/ui/badge';
import { ProductPackagingListDto, ProductStatus, DiscountType, UnitOfMeasure } from '../lib/types';

const getStatusBadge = (status: ProductStatus) => {
    switch (status) {
        case ProductStatus.Active:
            return <Badge variant="default" className="bg-green-500/15 text-green-700 hover:bg-green-500/25 border-green-200">نشط</Badge>;
        case ProductStatus.Inactive:
            return <Badge variant="secondary">غير نشط</Badge>;
        case ProductStatus.OutOfStock:
            return <Badge variant="destructive">نفذت الكمية</Badge>;
        case ProductStatus.Discontinued:
            return <Badge variant="outline">متوقف</Badge>;
        default:
            return <Badge variant="secondary">غير معروف</Badge>;
    }
};

const getUnitLabel = (unit: UnitOfMeasure): string => {
    const labels: Record<UnitOfMeasure, string> = {
        [UnitOfMeasure.Piece]: 'قطعة',
        [UnitOfMeasure.Dozen]: 'درزن',
        [UnitOfMeasure.Pair]: 'زوج',
        [UnitOfMeasure.Set]: 'طقم',
        [UnitOfMeasure.Kilogram]: 'كغم',
        [UnitOfMeasure.Gram]: 'غرام',
        [UnitOfMeasure.Milligram]: 'ملغم',
        [UnitOfMeasure.MetricTon]: 'طن',
        [UnitOfMeasure.Pound]: 'باوند',
        [UnitOfMeasure.Ounce]: 'أونصة',
        [UnitOfMeasure.Liter]: 'لتر',
        [UnitOfMeasure.Milliliter]: 'مل',
        [UnitOfMeasure.CubicMeter]: 'م³',
        [UnitOfMeasure.CubicCentimeter]: 'سم³',
        [UnitOfMeasure.Gallon]: 'غالون',
        [UnitOfMeasure.Quart]: 'كوارت',
        [UnitOfMeasure.Pint]: 'باينت',
        [UnitOfMeasure.FluidOunce]: 'أونصة سائلة',
        [UnitOfMeasure.Meter]: 'متر',
        [UnitOfMeasure.Centimeter]: 'سم',
        [UnitOfMeasure.Millimeter]: 'مم',
        [UnitOfMeasure.Kilometer]: 'كم',
        [UnitOfMeasure.Foot]: 'قدم',
        [UnitOfMeasure.Inch]: 'بوصة',
        [UnitOfMeasure.Yard]: 'ياردة',
        [UnitOfMeasure.SquareMeter]: 'م²',
        [UnitOfMeasure.SquareFoot]: 'قدم²',
        [UnitOfMeasure.SquareCentimeter]: 'سم²',
        [UnitOfMeasure.Box]: 'صندوق',
        [UnitOfMeasure.Pack]: 'علبة',
        [UnitOfMeasure.Bag]: 'كيس',
        [UnitOfMeasure.Bottle]: 'زجاجة',
        [UnitOfMeasure.Can]: 'علبة معدنية',
        [UnitOfMeasure.Jar]: 'مرطبان',
        [UnitOfMeasure.Tube]: 'أنبوب',
        [UnitOfMeasure.Roll]: 'رول',
        [UnitOfMeasure.Sheet]: 'ورقة',
        [UnitOfMeasure.Pallet]: 'طبلية',
        [UnitOfMeasure.Crate]: 'صندوق خشبي',
        [UnitOfMeasure.Bundle]: 'حزمة',
        [UnitOfMeasure.Carton]: 'كرتون',
        [UnitOfMeasure.Container]: 'حاوية',
    };
    return labels[unit] || 'غير معروف';
};

const getWeightUnitLabel = (unit: UnitOfMeasure): string => {
    const labels: Partial<Record<UnitOfMeasure, string>> = {
        [UnitOfMeasure.Kilogram]: 'كغم',
        [UnitOfMeasure.Gram]: 'غرام',
        [UnitOfMeasure.Milligram]: 'ملغم',
        [UnitOfMeasure.MetricTon]: 'طن',
        [UnitOfMeasure.Pound]: 'باوند',
        [UnitOfMeasure.Ounce]: 'أونصة',
    };
    return labels[unit] || '';
};

export const createPackagingTableColumns = (): ColumnDef<ProductPackagingListDto>[] => [
    {
        id: 'expander',
        header: () => null,
        cell: () => null,
    },
    {
        id: 'name',
        accessorKey: 'name',
        header: 'اسم الوحدة',
        cell: ({ row }) => {
            const packaging = row.original;
            return (
                <div className="flex items-center gap-2 pr-8">
                    {packaging.isDefault && (
                        <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />
                    )}
                    <span className="text-gray-700 text-sm font-medium">{packaging.name}</span>
                    {packaging.description && (
                        <span className="text-muted-foreground text-xs">({packaging.description})</span>
                    )}
                </div>
            );
        },
    },
    {
        id: 'barcode',
        accessorKey: 'barcode',
        header: 'الباركود',
        cell: ({ row }) => {
            const barcode = row.original.barcode;
            if (!barcode?.value) {
                return <span className="text-muted-foreground text-xs">-</span>;
            }
            return (
                <div className="flex items-center gap-2">
                    <BarcodeIcon className="h-3 w-3 text-muted-foreground" />
                    <span className="text-xs font-mono">{barcode.value}</span>
                </div>
            );
        },
    },
    {
        id: 'unitInfo',
        accessorKey: 'unitsPerPackage',
        header: 'الوحدة',
        cell: ({ row }) => {
            const packaging = row.original;
            return (
                <div className="flex items-center gap-1">
                    <Package className="h-3 w-3 text-muted-foreground" />
                    <span className="text-sm">
                        {packaging.unitsPerPackage} {getUnitLabel(packaging.unitOfMeasure)}
                    </span>
                </div>
            );
        },
    },
    {
        id: 'sellingPrice',
        accessorKey: 'sellingPrice',
        header: 'سعر البيع',
        cell: ({ row }) => {
            const price = row.original.sellingPrice;
            const discount = row.original.discount;
            const hasDiscount = discount && discount.type !== DiscountType.None && discount.value > 0;

            if (hasDiscount) {
                const discountedAmount = discount.type === DiscountType.Percentage
                    ? price.amount * (1 - discount.value / 100)
                    : Math.max(0, price.amount - discount.value);

                return (
                    <div className="flex flex-col">
                        <span className="text-sm font-medium text-green-600">
                            {discountedAmount.toLocaleString()} {price.currency}
                        </span>
                        <span className="text-xs text-muted-foreground line-through">
                            {price.amount.toLocaleString()} {price.currency}
                        </span>
                    </div>
                );
            }

            return (
                <span className="text-sm font-medium">
                    {price.amount.toLocaleString()} {price.currency}
                </span>
            );
        },
    },
    {
        id: 'discount',
        accessorKey: 'discount',
        header: 'الخصم',
        cell: ({ row }) => {
            const discount = row.original.discount;
            if (!discount || discount.type === DiscountType.None || discount.value === 0) {
                return <span className="text-muted-foreground text-xs">-</span>;
            }

            return (
                <div className="flex items-center gap-1">
                    {discount.type === DiscountType.Percentage ? (
                        <>
                            <Percent className="h-3 w-3 text-orange-500" />
                            <Badge variant="outline" className="bg-orange-50 text-orange-700 border-orange-200">
                                {discount.value}%
                            </Badge>
                        </>
                    ) : (
                        <>
                            <Tag className="h-3 w-3 text-orange-500" />
                            <Badge variant="outline" className="bg-orange-50 text-orange-700 border-orange-200">
                                {discount.value.toLocaleString()}
                            </Badge>
                        </>
                    )}
                </div>
            );
        },
    },
    {
        id: 'weight',
        accessorKey: 'weight',
        header: 'الوزن',
        cell: ({ row }) => {
            const weight = row.original.weight;
            if (!weight) {
                return <span className="text-muted-foreground text-xs">-</span>;
            }
            return (
                <div className="flex items-center gap-1">
                    <Scale className="h-3 w-3 text-muted-foreground" />
                    <span className="text-xs">
                        {weight.value} {getWeightUnitLabel(weight.unit)}
                    </span>
                </div>
            );
        },
    },
    {
        id: 'dimensions',
        accessorKey: 'dimensions',
        header: 'الأبعاد',
        cell: ({ row }) => {
            const dimensions = row.original.dimensions;
            if (!dimensions) {
                return <span className="text-muted-foreground text-xs">-</span>;
            }
            return (
                <div className="flex items-center gap-1">
                    <Ruler className="h-3 w-3 text-muted-foreground" />
                    <span className="text-xs">{dimensions}</span>
                </div>
            );
        },
    },
    {
        id: 'color',
        accessorKey: 'color',
        header: 'اللون',
        cell: ({ row }) => {
            const color = row.original.color;
            if (!color) {
                return <span className="text-muted-foreground text-xs">-</span>;
            }

            const isHexColor = color.startsWith('#');
            return (
                <div className="flex items-center gap-1">
                    {isHexColor ? (
                        <div
                            className="h-3 w-3 rounded-full border border-gray-300"
                            style={{ backgroundColor: color }}
                        />
                    ) : (
                        <Palette className="h-3 w-3 text-muted-foreground" />
                    )}
                    <span className="text-xs">{isHexColor ? '' : color}</span>
                </div>
            );
        },
    },
    {
        id: 'status',
        accessorKey: 'status',
        header: 'الحالة',
        cell: ({ row }) => {
            const status = row.original.status;
            return getStatusBadge(status);
        },
    },
];
