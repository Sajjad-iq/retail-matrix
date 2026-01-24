import { Badge } from '@/app/components/ui/badge';
import { Globe, Phone, Check } from 'lucide-react';
import { OrganizationStatus, type Organization } from '@/app/lib/types/global';
import { cn } from '@/lib/utils';

interface OrganizationCardProps {
    organization: Organization;
    isSelected?: boolean;
    onSelect: (org: Organization) => void;
}

export function OrganizationCard({ organization, isSelected, onSelect }: OrganizationCardProps) {
    const getStatusLabel = (status: OrganizationStatus) => {
        switch (status) {
            case OrganizationStatus.Active: return 'نشط';
            case OrganizationStatus.Suspended: return 'معلق';
            case OrganizationStatus.Pending: return 'قيد الانتظار';
            case OrganizationStatus.Closed: return 'مغلق';
            default: return 'غير معروف';
        }
    };

    const getStatusVariant = (status: OrganizationStatus) => {
        switch (status) {
            case OrganizationStatus.Active: return 'default';
            case OrganizationStatus.Suspended: return 'destructive';
            case OrganizationStatus.Pending: return 'secondary';
            case OrganizationStatus.Closed: return 'outline';
            default: return 'outline';
        }
    };

    return (
        <div
            className={cn(
                "bg-card rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 cursor-pointer p-6 border border-border flex flex-col relative overflow-hidden group aspect-square min-w-0",
                isSelected && "border-primary ring-2 ring-primary ring-opacity-50 bg-primary/5"
            )}
            onClick={() => onSelect(organization)}
            dir="rtl"
        >
            {isSelected && (
                <div className="absolute left-2 top-2 z-10 text-primary">
                    <div className="bg-primary text-primary-foreground rounded-full p-1 shadow-sm">
                        <Check className="h-4 w-4" />
                    </div>
                </div>
            )}

            <div className="flex items-start justify-between mb-6">
                <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl flex items-center justify-center text-white font-bold text-2xl shadow-inner">
                    {organization.logoUrl ? (
                        <img src={organization.logoUrl} alt={organization.name} className="h-full w-full rounded-xl object-cover" />
                    ) : (
                        organization.name.charAt(0).toUpperCase()
                    )}
                </div>
                <Badge variant={getStatusVariant(organization.status)} className="text-xs h-fit">
                    {getStatusLabel(organization.status)}
                </Badge>
            </div>

            <h3 className="text-xl font-semibold text-foreground mb-3 truncate" title={organization.name}>{organization.name}</h3>
            <p className="text-muted-foreground mb-4 flex-grow text-sm line-clamp-2">
                {organization.description || 'لا يوجد وصف متاح'}
            </p>

            <div className="space-y-2 text-sm text-muted-foreground w-full">
                <div className="flex items-center w-full" >
                    <Globe className="w-4 h-4 me-3 text-primary/70" />
                    <span className="text-left">{organization.domain}</span>
                </div>
                {organization.phone && (
                    <div className="flex items-center w-full" >
                        <Phone className="w-4 h-4 me-3 text-primary/70" />
                        <span className="text-left">{organization.phone}</span>
                    </div>
                )}
            </div>
        </div>
    );
}
