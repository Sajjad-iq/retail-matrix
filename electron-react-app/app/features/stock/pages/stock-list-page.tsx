import { useState, useMemo } from 'react';
import { useMyStocks } from '../hooks/useStock';
import { columns } from '../components/stock-table-config';
import { createRenderSubRow } from '../components/batch-table';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { CreateStockDialog } from '../components/create-stock-dialog';
import { StockFiltersComponent, StockFilters } from '../components/stock-filters';
import { StockQueryParams } from '../services/stockService';

export default function StockListPage() {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [filters, setFilters] = useState<StockFilters>({});

    const params: StockQueryParams = {
        pageNumber: page + 1,
        pageSize: pageSize,
        inventoryId: filters.inventoryId,
        productPackagingId: filters.productPackagingId,
        productName: filters.productName,
    };

    const { data: stocksData, isLoading } = useMyStocks(params);

    const handleFiltersChange = (newFilters: StockFilters) => {
        setFilters(newFilters);
        setPage(0); // Reset to first page when filters change
    };

    const pagination = useMemo(
        () => ({
            page: page,
            size: pageSize,
            totalElements: stocksData?.totalCount ?? 0,
            totalPages: stocksData?.totalPages ?? 0,
            onPageChange: (newPage: number) => setPage(newPage),
            onPageSizeChange: (newSize: number) => {
                setPageSize(newSize);
                setPage(0);
            },
        }),
        [page, pageSize, stocksData]
    );

    const renderSubRow = useMemo(() => createRenderSubRow(), []);

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">المخزون</h2>
                    <p className="text-muted-foreground">
                        عرض وإدارة المخزون الحالي والدفعات
                    </p>
                </div>
                <div className="flex items-center space-x-2">
                    <div className="flex items-center space-x-2">
                        <CreateStockDialog>
                            <span className="hidden" />
                        </CreateStockDialog>
                    </div>
                </div>
            </div>

            {/* Filters */}
            <StockFiltersComponent filters={filters} onFiltersChange={handleFiltersChange} />

            <div className="flex h-full flex-1 flex-col space-y-8">
                <DataTable
                    data={stocksData?.items ?? []}
                    columns={columns}
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
