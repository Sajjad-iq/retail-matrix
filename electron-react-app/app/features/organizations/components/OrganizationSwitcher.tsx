import { ChevronsUpDown, Plus } from 'lucide-react';
import { useEffect } from 'react';
import { useOrganizationStore } from '@/app/stores/organization';
import { useMyOrganizations } from '../hooks/useOrganizationActions';
import { CreateOrganizationDialog } from './CreateOrganizationDialog';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuShortcut,
    DropdownMenuTrigger,
} from '@/app/components/ui/dropdown-menu';
import {
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
    useSidebar,
} from '@/app/components/ui/sidebar';

export function OrganizationSwitcher() {
    const { isMobile } = useSidebar();
    const { data: organizations } = useMyOrganizations();
    const { selectedOrganization, setSelectedOrganization } = useOrganizationStore();

    // Auto-select first organization if none selected and data loaded
    useEffect(() => {
        if (!selectedOrganization && organizations && organizations.length > 0) {
            setSelectedOrganization(organizations[0]);
        }
    }, [organizations, selectedOrganization, setSelectedOrganization]);

    return (
        <SidebarMenu>
            <SidebarMenuItem>
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <SidebarMenuButton
                            size="lg"
                            className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
                        >
                            <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                                {selectedOrganization ? (
                                    <span className="font-bold text-lg">
                                        {selectedOrganization.name.charAt(0).toUpperCase()}
                                    </span>
                                ) : (
                                    <span className="font-bold">?</span>
                                )}
                            </div>
                            <div className="grid flex-1 text-left text-sm leading-tight">
                                <span className="truncate font-semibold">
                                    {selectedOrganization?.name || 'اختر المؤسسة'}
                                </span>
                                <span className="truncate text-xs">
                                    {selectedOrganization?.domain || 'لم يتم تحديد مؤسسة'}
                                </span>
                            </div>
                            <ChevronsUpDown className="ml-auto" />
                        </SidebarMenuButton>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent
                        className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
                        align="start"
                        side={isMobile ? 'bottom' : 'right'}
                        sideOffset={4}
                    >
                        <DropdownMenuLabel className="text-xs text-muted-foreground">
                            المؤسسات
                        </DropdownMenuLabel>
                        {organizations?.map((org) => (
                            <DropdownMenuItem
                                key={org.id}
                                onClick={() => setSelectedOrganization(org)}
                                className="gap-2 p-2"
                            >
                                <div className="flex size-6 items-center justify-center rounded-sm border">
                                    {org.name.charAt(0)}
                                </div>
                                {org.name}
                                <DropdownMenuShortcut>⌘{org.name.charAt(0)}</DropdownMenuShortcut>
                            </DropdownMenuItem>
                        ))}
                        <DropdownMenuSeparator />
                        <CreateOrganizationDialog>
                            <DropdownMenuItem onSelect={(e) => e.preventDefault()}>
                                <div className="flex size-6 items-center justify-center rounded-md border bg-background">
                                    <Plus className="size-4" />
                                </div>
                                <div className="font-medium text-muted-foreground">إضافة مؤسسة</div>
                            </DropdownMenuItem>
                        </CreateOrganizationDialog>
                    </DropdownMenuContent>
                </DropdownMenu>
            </SidebarMenuItem>
        </SidebarMenu>
    );
}
