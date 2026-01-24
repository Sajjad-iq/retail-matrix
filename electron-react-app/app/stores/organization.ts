import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { Organization } from '@/app/lib/types/global';

interface OrganizationState {
    selectedOrganizationId: string | null;
    selectedOrganization: Organization | null;
    setSelectedOrganization: (org: Organization) => void;
    clearSelectedOrganization: () => void;
}

export const useOrganizationStore = create<OrganizationState>()(
    persist(
        (set) => ({
            selectedOrganizationId: null,
            selectedOrganization: null,

            setSelectedOrganization: (org) => {
                set({
                    selectedOrganization: org,
                    selectedOrganizationId: org.id
                });
            },

            clearSelectedOrganization: () => {
                set({
                    selectedOrganization: null,
                    selectedOrganizationId: null
                });
            }
        }),
        {
            name: 'organization-storage',
        }
    )
);
