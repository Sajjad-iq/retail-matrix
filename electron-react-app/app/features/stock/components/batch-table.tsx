import { ColumnDef } from '@tanstack/react-table';
import { StockBatchDto, StockCondition } from '../lib/types';
import { formatPrice } from '@/lib/utils';
import { Badge } from '@/app/components/ui/badge';
import { Calendar } from 'lucide-react';
import { DataTable } from '@/app/components/dataTable/DataTable';
import { StockListDto } from '../lib/types';

const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ar-SA', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    }).format(date);
};

const getConditionLabel = (condition: StockCondition) => {
    switch (condition) {
        case StockCondition.New: return 'جديد';
        case StockCondition.Damaged: return 'تالف';
        case StockCondition.Refurbished: return 'مجدد';
        case StockCondition.Used: return 'مستعمل';
        default: return '-';
    }
};

export const batchColumns: ColumnDef<StockBatchDto>[] = [
    {
        accessorKey: 'batchNumber',
        header: 'رقم الدفعة',
        cell: ({ row }) => <span className="font-mono text-xs">{row.original.batchNumber}</span>
    },
    {
        accessorKey: 'quantity',
        header: 'الكمية الكلية',
        cell: ({ row }) => <span className="font-medium">{row.original.quantity}</span>
    },
    {
        accessorKey: 'availableQuantity',
        header: 'الكمية المتاحة',
        cell: ({ row }) => {
            const qty = row.original.availableQuantity;
            return (
                <span className={qty > 0 ? "text-green-600 font-medium" : "text-red-500"}>
                    {qty}
                </span>
            );
        }
    },
    {
        id: 'expiryDate',
        header: 'تاريخ الصلاحية',
        cell: ({ row }) => {
            const date = row.original.expiryDate;
            const isExpired = row.original.isExpired;
            if (!date) return '-';

            return (
                <div className={`flex items-center gap-2 text-sm ${isExpired ? "text-red-600 font-medium" : "text-muted-foreground"}`}>
                    <Calendar className="h-3 w-3" />
                    <span>{formatDate(date)}</span>
                    {isExpired && <Badge variant="destructive" className="text-[10px] h-5">منتهي</Badge>}
                </div>
            );
        }
    },
    {
        id: 'condition',
        header: 'الحالة',
        cell: ({ row }) => <Badge variant="outline">{getConditionLabel(row.original.condition)}</Badge>
    },
    {
        id: 'costPrice',
        header: 'سعر التكلفة',
        cell: ({ row }) => {
            const price = row.original.costPrice;
            if (!price) return '-';
            return <span className="text-xs text-muted-foreground">{formatPrice(price.amount, price.currency)}</span>
        }
    }
];

export function createRenderSubRow() {
    return (stock: StockListDto) => {
        const batches = stock.batches || [];

        if (batches.length === 0) {
            return (
                <div className="p-4 text-center text-sm text-muted-foreground bg-muted/30">
                    لا توجد دفعات لهذا المخزون
                </div>
            );
        }

        return (
            <div className="p-4 bg-muted/10 rounded-md">
                <h4 className="text-sm font-semibold mb-2">الدفعات ({batches.length})</h4>
                <div className="border rounded-md bg-background">
                    <DataTable
                        data={batches}
                        columns={batchColumns}
                        showToolbar={false}
                    />
                </div>
            </div>
        );
    };
}
