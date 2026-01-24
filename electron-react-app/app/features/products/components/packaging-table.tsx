'use client';

import * as React from 'react';
import {
    ColumnDef,
    flexRender,
    getCoreRowModel,
    useReactTable,
} from '@tanstack/react-table';
import { TableRow, TableCell } from '@/app/components/ui/table';
import type { ProductWithPackagingsDto, ProductPackagingListDto } from '../lib/types';
import { createPackagingTableColumns } from './packaging-table-config';

interface PackagingSubRowsProps {
    product: ProductWithPackagingsDto;
    columns: ColumnDef<ProductPackagingListDto>[];
}

function PackagingSubRows({
    product,
    columns,
}: PackagingSubRowsProps) {
    const subTable = useReactTable({
        data: product.packagings,
        columns,
        getCoreRowModel: getCoreRowModel(),
    });

    const subRows = subTable.getRowModel().rows;

    if (subRows.length === 0) {
        return (
            <TableRow>
                <TableCell colSpan={columns.length} className="py-4 text-center">
                    <span className="text-muted-foreground text-sm">
                        لا توجد وحدات بيع لهذا المنتج
                    </span>
                </TableCell>
            </TableRow>
        );
    }

    return (
        <>
            {subRows.map((row) => (
                <TableRow
                    key={row.id}
                    className="bg-muted/30 border-muted transition-colors hover:bg-muted/50"
                >
                    {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id} className="text-center whitespace-nowrap">
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </TableCell>
                    ))}/home/sajjad/Documents/bms-frontend/app/(app)/(admin)/pre-registration-management
                </TableRow>
            ))}
        </>
    );
}

export const createRenderSubRow = () => {
    const columns = createPackagingTableColumns();

    const RenderSubRow = (product: ProductWithPackagingsDto) => {
        return (
            <PackagingSubRows
                product={product}
                columns={columns}
            />
        );
    };
    RenderSubRow.displayName = 'RenderSubRow';
    return RenderSubRow;
};
