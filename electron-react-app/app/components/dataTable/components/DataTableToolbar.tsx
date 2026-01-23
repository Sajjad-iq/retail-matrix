import type { Table } from '@tanstack/react-table';
import { DataTableColumnVisibility } from './DataTableColumnVisibility';

interface DataTableToolbarProps<TData> {
  table: Table<TData>;
  toolbar?: (table: Table<TData>) => React.ReactNode;
}

export function DataTableToolbar<TData>({
  table,
  toolbar,
}: DataTableToolbarProps<TData>) {
  return (
    <div className="flex flex-col items-start justify-between gap-3 sm:flex-row sm:items-center">
      <div className="flex w-full flex-1 items-center gap-2 sm:w-auto">
        {toolbar?.(table)}
      </div>
      <div className="flex w-full items-center justify-end gap-2 sm:w-auto">
        <DataTableColumnVisibility table={table} />
      </div>
    </div>
  );
}
