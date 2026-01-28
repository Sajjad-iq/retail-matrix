import { ColumnDef } from '@tanstack/react-table';
import { Badge } from '@/app/components/ui/badge';
import { Button } from '@/app/components/ui/button';
import { Package, Calendar, ChevronDown, ChevronUp, PackageOpen } from 'lucide-react';
import { formatPrice } from '@/lib/utils';
import { ProductListDto, ProductStatus } from '../lib/types';
import { Trash2, Edit, Copy } from 'lucide-react';
import { ConfirmDialog } from '@/app/components/ui/confirm-dialog';
import { EditProductDialog } from './edit-product-dialog';
import { useDeleteProduct } from '../hooks/useProductActions';
import { toast } from 'sonner';
import { Link } from 'react-router';

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

export const columns: ColumnDef<ProductListDto>[] = [
    {
        id: 'expander',
        header: () => null,
        cell: ({ row }) => {
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
            return <ProductActions product={row.original} />;
        },
    },
];

function ProductActions({ product }: { product: ProductListDto }) {
    const { mutate: deleteProduct, isPending: isDeleting } = useDeleteProduct();

    const handleCopyId = () => {
        navigator.clipboard.writeText(product.id);
        toast.success('تم نسخ المعرف بنجاح');
    };

    const handleDelete = () => {
        deleteProduct(product.id);
    };

    return (
        <div className="flex items-center gap-1 justify-end">
            <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 text-muted-foreground hover:text-primary"
                title="عرض المخزون"
                asChild
            >
                <Link to={`/inventory/stocks?productId=${product.id}`}>
                    <PackageOpen className="h-4 w-4" />
                    <span className="sr-only">عرض المخزون</span>
                </Link>
            </Button>

            <Button
                variant="ghost"
                size="icon"
                className="h-8 w-8 text-muted-foreground hover:text-primary"
                onClick={handleCopyId}
                title="نسخ المعرف"
            >
                <Copy className="h-4 w-4" />
                <span className="sr-only">نسخ المعرف</span>
            </Button>

            <EditProductDialog product={product}>
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-primary"
                    title="تعديل المنتج"
                >
                    <Edit className="h-4 w-4" />
                    <span className="sr-only">تعديل المنتج</span>
                </Button>
            </EditProductDialog>

            <ConfirmDialog
                title="هل أنت متأكد من حذف هذا المنتج؟"
                description={`سيتم حذف المنتج "${product.name}" وجميع وحدات البيع التابعة له. هذا الإجراء لا يمكن التراجع عنه.`}
                confirmText={isDeleting ? "جاري الحذف..." : "حذف"}
                cancelText="إلغاء"
                onConfirm={handleDelete}
                variant="destructive"
            >
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 text-muted-foreground hover:text-destructive"
                    title="حذف المنتج"
                >
                    <Trash2 className="h-4 w-4" />
                    <span className="sr-only">حذف المنتج</span>
                </Button>
            </ConfirmDialog>
        </div>
    );
}
