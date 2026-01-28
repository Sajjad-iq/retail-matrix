import { useQuery } from '@tanstack/react-query';
import { inventoryLocationService } from '../services/inventoryService';

export function useOperationItems(operationId: string) {
    return useQuery({
        queryKey: ['operation-items', operationId],
        queryFn: () => inventoryLocationService.getOperationItems(operationId),
        enabled: !!operationId,
    });
}
