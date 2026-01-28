import { ColumnDef } from '@tanstack/react-table';
import { InventoryOperationItemDto, InventoryOperationDto } from '../lib/types';
import { formatPrice } from '@/lib/utils';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { useOperationItems } from '../hooks/useOperationItems';
import { Loader2 } from 'lucide-react';

export const operationItemColumns: ColumnDef<InventoryOperationItemDto>[] = [
    {
        id: 'expander',
        header: () => null,
        cell: () => null,
    },
    {
        accessorKey: 'productName',
        header: 'المنتج',
        cell: ({ row }) => (
            <div className="flex flex-col">
                <span className="font-medium">{row.original.productName}</span>
                {row.original.barcode && (
                    <span className="text-xs text-muted-foreground font-mono">
                        {row.original.barcode}
                    </span>
                )}
            </div>
        ),
    },
    {
        accessorKey: 'quantity',
        header: 'الكمية',
        cell: ({ row }) => (
            <span className="font-semibold text-primary">
                {row.original.quantity.toLocaleString('ar-SA')}
            </span>
        ),
    },
    {
        accessorKey: 'unitPrice',
        header: 'سعر الوحدة',
        cell: ({ row }) => (
            <span className="font-mono text-xs text-muted-foreground">
                {formatPrice(row.original.unitPrice.amount, row.original.unitPrice.currency)}
            </span>
        ),
    },
    {
        id: 'total',
        header: 'المجموع',
        cell: ({ row }) => {
            const total = row.original.quantity * row.original.unitPrice.amount;
            return (
                <span className="font-medium">
                    {formatPrice(total, row.original.unitPrice.currency)}
                </span>
            );
        },
    },
    {
        accessorKey: 'notes',
        header: 'ملاحظات',
        cell: ({ row }) => (
            <span className="text-sm text-muted-foreground">
                {row.original.notes || '-'}
            </span>
        ),
    },
];

function OperationItemsSubTable({ operationId }: { operationId: string }) {
    const { data: items, isLoading } = useOperationItems(operationId);

    if (isLoading) {
        return (
            <div className="flex items-center justify-center py-8">
                <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
        );
    }

    if (!items || items.length === 0) {
        return (
            <div className="text-center py-4 text-muted-foreground text-sm">
                لا توجد عناصر في هذه العملية
            </div>
        );
    }

    return (
        <div className="[&>div>div]:border-0 [&>div>div]:rounded-none">
            <DataTable
                data={items}
                columns={operationItemColumns}
                showToolbar={false}
                showPagination={false}
                meta={{
                    tableHeaderClassName: 'bg-muted/50',
                    tableHeaderRowClassName: 'border-muted hover:bg-muted/50',
                    tableHeaderCellClassName: 'text-start align-top whitespace-nowrap text-xs font-semibold text-muted-foreground py-2 h-auto',
                    tableBodyRowClassName: 'bg-muted/30 border-muted transition-colors hover:bg-muted/50',
                    tableBodyCellClassName: 'text-start align-top whitespace-nowrap py-2',
                }}
            />
        </div>
    );
}

export function createRenderSubRow() {
    return (operation: InventoryOperationDto) => {
        return (
            <div className="p-2 bg-muted/10 rounded-md">
                <OperationItemsSubTable operationId={operation.id} />
            </div>
        );
    };
}
