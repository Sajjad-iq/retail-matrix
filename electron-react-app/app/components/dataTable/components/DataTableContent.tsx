import * as React from 'react';
import type { Table, ColumnDef } from '@tanstack/react-table';
import { flexRender } from '@tanstack/react-table';
import {
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/app/components/ui/table';
import { Skeleton } from '@/app/components/ui/skeleton';
import { cn } from '@/lib/utils';
import type { DataTableMeta } from '../DataTable';
import { EmptyState } from '@/app/components/common';
import { Inbox } from 'lucide-react';

interface DataTableContentProps<TData, TValue> {
  table: Table<TData>;
  columns: ColumnDef<TData, TValue>[];
  isLoading?: boolean;
}

export function DataTableContent<TData, TValue>({
  table,
  columns,
  isLoading,
}: DataTableContentProps<TData, TValue>) {
  const tableMeta = table.options.meta as DataTableMeta<TData> | undefined;
  const renderSubRow = tableMeta?.renderSubRow;
  const rows = table.getRowModel().rows;

  return (
    <div className="bg-card border-border flex h-full flex-col overflow-hidden rounded-md border">
      <div className="flex-1 overflow-auto">
        <table
          className="w-full caption-bottom text-sm"
          style={{ height: rows.length >= 10 ? '100%' : 'auto' }}
        >
          <TableHeader
            className={cn(
              'bg-muted/90 border-border sticky top-0 z-10 border-b-2',
              tableMeta?.tableHeaderClassName,
            )}
          >
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow
                key={headerGroup.id}
                className={cn(
                  'border-0 hover:bg-transparent',
                  tableMeta?.tableHeaderRowClassName,
                )}
              >
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    className={cn(
                      'font-semibold text-start',
                      tableMeta?.tableHeaderCellClassName,
                    )}
                  >
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                        header.column.columnDef.header,
                        header.getContext(),
                      )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody className="h-full">
            {isLoading ? (
              // Show skeleton rows when loading
              Array.from({ length: rows.length || 100 }).map((_, i) => (
                <TableRow
                  key={`skeleton-${i}`}
                  className="border-border border-b"
                >
                  {columns.map((_, j) => (
                    <TableCell key={`skeleton-${i}-${j}`}>
                      <Skeleton className="h-4 w-full" />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : rows.length ? (
              rows.map((row) => (
                <React.Fragment key={row.id}>
                  <TableRow
                    data-state={row.getIsSelected() && 'selected'}
                    className={cn(
                      'border-border border-b transition-colors',
                      tableMeta?.tableBodyRowClassName,
                    )}
                  >
                    {row.getVisibleCells().map((cell) => (
                      <TableCell
                        key={cell.id}
                        className={cn('text-start', tableMeta?.tableBodyCellClassName)}
                      >
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext(),
                        )}
                      </TableCell>
                    ))}
                  </TableRow>

                  {/* Render sub-rows if expanded */}
                  {row.getIsExpanded() && renderSubRow && (
                    <TableRow>
                      <TableCell colSpan={columns.length} className="p-0">
                        {renderSubRow(row.original, row.index)}
                      </TableCell>
                    </TableRow>
                  )}
                </React.Fragment>
              ))
            ) : (
              <TableRow className="hover:bg-transparent">
                <TableCell className="h-[500px]" colSpan={columns.length}>
                  <EmptyState
                    icon={Inbox}
                    title="لا يوجد بيانات"
                    iconSize="md"
                  />
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </table>
      </div>
    </div>
  );
}
