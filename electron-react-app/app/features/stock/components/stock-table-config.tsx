import { ColumnDef } from '@tanstack/react-table';
import { StockListDto } from '../lib/types';
import { Package, ChevronDown, ChevronUp } from 'lucide-react';
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
            return (
                <div className="flex items-center justify-center">
                    <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => row.toggleExpanded()}
                        className="h-8 w-8 p-0"
                        disabled={!hasBatches}
                    >
                        {row.getIsExpanded() ? (
                            <ChevronUp className="h-4 w-4" />
                        ) : (
                            <ChevronDown className={`h-4 w-4 ${!hasBatches ? 'opacity-30' : ''}`} />
                        )}
                    </Button>
                </div>
            );
        },
    },
    {
        accessorKey: 'productName',
        header: 'المنتج',
        cell: ({ row }) => {
            const productName = row.original.productName;
            const packagingName = row.original.packagingName;
            
            return (
                <div className="flex items-center gap-2">
                    <Package className="h-4 w-4 text-muted-foreground" />
                    <div className="flex flex-col">
                        <span className="font-medium text-sm">
                            {productName || 'منتج غير معروف'}
                        </span>
                        {packagingName && (
                            <span className="text-xs text-muted-foreground">
                                {packagingName}
                            </span>
                        )}
                    </div>
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
                <Badge variant={qty > 0 ? "outline" : "destructive"}>
                    {qty}
                </Badge>
            );
        }
    },
    {
        accessorKey: 'totalQuantity', // Physical quantity
        header: 'إجمالي الكمية',
        cell: ({ row }) => <span className="text-muted-foreground">{row.original.totalQuantity}</span>
    },
    {
        accessorKey: 'insertDate',
        header: 'تاريخ الإضافة',
        cell: ({ row }) => {
            return <span className="text-xs text-muted-foreground">{formatDate(row.original.insertDate)}</span>
        }
    }
];
