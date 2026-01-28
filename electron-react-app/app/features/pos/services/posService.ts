import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult } from '@/app/lib/types/global';
import { 
    PosProductDto, 
    PosProductFilter, 
    CreateSaleRequest, 
    CompleteSaleRequest, 
    SaleDto,
    CompletedSaleDto
} from '../lib/types';

export const posService = {
    // Get or create draft sale for inventory
    getOrCreateDraftSale: async (inventoryId: string) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<SaleDto>>(
            `/api/Pos/inventory/${inventoryId}/draft-sale`
        );
        return response.data.data;
    },

    // Get inventory products for POS
    getInventoryProducts: async (filters: PosProductFilter) => {
        const { inventoryId, ...params } = filters;
        const response = await httpService.getAxiosInstance().get<ApiResponse<PagedResult<PosProductDto>>>(
            `/api/Pos/inventory/${inventoryId}/products`,
            { params }
        );
        return response.data.data;
    },

    // Search product by barcode (for future barcode scanner feature)
    searchByBarcode: async (barcode: string, inventoryId: string) => {
        const response = await httpService.getAxiosInstance().get<ApiResponse<PosProductDto>>(
            `/api/Pos/product/barcode/${barcode}`,
            { params: { inventoryId } }
        );
        return response.data.data;
    },

    // Sale operations
    updateSale: async (saleId: string, data: CreateSaleRequest) => {
        const response = await httpService.getAxiosInstance().put<ApiResponse<boolean>>(
            `/api/Pos/sales/${saleId}`,
            data
        );
        return response.data.data; // Returns boolean
    },

    cancelSale: async (saleId: string) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<boolean>>(
            `/api/Pos/sales/${saleId}/cancel`,
            {}
        );
        return response.data.data; // Returns boolean
    },

    // Complete sale
    completeSale: async (saleId: string, data: CompleteSaleRequest) => {
        const response = await httpService.getAxiosInstance().post<ApiResponse<CompletedSaleDto>>(
            `/api/Pos/sales/${saleId}/complete`,
            data
        );
        return response.data.data; // Returns CompletedSaleDto
    },
};
