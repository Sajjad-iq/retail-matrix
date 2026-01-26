export enum InventoryType {
    Warehouse = 0,
    Aisle = 1,
    Rack = 2,
    Shelf = 3,
    Bin = 4,
    Drawer = 5,
    Floor = 6,
}

export enum InventoryOperationType {
    Purchase = 0,
    Sale = 1,
    Transfer = 2,
    Stocktake = 3,
    Adjustment = 4,
    Return = 5,
    Damage = 6,
    Expired = 7
}

export interface InventoryDto {
    id: string;
    name: string;
    code: string;
    type: InventoryType;
    organizationId: string;
    parentId?: string;
    isActive: boolean;
    insertDate: string;
}

export interface CreateInventoryRequest {
    name: string;
    code: string;
    type: InventoryType;
    parentId?: string;
}

export interface UpdateInventoryRequest {
    id: string;
    name: string;
    code: string;
    type: InventoryType;
    parentId?: string;
    isActive: boolean;
}

export interface InventoryOperationDto {
    id: string;
    operationType: InventoryOperationType;
    operationNumber: string;
    sourceInventoryId?: string;
    destinationInventoryId?: string;
    notes?: string;
    insertDate: string;
    createdBy: string;
}

export interface CreateInventoryOperationRequest {
    operationType: InventoryOperationType;
    sourceInventoryId?: string;
    destinationInventoryId?: string;
    notes?: string;
}
