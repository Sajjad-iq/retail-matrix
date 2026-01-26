'use client';

import { Link } from 'react-router';

import { ColumnDef } from '@tanstack/react-table';
import { Barcode as BarcodeIcon, Star, Package, Ruler, Scale, Palette, Percent, Tag, Plus } from 'lucide-react';
import { Badge } from '@/app/components/ui/badge';
import { formatPrice } from '@/lib/utils';
import { getUnitLabel, getWeightUnitLabel } from '@/app/lib/constants';
import { ProductPackagingListDto, ProductStatus, DiscountType } from '../lib/types';
import { Trash2, Edit } from 'lucide-react';
import { Button } from '@/app/components/ui/button';
import { ConfirmDialog } from '@/app/components/ui/confirm-dialog';
import { CreatePackagingDialog } from './create-packaging-dialog';
import { useDeletePackaging } from '../hooks/useProductActions';
import { toast } from 'sonner';

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
                <div className="flex items-start gap-2">
                    <div className="h-8 w-8 rounded-md bg-muted flex items-center justify-center shrink-0 overflow-hidden">
                        {packaging.imageUrls && packaging.imageUrls.length > 0 ? (
                            <img src={packaging.imageUrls[0]} alt={packaging.name} className="h-full w-full object-cover" />
                        ) : (
                            <Package className="h-4 w-4 text-muted-foreground" />
                        )}
                    </div>
                    <div>
                        <div className="flex items-center gap-2">
                            {packaging.isDefault && (
                                <Star className="h-3 w-3 text-yellow-500 fill-yellow-500" />
                            )}
                            <span className="text-gray-700 text-sm font-medium">{packaging.name}</span>
                        </div>
                        {packaging.description && (
                            <span className="text-muted-foreground text-xs block">({packaging.description})</span>
                        )}
                    </div>
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
                <div className="flex items-start gap-2">
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
                <div className="flex items-start gap-1">
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
                            {formatPrice(discountedAmount, price.currency)}
                        </span>
                        <span className="text-xs text-muted-foreground line-through">
                            {formatPrice(price.amount, price.currency)}
                        </span>
                    </div>
                );
            }

            return (
                <span className="text-sm font-medium">
                    {formatPrice(price.amount, price.currency)}
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
                <div className="flex items-start gap-1">
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
                <div className="flex items-start gap-1">
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
                <div className="flex items-start gap-1">
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
                <div className="flex items-start gap-1">
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
    {
        id: 'actions',
        cell: ({ row }) => {
            return <PackagingActions packaging={row.original} />;
        },
    },
];

function PackagingActions({ packaging }: { packaging: ProductPackagingListDto }) {
    const { mutate: deletePackaging, isPending: isDeleting } = useDeletePackaging();

    const handleDelete = () => {
        deletePackaging(packaging.id);
    };

    return (
        <div className="flex items-center gap-1 justify-end">
            <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 text-muted-foreground hover:text-primary"
                title="إضافة مخزون"
                asChild
            >
                <Link to={`/inventory/stocks?packagingId=${packaging.id}&packagingName=${encodeURIComponent(packaging.name)}`}>
                    <Plus className="h-4 w-4" />
                    <span className="sr-only">إضافة مخزون</span>
                </Link>
            </Button>

            <CreatePackagingDialog
                productId="" // Not needed for edit
                productName="" // Not needed for edit if we don't display it or pass it differently
                packaging={packaging}
            >
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-primary"
                    title="تعديل وحدة البيع"
                >
                    <Edit className="h-4 w-4" />
                    <span className="sr-only">تعديل وحدة البيع</span>
                </Button>
            </CreatePackagingDialog>

            <ConfirmDialog
                title="هل أنت متأكد من حذف وحدة البيع؟"
                description={`سيتم حذف وحدة البيع "${packaging.name}". هذا الإجراء لا يمكن التراجع عنه.`}
                confirmText={isDeleting ? "جاري الحذف..." : "حذف"}
                cancelText="إلغاء"
                onConfirm={handleDelete}
                variant="destructive"
            >
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-destructive"
                    title="حذف وحدة البيع"
                >
                    <Trash2 className="h-4 w-4" />
                    <span className="sr-only">حذف وحدة البيع</span>
                </Button>
            </ConfirmDialog>
        </div>
    );
}
