import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { organizationService } from '../services/organizationService';
import type { CreateOrganizationRequest } from '../lib/types';

/**
 * Hook to get current user's organizations
 */
export function useMyOrganizations() {
    return useQuery({
        queryKey: ['my-organizations'],
        queryFn: async () => {
            const result = await organizationService.getMyOrganizations();
            if (result.success && result.data) {
                return result.data;
            }
            return [];
        },
        // Auto-select the first organization if none selected
        staleTime: 5 * 60 * 1000,
    });
}

/**
 * Hook to create a new organization
 */
export function useCreateOrganization() {
    const queryClient = useQueryClient();
    // const { setSelectedOrganization } = useOrganizationStore(); // Not used currently

    return useMutation({
        mutationFn: (data: CreateOrganizationRequest) => organizationService.create(data),
        onSuccess: async (result) => {
            if (result.success && result.data) {
                // result.data is the ID
                toast.success(result.message || 'تم إنشاء المؤسسة بنجاح');

                // Invalidate query
                await queryClient.invalidateQueries({ queryKey: ['my-organizations'] });

                // Optional: Fetch the full org and select it? 
                // We'll let the user select it or the component handle logic
            }
        }
    });
}
