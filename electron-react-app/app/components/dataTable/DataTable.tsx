import type { VisibilityState, ColumnDef } from '@tanstack/react-table';
import {
  getCoreRowModel,
  getSortedRowModel,
  getExpandedRowModel,
  useReactTable,
} from '@tanstack/react-table';
import * as React from 'react';
import {
  DataTableToolbar,
  DataTableContent,
  DataTablePagination,
} from './components';

export interface ServerPaginationState {
  page: number;
  size: number;
  totalElements: number;
  totalPages: number;
  onPageChange?: (page: number) => void;
  onPageSizeChange?: (size: number) => void;
}

export interface PaginationConfig {
  showPageInfo?: boolean;
  showSelectedCount?: boolean;
  showPageSize?: boolean;
}

// Table meta type for custom options
export interface DataTableMeta<TData> {
  renderSubRow?: (row: TData, index: number) => React.ReactNode;
  tableHeaderClassName?: string;
  tableHeaderRowClassName?: string;
  tableHeaderCellClassName?: string;
  tableBodyRowClassName?: string;
  tableBodyCellClassName?: string;
}

export interface DataTableProps<TData, TValue> {
  data: TData[];
  columns: ColumnDef<TData, TValue>[];
  initialColumnVisibility?: VisibilityState;
  toolbar?: (table: ReturnType<typeof useReactTable<TData>>) => React.ReactNode;
  isLoading?: boolean;
  onRetry?: () => void;
  pagination?: ServerPaginationState;

  // Table meta for custom options (e.g., renderSubRow)
  meta?: DataTableMeta<TData>;

  // Show/hide table features
  showToolbar?: boolean;
  showPagination?: boolean;

  // Pagination feature config
  paginationConfig?: PaginationConfig;
}

export function DataTable<TData, TValue>({
  data,
  columns,
  initialColumnVisibility = {},
  toolbar,
  isLoading = false,
  onRetry,
  pagination,
  meta,
  showToolbar = true,
  showPagination = true,
  paginationConfig,
}: DataTableProps<TData, TValue>) {
  // Manage column visibility state
  const [columnVisibility, setColumnVisibility] =
    React.useState<VisibilityState>(initialColumnVisibility);

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getExpandedRowModel: getExpandedRowModel(),
    manualPagination: true,
    pageCount: pagination?.totalPages ?? 0,
    meta,
    state: {
      columnVisibility,
      pagination: {
        pageIndex: pagination?.page ?? 0,
        pageSize: pagination?.size ?? 20,
      },
    },
    onColumnVisibilityChange: setColumnVisibility,
  });

  const rows = table.getRowModel().rows;

  return (
    <div className="flex h-full w-full flex-col gap-4">
      {showToolbar && (
        <div className="flex-shrink-0">
          <DataTableToolbar table={table} toolbar={toolbar} />
        </div>
      )}
      <div className="min-h-0 flex-1">
        <DataTableContent
          table={table}
          columns={columns}
          isLoading={isLoading}
        />
      </div>
      {showPagination && rows.length > 0 && (
        <div className="flex-shrink-0">
          <DataTablePagination
            table={table}
            totalElements={pagination?.totalElements ?? 0}
            onPageChange={pagination?.onPageChange}
            onPageSizeChange={pagination?.onPageSizeChange}
            config={paginationConfig}
          />
        </div>
      )}
    </div>
  );
}
