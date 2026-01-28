import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/app/components/ui/badge';
import { Button } from '@/app/components/ui/button';
import { Star, Edit, Trash2 } from 'lucide-react';
import { CurrencyDto, CurrencyStatus } from '../lib/types';
import { ConfirmDialog } from '@/app/components/ui/confirm-dialog';
import { useSetBaseCurrency, useDeleteCurrency } from '../hooks/useCurrencyActions';
import { EditCurrencyDialog } from './EditCurrencyDialog';

const getStatusBadge = (status: CurrencyStatus) => {
    switch (status) {
        case CurrencyStatus.Active:
            return <Badge variant="default" className="bg-green-500/15 text-green-700 hover:bg-green-500/25 border-green-200">نشط</Badge>;
        case CurrencyStatus.Inactive:
            return <Badge variant="secondary">غير نشط</Badge>;
        default:
            return <Badge variant="secondary">غير معروف</Badge>;
    }
};

export const columns: ColumnDef<CurrencyDto>[] = [
    {
        accessorKey: 'code',
        header: 'الرمز',
        cell: ({ row }) => {
            const code = row.getValue('code') as string;
            const isBase = row.original.isBaseCurrency;
            return (
                <div className="flex items-center gap-2">
                    <span className="font-mono font-semibold">{code}</span>
                    {isBase && (
                        <Badge variant="default" className="bg-yellow-100 text-yellow-800 border-yellow-300 gap-1">
                            <Star className="h-3 w-3 fill-yellow-800" />
                            أساسية
                        </Badge>
                    )}
                </div>
            );
        },
    },
    {
        accessorKey: 'name',
        header: 'الاسم',
        cell: ({ row }) => {
            const name = row.getValue('name') as string;
            return <span className="font-medium">{name}</span>;
        },
    },
    {
        accessorKey: 'symbol',
        header: 'الرمز',
        cell: ({ row }) => {
            const symbol = row.getValue('symbol') as string;
            return <span className="font-mono">{symbol}</span>;
        },
    },
    {
        accessorKey: 'exchangeRate',
        header: 'سعر الصرف',
        cell: ({ row }) => {
            const rate = row.getValue('exchangeRate') as number;
            return <span className="font-mono">{rate.toFixed(4)}</span>;
        },
    },
    {
        accessorKey: 'status',
        header: 'الحالة',
        cell: ({ row }) => {
            const status = row.getValue('status') as CurrencyStatus;
            return getStatusBadge(status);
        },
    },
    {
        id: 'actions',
        header: 'الإجراءات',
        cell: ({ row }) => {
            return <CurrencyActions currency={row.original} />;
        },
    },
];

function CurrencyActions({ currency }: { currency: CurrencyDto }) {
    const { mutate: setBase, isPending: isSettingBase } = useSetBaseCurrency();
    const { mutate: deleteCurrency, isPending: isDeleting } = useDeleteCurrency();

    const handleSetBase = () => {
        setBase(currency.id);
    };

    const handleDelete = () => {
        deleteCurrency(currency.id);
    };

    return (
        <div className="flex items-center gap-1 justify-end">
            {!currency.isBaseCurrency && (
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-yellow-600"
                    onClick={handleSetBase}
                    disabled={isSettingBase}
                    title="تعيين كعملة أساسية"
                >
                    <Star className="h-4 w-4" />
                    <span className="sr-only">تعيين كعملة أساسية</span>
                </Button>
            )}

            <EditCurrencyDialog currency={currency}>
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-primary"
                    title="تعديل"
                >
                    <Edit className="h-4 w-4" />
                    <span className="sr-only">تعديل</span>
                </Button>
            </EditCurrencyDialog>

            <ConfirmDialog
                title="هل أنت متأكد من حذف هذه العملة؟"
                description={`سيتم حذف العملة "${currency.name}". ${currency.isBaseCurrency ? 'لا يمكن حذف العملة الأساسية.' : 'هذا الإجراء لا يمكن التراجع عنه.'}`}
                confirmText={isDeleting ? "جاري الحذف..." : "حذف"}
                cancelText="إلغاء"
                onConfirm={handleDelete}
                variant="destructive"
                disabled={currency.isBaseCurrency}
            >
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-destructive"
                    title="حذف"
                    disabled={currency.isBaseCurrency}
                >
                    <Trash2 className="h-4 w-4" />
                    <span className="sr-only">حذف</span>
                </Button>
            </ConfirmDialog>
        </div>
    );
}
