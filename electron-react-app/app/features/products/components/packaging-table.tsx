'use client';

import { ColumnDef } from '@tanstack/react-table';
import { ProductPackagingListDto, ProductListDto } from '../lib/types';
import { createPackagingTableColumns } from './packaging-table-config';
import { Button } from '@/app/components/ui/button';
import { Plus, Loader2 } from 'lucide-react';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { CreatePackagingDialog } from './create-packaging-dialog';
import { useProductPackagings } from '../hooks/useProductActions';

const ProductPackagingsSubTable = ({ product }: { product: ProductListDto }) => {
    const { data, isLoading } = useProductPackagings(product.id);
    const packagings = data?.items || [];

    if (isLoading) {
        return (
            <div className="p-4 flex justify-center text-muted-foreground">
                <Loader2 className="h-5 w-5 animate-spin mr-2" />
                جاري تحميل وحدات البيع...
            </div>
        );
    }

    const columns = createPackagingTableColumns();

    return (
        <div className="p-2 bg-muted/10 rounded-md">
            {packagings.length > 0 ? (
                <div className="[&>div>div]:border-0 [&>div>div]:rounded-none">
                    <DataTable
                        data={packagings}
                        columns={columns}
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
            ) : (
                <div className="text-center p-4 text-muted-foreground text-sm border border-dashed rounded-md bg-muted/30">
                    لا توجد وحدات بيع لهذا المنتج
                </div>
            )}
            <div className="mt-2">
                <CreatePackagingDialog
                    productId={product.id}
                    productName={product.name}
                >
                    <Button
                        variant="ghost"
                        size="sm"
                        className="w-full gap-2 text-muted-foreground hover:text-primary border border-dashed"
                    >
                        <Plus className="h-4 w-4" />
                        إضافة وحدة بيع
                    </Button>
                </CreatePackagingDialog>
            </div>
        </div>
    );
};

export function createRenderSubRow() {
    return (product: ProductListDto) => <ProductPackagingsSubTable product={product} />;
};
