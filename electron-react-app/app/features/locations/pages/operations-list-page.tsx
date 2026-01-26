import { useState, useMemo } from 'react';
import { useMyInventoryOperations } from '../hooks/useInventoryActions';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { PaginationParams } from '@/app/lib/types/global';
import { ColumnDef } from '@tanstack/react-table';
import { InventoryOperationDto, InventoryOperationType } from '../lib/types';
import { Badge } from '@/app/components/ui/badge';
import { Calendar } from 'lucide-react';

const getOperationTypeLabel = (type: InventoryOperationType) => {
    switch (type) {
        case InventoryOperationType.Purchase: return 'شراء';
        case InventoryOperationType.Sale: return 'بيع';
        case InventoryOperationType.Transfer: return 'نقل';
        case InventoryOperationType.Stocktake: return 'جرد';
        case InventoryOperationType.Adjustment: return 'تعديل';
        case InventoryOperationType.Return: return 'إرجاع';
        case InventoryOperationType.Damage: return 'تالف';
        case InventoryOperationType.Expired: return 'منتهي الصلاحية';
        default: return '-';
    }
};

const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ar-SA', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    }).format(date);
};

const columns: ColumnDef<InventoryOperationDto>[] = [
    {
        accessorKey: 'operationNumber',
        header: 'رقم العملية',
        cell: ({ row }) => <span className="font-mono text-sm">{row.original.operationNumber}</span>
    },
    {
        accessorKey: 'operationType',
        header: 'نوع العملية',
        cell: ({ row }) => <Badge variant="secondary">{getOperationTypeLabel(row.original.operationType)}</Badge>
    },
    {
        accessorKey: 'notes',
        header: 'ملاحظات',
        cell: ({ row }) => {
            const notes = row.original.notes;
            return <span className="text-sm text-muted-foreground">{notes || '-'}</span>
        }
    },
    {
        accessorKey: 'insertDate',
        header: 'التاريخ',
        cell: ({ row }) => (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Calendar className="h-4 w-4" />
                <span>{formatDate(row.original.insertDate)}</span>
            </div>
        )
    }
];

export default function InventoryOperationsPage() {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);

    const params: PaginationParams = {
        pageNumber: page + 1,
        pageSize: pageSize,
    };

    const { data: operationsData, isLoading } = useMyInventoryOperations(params);

    const pagination = useMemo(
        () => ({
            page: page,
            size: pageSize,
            totalElements: operationsData?.totalCount ?? 0,
            totalPages: operationsData?.totalPages ?? 0,
            onPageChange: (newPage: number) => setPage(newPage),
            onPageSizeChange: (newSize: number) => {
                setPageSize(newSize);
                setPage(0);
            },
        }),
        [page, pageSize, operationsData]
    );

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">سجل العمليات</h2>
                    <p className="text-muted-foreground">
                        تاريخ حركات المخزون والعمليات السابقة
                    </p>
                </div>
            </div>

            <div className="flex h-full flex-1 flex-col space-y-8">
                <DataTable
                    data={operationsData?.items ?? []}
                    columns={columns}
                    isLoading={isLoading}
                    pagination={pagination}
                    showToolbar={true}
                />
            </div>
        </div>
    );
}
