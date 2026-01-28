import { useState, useMemo } from 'react';
import { useMyInventoryOperations } from '../hooks/useInventoryActions';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { PaginationParams } from '@/app/lib/types/global';
import { createRenderSubRow } from '../components/operation-items-table';
import { operationColumns } from '../components/operations-table-config';

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

    const renderSubRow = useMemo(() => createRenderSubRow(), []);

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
                    columns={operationColumns}
                    isLoading={isLoading}
                    pagination={pagination}
                    showToolbar={true}
                    meta={{
                        renderSubRow,
                        tableBodyCellClassName: 'align-top',
                        tableHeaderCellClassName: 'align-top',
                    }}
                />
            </div>
        </div>
    );
}
