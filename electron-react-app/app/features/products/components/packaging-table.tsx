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

    const headerGroups = subTable.getHeaderGroups();

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
            {/* Header Row */}
            {headerGroups.map((headerGroup) => (
                <TableRow
                    key={headerGroup.id}
                    className="bg-muted/50 border-muted"
                >
                    {headerGroup.headers.map((header) => (
                        <TableCell
                            key={header.id}
                            className="text-start whitespace-nowrap text-xs font-semibold text-muted-foreground py-2"
                        >
                            {header.isPlaceholder
                                ? null
                                : flexRender(header.column.columnDef.header, header.getContext())}
                        </TableCell>
                    ))}
                </TableRow>
            ))}
            {/* Data Rows */}
            {subRows.map((row) => (
                <TableRow
                    key={row.id}
                    className="bg-muted/30 border-muted transition-colors hover:bg-muted/50"
                >
                    {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id} className="text-start whitespace-nowrap">
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </TableCell>
                    ))}
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
