'use client';

import { ColumnDef } from '@tanstack/react-table';
import { Barcode as BarcodeIcon } from 'lucide-react';
import { Badge } from '@/app/components/ui/badge';
import { ProductPackagingListDto, ProductStatus } from '../lib/types';

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
                <div className="flex items-center gap-2 pr-8">
                    <span className="text-gray-700 text-sm">{packaging.name}</span>
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
        id: 'sellingPrice',
        accessorKey: 'sellingPrice',
        header: 'سعر البيع',
        cell: ({ row }) => {
            const price = row.original.sellingPrice;
            return (
                <span className="text-sm font-medium">
                    {price.amount.toLocaleString()} {price.currency}
                </span>
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
