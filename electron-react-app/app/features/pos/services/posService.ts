import { httpService } from '@/app/lib/config/http';
import { ApiResponse, PagedResult } from '@/app/lib/types/global';
import { 
    PosProductDto, 
    PosProductFilter, 
    CreateSaleRequest, 
    CompleteSaleRequest, 
    SaleDto,
    CompletedSaleDto,
    CreateAndCompleteSaleRequest
} from '../lib/types';

export const posService = {
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

    // Create and complete sale in one go (for local cart approach)
    createAndCompleteSale: async (data: CreateAndCompleteSaleRequest): Promise<CompletedSaleDto> => {
        // Step 1: Create sale (backend returns just the Guid/string saleId)
        const createResponse = await httpService.getAxiosInstance().post<ApiResponse<string>>(
            '/api/Pos/sales',
            {
                inventoryId: data.inventoryId,
                items: data.items,
                notes: data.notes
            } as CreateSaleRequest
        );
        
        const saleId = createResponse.data.data; // This is just a string (Guid)
        
        // Step 2: Immediately complete it
        const completeResponse = await httpService.getAxiosInstance().post<ApiResponse<CompletedSaleDto>>(
            `/api/Pos/sales/${saleId}/complete`,
            {
                inventoryId: data.inventoryId,
                amountPaid: data.amountPaid
            } as CompleteSaleRequest
        );
        
        return completeResponse.data.data;
    },
};
