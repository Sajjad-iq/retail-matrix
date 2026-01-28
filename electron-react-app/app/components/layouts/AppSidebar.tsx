'use client';

import { Home, Users, Settings, LogOut, Package, ClipboardList, Warehouse, ShoppingCart } from 'lucide-react';
import {
    Sidebar,
    SidebarContent,
    SidebarFooter,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
} from '@/app/components/ui/sidebar';

import { OrganizationSwitcher } from '@/app/features/organizations/components/OrganizationSwitcher';
import { Link } from 'react-router';

const menuItems = [
    {
        title: 'الرئيسية',
        url: '/',
        icon: Home,
    },
    {
        title: 'نقاط البيع',
        url: '/pos',
        icon: ShoppingCart,
    },
    {
        title: 'المنتجات',
        url: '/products',
        icon: Package,
    },
    {
        title: 'المخازن',
        url: '/inventory/locations',
        icon: Warehouse,
    },
    {
        title: 'المخزون',
        url: '/inventory/stocks',
        icon: Package,
    },
    {
        title: 'سجل العمليات',
        url: '/inventory/operations',
        icon: ClipboardList,
    },
    {
        title: 'المستخدمين',
        url: '/users',
        icon: Users,
    },
    {
        title: 'الإعدادات',
        url: '/settings',
        icon: Settings,
    },
];

export function AppSidebar() {

    const handleLogout = async () => { }


    return (
        <Sidebar side="right" variant="sidebar" collapsible="icon">
            <SidebarContent>
                {/* Organization Switcher */}
                <OrganizationSwitcher />

                <SidebarGroup>
                    <SidebarGroupLabel>التطبيق</SidebarGroupLabel>
                    <SidebarGroupContent>
                        <SidebarMenu>
                            {menuItems.map((item) => (
                                <SidebarMenuItem key={item.title}>
                                    <SidebarMenuButton asChild>
                                        <Link to={item.url}>
                                            <item.icon />
                                            <span>{item.title}</span>
                                        </Link>
                                    </SidebarMenuButton>
                                </SidebarMenuItem>
                            ))}
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
            </SidebarContent>

            <SidebarFooter>
                <SidebarMenu>
                    <SidebarMenuItem>
                        <SidebarMenuButton onClick={handleLogout}>
                            <LogOut />
                            <span>تسجيل خروج</span>
                        </SidebarMenuButton>
                    </SidebarMenuItem>
                </SidebarMenu>

            </SidebarFooter>
        </Sidebar>
    );
}
