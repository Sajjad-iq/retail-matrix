import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/app/components/ui/badge';
import { Button } from '@/app/components/ui/button';
import { MoreHorizontal, Package, Calendar, ChevronDown, ChevronUp } from 'lucide-react';
import { formatPrice } from '@/lib/utils';
import { ProductWithPackagingsDto, ProductStatus } from '../lib/types';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/app/components/ui/dropdown-menu';

const getStatusBadge = (status: ProductStatus) => {
    switch (status) {
        case ProductStatus.Active:
            return <Badge variant="default" className="bg-green-500/15 text-green-700 hover:bg-green-500/25 border-green-200">نشط</Badge>;
        case ProductStatus.Inactive:
            return <Badge variant="secondary">غير نشط</Badge>;
        case ProductStatus.OutOfStock:
            return <Badge variant="destructive">نفذت الكمية</Badge>;
        case ProductStatus.Discontinued:
            return <Badge variant="outline">متوقف</Badge>;
        default:
            return <Badge variant="secondary">غير معروف</Badge>;
    }
};

const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('ar-SA', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    }).format(date);
};

export const columns: ColumnDef<ProductWithPackagingsDto>[] = [
    {
        id: 'expander',
        header: () => null,
        cell: ({ row }) => {
            const hasPackagings = row.original.packagings && row.original.packagings.length > 0;

            if (!hasPackagings) {
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
        accessorKey: 'name',
        header: 'اسم المنتج',
        cell: ({ row }) => {
            const product = row.original;
            return (
                <div className="flex items-start gap-3">
                    <div className="h-10 w-10 rounded-lg bg-muted flex items-center justify-center shrink-0 overflow-hidden">
                        {product.imageUrls && product.imageUrls.length > 0 ? (
                            <img src={product.imageUrls[0]} alt={product.name} className="h-full w-full object-cover" />
                        ) : (
                            <Package className="h-5 w-5 text-muted-foreground" />
                        )}
                    </div>
                    <div className="flex flex-col">
                        <span className="font-medium text-sm">{product.name}</span>
                        <span className="text-xs text-muted-foreground">
                            {product.packagings?.length || 0} وحدات بيع
                        </span>
                    </div>
                </div>
            )
        },
    },
    {
        accessorKey: 'categoryName',
        header: 'الفئة',
        cell: ({ row }) => {
            const categoryName = row.original.categoryName;
            return (
                <span className="text-sm font-medium">
                    {categoryName || <span className="text-muted-foreground text-xs">غير مصنف</span>}
                </span>
            );
        },
    },
    {
        accessorKey: 'status',
        header: 'الحالة',
        cell: ({ row }) => {
            const status = row.getValue('status') as ProductStatus;
            return getStatusBadge(status);
        },
    },
    {
        id: 'sellingPrice',
        header: 'سعر البيع',
        cell: ({ row }) => {
            const packagings = row.original.packagings;
            if (!packagings || packagings.length === 0) {
                return <span className="text-muted-foreground text-sm">-</span>;
            }

            const firstUnit = packagings[0];
            if (packagings.length === 1) {
                return (
                    <div className="font-medium text-sm">
                        {formatPrice(firstUnit.sellingPrice.amount, firstUnit.sellingPrice.currency)}
                    </div>
                );
            }

            // Show price range if multiple packagings
            const prices = packagings.map(p => p.sellingPrice.amount);
            const minPrice = Math.min(...prices);
            const maxPrice = Math.max(...prices);

            return (
                <div className="text-sm">
                    <span className="font-medium">{minPrice.toLocaleString()}</span>
                    <span className="text-muted-foreground mx-1">-</span>
                    <span className="font-medium">{maxPrice.toLocaleString()}</span>
                    <span className="text-muted-foreground mr-1">{firstUnit.sellingPrice.currency}</span>
                </div>
            );
        },
    },
    {
        accessorKey: 'insertDate',
        header: 'تاريخ الإضافة',
        cell: ({ row }) => {
            const date = row.getValue('insertDate') as string;
            if (!date) return <span className="text-muted-foreground text-sm">-</span>;

            return (
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{formatDate(date)}</span>
                </div>
            );
        },
    },
    {
        id: 'actions',
        cell: ({ row }) => {
            const product = row.original;

            return (
                <DropdownMenu dir="rtl">
                    <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                            <span className="sr-only">Open menu</span>
                            <MoreHorizontal className="h-4 w-4" />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                        <DropdownMenuLabel>الإجراءات</DropdownMenuLabel>
                        <DropdownMenuItem
                            onClick={() => navigator.clipboard.writeText(product.id)}
                        >
                            نسخ المعرف
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem>عرض التفاصيل</DropdownMenuItem>
                        <DropdownMenuItem>تعديل المنتج</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
