import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/app/components/ui/badge';
import { Button } from '@/app/components/ui/button';
import { MoreHorizontal, Package, Barcode as BarcodeIcon, Calendar } from 'lucide-react';
import { ProductWithPackagingsDto, ProductStatus } from '../lib/types';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/app/components/ui/dropdown-menu';
import {
    Tooltip,
    TooltipContent,
    TooltipProvider,
    TooltipTrigger,
} from '@/app/components/ui/tooltip';

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
        accessorKey: 'name',
        header: 'اسم المنتج',
        cell: ({ row }) => {
            const product = row.original;
            return (
                <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-lg bg-muted flex items-center justify-center shrink-0 overflow-hidden">
                        {product.imageUrls && product.imageUrls.length > 0 ? (
                            <img src={product.imageUrls[0]} alt={product.name} className="h-full w-full object-cover" />
                        ) : (
                            <Package className="h-5 w-5 text-muted-foreground" />
                        )}
                    </div>
                    <div className="flex flex-col">
                        <span className="font-medium text-sm">{product.name}</span>
                        <span className="text-xs text-muted-foreground line-clamp-1">
                            {product.packagings?.length || 0} وحدات بيع
                        </span>
                    </div>
                </div>
            )
        },
    },
    {
        id: 'barcode',
        header: 'الباركود',
        cell: ({ row }) => {
            const packagings = row.original.packagings;
            if (!packagings || packagings.length === 0) {
                return <span className="text-muted-foreground text-sm">-</span>;
            }

            const firstBarcode = packagings[0].barcode;
            if (!firstBarcode?.value) {
                return <span className="text-muted-foreground text-sm">-</span>;
            }

            return (
                <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger asChild>
                            <div className="flex items-center gap-2 cursor-pointer">
                                <BarcodeIcon className="h-4 w-4 text-muted-foreground" />
                                <span className="text-sm font-mono">{firstBarcode.value}</span>
                            </div>
                        </TooltipTrigger>
                        <TooltipContent>
                            <p>انقر للنسخ</p>
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider>
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
        id: 'packagings',
        header: 'وحدات البيع',
        cell: ({ row }) => {
            const packagings = row.original.packagings;
            if (!packagings || packagings.length === 0) {
                return <span className="text-muted-foreground text-sm">لا توجد وحدات</span>;
            }

            return (
                <div className="flex flex-col gap-1 max-w-[200px]">
                    {packagings.slice(0, 2).map((pkg) => (
                        <div key={pkg.id} className="flex items-center justify-between gap-2 text-xs">
                            <span className="truncate">{pkg.name}</span>
                            <span className="font-medium whitespace-nowrap">
                                {pkg.sellingPrice.amount.toLocaleString()} {pkg.sellingPrice.currency}
                            </span>
                        </div>
                    ))}
                    {packagings.length > 2 && (
                        <span className="text-xs text-muted-foreground">
                            +{packagings.length - 2} وحدات أخرى
                        </span>
                    )}
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
