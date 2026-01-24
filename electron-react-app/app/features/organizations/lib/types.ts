export interface CreateOrganizationRequest {
    name: string;
    domain: string;
    phone: string;
    description?: string;
    address?: string;
    logoUrl?: string;
}

export interface UpdateOrganizationRequest {
    id: string;
    name: string;
    description?: string;
    address?: string;
    phone?: string;
    logoUrl?: string;
}
