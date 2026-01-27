import { ColumnDef } from '@tanstack/react-table';
import { StockListDto } from '../lib/types';
import { Package, ChevronDown, ChevronUp, Calendar, Warehouse } from 'lucide-react';
import { Button } from '@/app/components/ui/button';
import { Badge } from '@/app/components/ui/badge';

const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ar-SA', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    }).format(date);
};

export const columns: ColumnDef<StockListDto>[] = [
    {
        id: 'expander',
        header: () => null,
        cell: ({ row }) => {
            const hasBatches = row.original.batches && row.original.batches.length > 0;

            if (!hasBatches) {
                return <div className="w-8" />;
            }

            return (
                <div className="flex items-center justify-center">
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => row.toggleExpanded()}
                        className="h-8 w-8 p-0"
                    >
                        {row.getIsExpanded() ? (
                            <ChevronUp className="h-4 w-4" />
                        ) : (
                            <ChevronDown className="h-4 w-4" />
                        )}
                    </Button>
                </div>
            );
        },
    },
    {
        accessorKey: 'productName',
        header: 'المنتج والتعبئة',
        cell: ({ row }) => {
            const stock = row.original;
            const productName = stock.productName;
            const packagingName = stock.packagingName;

            // Combine product name with packaging for clarity
            const displayName = productName && packagingName
                ? `${productName} - ${packagingName}`
                : productName || packagingName || 'منتج غير معروف';

            return (
                <div className="flex items-start gap-3">
                    <div className="h-10 w-10 rounded-lg bg-muted flex items-center justify-center shrink-0 overflow-hidden">
                        <Package className="h-5 w-5 text-muted-foreground" />
                    </div>
                    <div className="flex flex-col">
                        <span className="font-medium text-sm">
                            {displayName}
                        </span>
                        <span className="text-xs text-muted-foreground">
                            {stock.batches?.length || 0} دفعات
                        </span>
                    </div>
                </div>
            );
        }
    },
    {
        accessorKey: 'inventoryName',
        header: 'المخزن',
        cell: ({ row }) => {
            const inventoryName = row.original.inventoryName;
            return (
                <div className="flex items-center gap-2">
                    <Warehouse className="h-4 w-4 text-muted-foreground" />
                    <span className="text-sm font-medium">
                        {inventoryName || <span className="text-muted-foreground text-xs">غير محدد</span>}
                    </span>
                </div>
            );
        }
    },
    {
        accessorKey: 'totalAvailableQuantity',
        header: 'الكمية المتاحة',
        cell: ({ row }) => {
            const qty = row.original.totalAvailableQuantity;
            return (
                <Badge
                    variant={qty > 0 ? "default" : "destructive"}
                    className={qty > 0 ? "bg-green-500/15 text-green-700 hover:bg-green-500/25 border-green-200" : ""}
                >
                    {qty}
                </Badge>
            );
        }
    },
    {
        accessorKey: 'totalQuantity',
        header: 'إجمالي الكمية',
        cell: ({ row }) => (
            <span className="text-sm font-medium text-muted-foreground">
                {row.original.totalQuantity}
            </span>
        )
    },
    {
        accessorKey: 'insertDate',
        header: 'تاريخ الإضافة',
        cell: ({ row }) => {
            const date = row.original.insertDate;
            if (!date) return <span className="text-muted-foreground text-sm">-</span>;

            return (
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{formatDate(date)}</span>
                </div>
            );
        }
    }
];

