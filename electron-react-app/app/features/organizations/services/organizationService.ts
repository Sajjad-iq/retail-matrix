import { httpService } from '@/app/lib/config/http';
import type { ApiResponse, Organization } from '@/app/lib/types/global';
import type { CreateOrganizationRequest } from '../lib/types';

export const organizationService = {
    async getMyOrganizations(): Promise<ApiResponse<Organization[]>> {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<Organization[]>>('/api/organization/my');
        return response.data;
    },

    async create(data: CreateOrganizationRequest): Promise<ApiResponse<string>> {
        const axios = httpService.getAxiosInstance();
        const response = await axios.post<ApiResponse<string>>('/api/organization', data);
        return response.data;
    },

    async getById(id: string): Promise<ApiResponse<Organization>> {
        const axios = httpService.getAxiosInstance();
        const response = await axios.get<ApiResponse<Organization>>(`/api/organization/${id}`);
        return response.data;
    }
};
