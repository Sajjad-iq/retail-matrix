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
import { CreatePackagingDialog } from './create-packaging-dialog';
import { Button } from '@/app/components/ui/button';
import { Plus } from 'lucide-react';

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
                                    className="text-start align-top whitespace-nowrap text-xs font-semibold text-muted-foreground py-2 h-auto"
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
                                <TableCell key={cell.id} className="text-start align-top whitespace-nowrap py-2">
                                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                </TableCell>
                            ))}
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
            <div className="mt-2">
                <CreatePackagingDialog
                    productId={product.id}
                    productName={product.name}
                >
                    <Button
                        variant="ghost"
                        size="sm"
                        className="w-full gap-2 text-muted-foreground hover:text-primary border border-dashed"
                    >
                        <Plus className="h-4 w-4" />
                        إضافة وحدة بيع
                    </Button>
                </CreatePackagingDialog>
            </div>
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
