import { ColumnDef } from '@tanstack/react-table';
import { InventoryDto, InventoryType } from '../lib/types';
import { Badge } from '@/app/components/ui/badge';
import { Button } from '@/app/components/ui/button';
import { Pencil, MapPin } from 'lucide-react';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';
import { InventoryDialog } from './InventoryDialog';

const getInventoryTypeLabel = (type: InventoryType): string => {
    const labels: Record<InventoryType, string> = {
        [InventoryType.Warehouse]: 'مستودع',
        [InventoryType.Aisle]: 'ممر',
        [InventoryType.Rack]: 'رف كبير',
        [InventoryType.Shelf]: 'رف',
        [InventoryType.Bin]: 'صندوق',
        [InventoryType.Drawer]: 'درج',
        [InventoryType.Floor]: 'أرضية',
    };
    return labels[type] || 'غير محدد';
};

const getInventoryTypeBadgeVariant = (type: InventoryType): 'default' | 'secondary' | 'outline' => {
    if (type === InventoryType.Warehouse) return 'default';
    if (type === InventoryType.Aisle || type === InventoryType.Floor) return 'secondary';
    return 'outline';
};

export const createColumns = (): ColumnDef<InventoryDto>[] => [
    {
        id: 'name',
        accessorKey: 'name',
        header: 'اسم المخزن',
        cell: ({ getValue, row }) => {
            const name = getValue() as string;
            const type = row.original.type;
            return (
                <div className="flex items-center gap-2">
                    <MapPin className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{name}</span>
                </div>
            );
        },
    },
    {
        id: 'code',
        accessorKey: 'code',
        header: 'الرمز',
        cell: ({ getValue }) => {
            const code = getValue() as string;
            return (
                <span className="font-mono text-sm bg-muted px-2 py-1 rounded">
                    {code}
                </span>
            );
        },
    },
    {
        id: 'type',
        accessorKey: 'type',
        header: 'النوع',
        cell: ({ getValue }) => {
            const type = getValue() as InventoryType;
            return (
                <Badge variant={getInventoryTypeBadgeVariant(type)}>
                    {getInventoryTypeLabel(type)}
                </Badge>
            );
        },
    },
    {
        id: 'isActive',
        accessorKey: 'isActive',
        header: 'الحالة',
        cell: ({ getValue }) => {
            const isActive = getValue() as boolean;
            return (
                <Badge variant={isActive ? 'default' : 'secondary'}>
                    {isActive ? 'نشط' : 'غير نشط'}
                </Badge>
            );
        },
    },
    {
        id: 'insertDate',
        accessorKey: 'insertDate',
        header: 'تاريخ الإنشاء',
        cell: ({ getValue }) => {
            const date = getValue() as string;
            return (
                <span className="text-sm text-muted-foreground">
                    {format(new Date(date), 'dd MMM yyyy', { locale: ar })}
                </span>
            );
        },
    },
    {
        id: 'actions',
        header: 'الإجراءات',
        cell: ({ row }) => {
            const inventory = row.original;
            return (
                <div className="flex items-center justify-center gap-2">
                    <InventoryDialog
                        inventoryId={inventory.id}
                        trigger={
                            <Button variant="outline" size="sm">
                                <Pencil className="h-4 w-4 ml-1" />
                                تعديل
                            </Button>
                        }
                    />
                </div>
            );
        },
    },
];
