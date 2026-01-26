import { useMemo, useState } from 'react';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { createColumns } from '../components/table-config';
import { useMyInventories } from '../hooks/useInventoryActions';
import { PaginationParams } from '@/app/lib/types/global';
import { Button } from '@/app/components/ui/button';
import { Plus } from 'lucide-react';
import { InventoryDialog } from '../components/InventoryDialog';

export default function InventoryPage() {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);

    // API uses 1-based indexing for page numbers
    const params: PaginationParams = {
        pageNumber: page + 1,
        pageSize: pageSize,
    };

    const { data: inventoriesData, isLoading } = useMyInventories(params);

    const pagination = useMemo(
        () => ({
            page: page, // DataTable uses 0-based indexing
            size: pageSize,
            totalElements: inventoriesData?.totalCount ?? 0,
            totalPages: inventoriesData?.totalPages ?? 0,
            onPageChange: (newPage: number) => setPage(newPage),
            onPageSizeChange: (newSize: number) => {
                setPageSize(newSize);
                setPage(0); // Reset to first page on size change
            },
        }),
        [page, pageSize, inventoriesData]
    );

    const columns = useMemo(() => createColumns(), []);

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">إدارة المخازن</h2>
                    <p className="text-muted-foreground">
                        إدارة المخازن ومواقع التخزين (المستودعات، الممرات، الأرفف، الصناديق)
                    </p>
                </div>
                <div className="flex items-center space-x-2">
                    <InventoryDialog
                        trigger={
                            <Button>
                                <Plus className="ml-2 h-4 w-4" />
                                إضافة مخزن
                            </Button>
                        }
                    />
                </div>
            </div>

            <div className="flex h-full flex-1 flex-col space-y-8">
                <DataTable
                    data={inventoriesData?.items ?? []}
                    columns={columns}
                    isLoading={isLoading}
                    pagination={pagination}
                    showToolbar={true}
                />
            </div>
        </div>
    );
}
