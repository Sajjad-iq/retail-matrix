import { useMemo, useState } from 'react';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { columns } from '../components/columns';
import { useMyProducts } from '../hooks/useProductActions';
import { PaginationParams } from '@/app/lib/types/global';
import { Button } from '@/app/components/ui/button';
import { Plus } from 'lucide-react';
import { createRenderSubRow } from '../components/packaging-table';
import { CreateProductDialog } from '../components/CreateProductDialog';

export default function ProductsPage() {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);

    // API uses 1-based indexing for page numbers
    const params: PaginationParams = {
        pageNumber: page + 1,
        pageSize: pageSize,
    };

    const { data: productsData, isLoading } = useMyProducts(params);

    const pagination = useMemo(
        () => ({
            page: page, // DataTable uses 0-based indexing
            size: pageSize,
            totalElements: productsData?.totalCount ?? 0,
            totalPages: productsData?.totalPages ?? 0,
            onPageChange: (newPage: number) => setPage(newPage),
            onPageSizeChange: (newSize: number) => {
                setPageSize(newSize);
                setPage(0); // Reset to first page on size change
            },
        }),
        [page, pageSize, productsData]
    );

    // Create renderSubRow function for packagings
    const renderSubRow = useMemo(() => createRenderSubRow(), []);

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">المنتجات</h2>
                    <p className="text-muted-foreground">
                        إدارة قائمة المنتجات والمخزون الخاص بك
                    </p>
                </div>
                <div className="flex items-center space-x-2">
                    <CreateProductDialog>
                        <Button>
                            <Plus className="ml-2 h-4 w-4" />
                            إضافة منتج
                        </Button>
                    </CreateProductDialog>
                </div>
            </div>

            <div className="flex h-full flex-1 flex-col space-y-8">
                <DataTable
                    data={productsData?.items ?? []}
                    columns={columns}
                    isLoading={isLoading}
                    pagination={pagination}
                    showToolbar={true}
                    meta={{
                        renderSubRow,
                    }}
                />
            </div>
        </div>
    );
}
