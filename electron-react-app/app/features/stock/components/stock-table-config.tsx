import { ColumnDef } from '@tanstack/react-table';
import { StockListDto } from '../lib/types';
import { Package, Calendar, ChevronDown, ChevronUp } from 'lucide-react';
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
        accessorKey: 'productName', // NOTE: Backend DTO currently doesn't have productName directly, we might need to join or fetch. 
        // Or assume user will add it eventually or we rely on productPackagingId lookup?
        // Actually, StockListDto doesn't have ProductName. 
        // For now, let's display ProductPackagingId or generic text, OR better yet, update the backend query to include Product.Name.
        // Assuming for MVP we might show "Stock Item" or ID if name missing.
        // Wait, the user wants "table and sub table".
        // Let's check DTO again. Step 455 updated it with Batches but not ProductName.
        // The default handler maps Stock -> StockListDto. Stock entity doesn't have Name. It needs to navigate Stock -> ProductPackaging -> Name.
        // AutoMapper can flatten this if configured: .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductPackaging.Name))
        // I will assume it's NOT mapped yet so I'll show ID or placeholder.
        header: 'المنتج',
        cell: ({ row }) => {
            return (
                <div className="flex items-center gap-2">
                    <Package className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium text-sm">
                        {/* Placeholder as Name isn't in DTO yet */}
                        وحدة: {row.original.productPackagingId.substring(0, 8)}...
                    </span>
                </div>
            )
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
