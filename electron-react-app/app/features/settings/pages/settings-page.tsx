import { useState, useMemo } from 'react';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { columns } from '../components/currency-table-config';
import { useMyCurrencies } from '../hooks/useCurrencyActions';
import { Button } from '@/app/components/ui/button';
import { Plus, Coins } from 'lucide-react';
import { CreateCurrencyDialog } from '../components/CreateCurrencyDialog';

export default function SettingsPage() {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);

    const { data: currenciesData, isLoading } = useMyCurrencies({
        pageNumber: page + 1,
        pageSize: pageSize,
    });

    const pagination = useMemo(
        () => ({
            page: page,
            size: pageSize,
            totalElements: currenciesData?.totalCount ?? 0,
            totalPages: currenciesData?.totalPages ?? 0,
            onPageChange: (newPage: number) => setPage(newPage),
            onPageSizeChange: (newSize: number) => {
                setPageSize(newSize);
                setPage(0);
            },
        }),
        [page, pageSize, currenciesData]
    );

    return (
        <div className="flex h-full flex-col space-y-4 p-4 md:p-8 pt-6">
            <div className="flex items-center justify-between space-y-2">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">الإعدادات</h2>
                    <p className="text-muted-foreground">
                        إدارة إعدادات النظام والعملات
                    </p>
                </div>
            </div>

            {/* Currency Section */}
            <div className="flex flex-col space-y-4">
                <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                        <Coins className="h-5 w-5 text-primary" />
                        <h3 className="text-xl font-semibold">إدارة العملات</h3>
                    </div>
                    <CreateCurrencyDialog>
                        <Button>
                            <Plus className="ml-2 h-4 w-4" />
                            إضافة عملة
                        </Button>
                    </CreateCurrencyDialog>
                </div>

                <DataTable
                    data={currenciesData?.items ?? []}
                    columns={columns}
                    isLoading={isLoading}
                    pagination={pagination}
                    showToolbar={true}
                />
            </div>
        </div>
    );
}
