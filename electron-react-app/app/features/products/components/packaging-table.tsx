'use client';

import * as React from 'react';
import {
    ColumnDef,
    flexRender,
    getCoreRowModel,
    useReactTable,
} from '@tanstack/react-table';
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/app/components/ui/table';
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
            <div className="p-4 text-center">
                <span className="text-muted-foreground text-sm">
                    لا توجد وحدات بيع لهذا المنتج
                </span>
            </div>
        );
    }

    return (
        <div className="p-2 bg-muted/10 rounded-md">
            <Table>
                <TableHeader>
                    {headerGroups.map((headerGroup) => (
                        <TableRow
                            key={headerGroup.id}
                            className="bg-muted/50 border-muted hover:bg-muted/50"
                        >
                            {headerGroup.headers.map((header) => (
                                <TableHead
                                    key={header.id}
                                    className="text-start whitespace-nowrap text-xs font-semibold text-muted-foreground py-2 h-auto"
                                >
                                    {header.isPlaceholder
                                        ? null
                                        : flexRender(header.column.columnDef.header, header.getContext())}
                                </TableHead>
                            ))}
                        </TableRow>
                    ))}
                </TableHeader>
                <TableBody>
                    {subRows.map((row) => (
                        <TableRow
                            key={row.id}
                            className="bg-muted/30 border-muted transition-colors hover:bg-muted/50"
                        >
                            {row.getVisibleCells().map((cell) => (
                                <TableCell key={cell.id} className="text-start whitespace-nowrap py-2">
                                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                </TableCell>
                            ))}
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>
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
