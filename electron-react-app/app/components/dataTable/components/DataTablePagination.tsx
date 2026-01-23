import type { Table } from '@tanstack/react-table';
import { DataTablePageSize } from './DataTablePageSize';
import { DataTablePaginationControls } from './DataTablePaginationControls';
import type { PaginationConfig } from '../DataTable';

interface DataTablePaginationProps<TData> {
  table: Table<TData>;
  totalElements: number;
  onPageChange?: (page: number) => void;
  onPageSizeChange?: (size: number) => void;
  config?: PaginationConfig;
}

export function DataTablePagination<TData>({
  table,
  totalElements,
  onPageChange,
  onPageSizeChange,
  config,
}: DataTablePaginationProps<TData>) {
  const currentPage = table.getState().pagination.pageIndex;
  const totalPages = table.getPageCount();
  const canPreviousPage = currentPage > 0;
  const canNextPage = currentPage < totalPages - 1;

  // Config with defaults
  const {
    showPageInfo = true,
    showSelectedCount = true,
    showPageSize = true,
  } = config ?? {};

  return (
    <div className="bg-card/50 flex flex-col items-center justify-between gap-4 py-1 sm:flex-row">
      <div className="text-muted-foreground flex items-center gap-4 text-xs sm:text-sm">
        {showPageInfo && (
          <span>
            صفحة {currentPage + 1} من {totalPages || 1}
          </span>
        )}
        {showSelectedCount &&
          table.getFilteredSelectedRowModel().rows.length > 0 && (
            <span className="bg-primary/10 text-primary rounded-md px-2 py-1 text-xs font-semibold">
              {`${table.getFilteredSelectedRowModel().rows.length} من ${table.getFilteredRowModel().rows.length} محدد`}
            </span>
          )}
      </div>

      <div className="flex flex-col items-center gap-3 overflow-x-auto sm:flex-row">
        {showPageSize && (
          <DataTablePageSize
            table={table}
            onPageSizeChange={onPageSizeChange}
          />
        )}

        <div className="shrink-0">
          <DataTablePaginationControls
            currentPage={currentPage}
            totalPages={totalPages}
            canPreviousPage={canPreviousPage}
            canNextPage={canNextPage}
            onPageChange={onPageChange}
          />
        </div>
      </div>
    </div>
  );
}
