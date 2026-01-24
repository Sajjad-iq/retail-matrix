import { useNavigate } from 'react-router';
import { CreateOrganizationDialog } from '@/app/features/organizations/components/CreateOrganizationDialog';
import { OrganizationCard } from '@/app/features/organizations/components/OrganizationCard';
import { useMyOrganizations } from '@/app/features/organizations/hooks/useOrganizationActions';
import { useOrganizationStore } from '@/app/stores/organization';
import { Building2, Plus, Loader2 } from 'lucide-react';
import type { Organization } from '@/app/lib/types/global';

export default function SetupOrganizationPage() {
    const navigate = useNavigate();
    const { data: organizations, isLoading } = useMyOrganizations();
    const { setSelectedOrganization } = useOrganizationStore();

    const handleSelectOrganization = (org: Organization) => {
        setSelectedOrganization(org);
        navigate('/', { replace: true });
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-background p-4 text-center">
            <div className="w-full max-w-5xl space-y-8">
                <div className="flex flex-col items-center justify-center space-y-4">
                    <div className="flex h-20 w-20 items-center justify-center rounded-full bg-muted">
                        <Building2 className="h-10 w-10 text-muted-foreground" />
                    </div>

                    <div className="space-y-2">
                        <h1 className="text-3xl font-bold tracking-tighter">مرحباً بك في ريتيل ماتركس</h1>
                        <p className="text-muted-foreground max-w-md mx-auto">
                            للبدء، يرجى اختيار مؤسسة موجودة أو إنشاء مؤسسة جديدة لإدارة متجرك ومخزونك.
                        </p>
                    </div>
                </div>

                {isLoading ? (
                    <div className="flex justify-center py-8">
                        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
                    </div>
                ) : (
                    <div className="w-full space-y-8">
                        {organizations && organizations.length > 0 && (
                            <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-3 text-right" dir="rtl">
                                {organizations.map((org) => (
                                    <OrganizationCard
                                        key={org.id}
                                        organization={org}
                                        onSelect={handleSelectOrganization}
                                    />
                                ))}

                                {/* Create New Organization Card */}
                                <CreateOrganizationDialog>
                                    <div className="flex flex-col items-center justify-center aspect-square border-2 border-dashed border-muted-foreground/25 rounded-xl hover:border-primary/50 hover:bg-muted/50 transition-all cursor-pointer group">
                                        <div className="h-16 w-16 mb-4 rounded-full bg-muted group-hover:bg-primary/10 flex items-center justify-center transition-colors">
                                            <Plus className="h-8 w-8 text-muted-foreground group-hover:text-primary transition-colors" />
                                        </div>
                                        <h3 className="text-lg font-semibold text-foreground">إنشاء مؤسسة جديدة</h3>
                                        <p className="text-sm text-muted-foreground mt-2 max-w-[200px] text-center">
                                            قم بإضافة مؤسسة جديدة لإدارة متجرك
                                        </p>
                                    </div>
                                </CreateOrganizationDialog>
                            </div>
                        )}
                    </div>
                )}

                <p className="text-xs text-muted-foreground">
                    بإنشاء مؤسسة، فإنك توافق على شروط الخدمة وسياسة الخصوصية الخاصة بنا.
                </p>
            </div>
        </div>
    );
}
