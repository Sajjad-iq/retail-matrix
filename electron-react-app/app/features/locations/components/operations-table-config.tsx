import { ColumnDef } from '@tanstack/react-table';
import { InventoryOperationDto, InventoryOperationType } from '../lib/types';
import { Badge } from '@/app/components/ui/badge';
import { Calendar } from 'lucide-react';

export const getOperationTypeLabel = (type: InventoryOperationType) => {
    switch (type) {
        case InventoryOperationType.Purchase: return 'شراء';
        case InventoryOperationType.Sale: return 'بيع';
        case InventoryOperationType.Transfer: return 'نقل';
        case InventoryOperationType.Stocktake: return 'جرد';
        case InventoryOperationType.Adjustment: return 'تعديل';
        case InventoryOperationType.Return: return 'إرجاع';
        case InventoryOperationType.Damage: return 'تالف';
        case InventoryOperationType.Expired: return 'منتهي الصلاحية';
        default: return '-';
    }
};

const formatDate = (dateString: string | null | undefined) => {
    if (!dateString) return '-';

    const date = new Date(dateString);

    // Check if date is valid
    if (isNaN(date.getTime())) return '-';

    return new Intl.DateTimeFormat('ar-SA', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    }).format(date);
};

export const operationColumns: ColumnDef<InventoryOperationDto>[] = [
    {
        id: 'expander',
        header: () => null,
        cell: () => null,
    },
    {
        accessorKey: 'operationNumber',
        header: 'رقم العملية',
        cell: ({ row }) => <span className="font-mono text-sm">{row.original.operationNumber}</span>
    },
    {
        accessorKey: 'operationType',
        header: 'نوع العملية',
        cell: ({ row }) => <Badge variant="secondary">{getOperationTypeLabel(row.original.operationType)}</Badge>
    },
    {
        accessorKey: 'operationDate',
        header: 'تاريخ العملية',
        cell: ({ row }) => (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Calendar className="h-4 w-4" />
                <span>{formatDate(row.original.operationDate)}</span>
            </div>
        )
    },
    {
        accessorKey: 'status',
        header: 'الحالة',
        cell: ({ row }) => {
            const statusLabels: Record<number, { label: string; variant: 'default' | 'secondary' | 'destructive' | 'outline' }> = {
                0: { label: 'مسودة', variant: 'outline' },
                1: { label: 'مكتمل', variant: 'default' },
                2: { label: 'ملغي', variant: 'destructive' },
            };
            const status = statusLabels[row.original.status] || { label: 'غير معروف', variant: 'secondary' };
            return <Badge variant={status.variant}>{status.label}</Badge>;
        }
    },
    {
        id: 'inventory',
        header: 'الموقع',
        cell: ({ row }) => {
            const { sourceInventoryName, destinationInventoryName, operationType } = row.original;

            // For transfers, show both source and destination
            if (operationType === InventoryOperationType.Transfer && sourceInventoryName && destinationInventoryName) {
                return (
                    <div className="flex flex-col gap-1 text-xs">
                        <div className="flex items-center gap-1">
                            <span className="text-muted-foreground">من:</span>
                            <span className="font-medium">{sourceInventoryName}</span>
                        </div>
                        <div className="flex items-center gap-1">
                            <span className="text-muted-foreground">إلى:</span>
                            <span className="font-medium">{destinationInventoryName}</span>
                        </div>
                    </div>
                );
            }

            // For other operations, show source or destination
            const inventoryName = sourceInventoryName || destinationInventoryName;
            return inventoryName ? (
                <span className="text-sm">{inventoryName}</span>
            ) : (
                <span className="text-sm text-muted-foreground">-</span>
            );
        }
    },
    {
        accessorKey: 'userName',
        header: 'المستخدم',
        cell: ({ row }) => (
            <div className="flex items-center gap-2">
                <div className="h-8 w-8 rounded-full bg-primary/10 flex items-center justify-center text-primary font-medium text-sm">
                    {row.original.userAvatar ? (
                        <img
                            src={row.original.userAvatar}
                            alt={row.original.userName}
                            className="h-8 w-8 rounded-full object-cover"
                        />
                    ) : (
                        row.original.userName.charAt(0).toUpperCase()
                    )}
                </div>
                <span className="text-sm font-medium">{row.original.userName}</span>
            </div>
        )
    },
    {
        accessorKey: 'notes',
        header: 'ملاحظات',
        cell: ({ row }) => {
            const notes = row.original.notes;
            return <span className="text-sm text-muted-foreground">{notes || '-'}</span>
        }
    },
    {
        accessorKey: 'insertDate',
        header: 'تاريخ الإنشاء',
        cell: ({ row }) => (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Calendar className="h-4 w-4" />
                <span>{formatDate(row.original.insertDate)}</span>
            </div>
        )
    }
];
