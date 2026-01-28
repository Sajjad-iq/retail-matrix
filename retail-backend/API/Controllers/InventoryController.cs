using API.Models;
using Application.Inventory.Commands.CreateInventory;
using Application.Inventory.Commands.CreateInventoryOperation;
using Application.Inventory.Commands.UpdateInventory;
using Application.Inventory.Queries.GetInventoryById;
using Application.Inventory.Queries.GetInventoryOperationById;
using Application.Inventory.Queries.GetMyInventories;
using Application.Inventory.Queries.GetMyInventoryOperations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Filters;

namespace API.Controllers;

/// <summary>
/// Controller for managing inventory locations (warehouses, aisles, shelves, bins) and inventory operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[OrganizationAuthorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Inventory Locations

    /// <summary>
    /// Create a new inventory location
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateInventory([FromBody] CreateInventoryCommand command)
    {
        var inventoryId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(inventoryId, "تم إنشاء المخزن بنجاح");
        return CreatedAtAction(nameof(GetInventoryById), new { id = inventoryId }, response);
    }

    /// <summary>
    /// Update existing inventory location
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateInventory(Guid id, [FromBody] UpdateInventoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiErrorResponse.Create("معرف المخزن غير متطابق"));
        }

        var result = await _mediator.Send(command);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تحديث المخزن بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get inventory by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetInventoryById(Guid id)
    {
        var inventory = await _mediator.Send(new GetInventoryByIdQuery(id));
        var response = ApiResponse<object>.SuccessResponse(inventory, "تم جلب بيانات المخزن بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get all inventories for current organization with pagination
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyInventories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var inventories = await _mediator.Send(new GetMyInventoriesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var response = ApiResponse<object>.SuccessResponse(inventories, "تم جلب قائمة المخازن بنجاح");
        return Ok(response);
    }

    #endregion

    #region Inventory Operations

    /// <summary>
    /// Create a new inventory operation (transfer, purchase, sale, adjustment, etc.)
    /// </summary>
    [HttpPost("operations")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateInventoryOperation([FromBody] CreateInventoryOperationCommand command)
    {
        var operationId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(operationId, "تم إنشاء عملية المخزون بنجاح");
        return CreatedAtAction(nameof(GetInventoryOperationById), new { id = operationId }, response);
    }

    /// <summary>
    /// Get inventory operation by ID with all details
    /// </summary>
    [HttpGet("operations/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetInventoryOperationById(Guid id)
    {
        var operation = await _mediator.Send(new GetInventoryOperationByIdQuery { OperationId = id });
        var response = ApiResponse<object>.SuccessResponse(operation, "تم جلب بيانات عملية المخزون بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my inventory operations (for current user's organization) with pagination
    /// </summary>
    [HttpGet("operations/my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyInventoryOperations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var operations = await _mediator.Send(new GetMyInventoryOperationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var response = ApiResponse<object>.SuccessResponse(operations, "تم جلب قائمة عمليات المخزون بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get operation items for a specific inventory operation
    /// </summary>
    [HttpGet("operations/{id}/items")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetInventoryOperationItems(Guid id)
    {
        var items = await _mediator.Send(new Application.Inventory.Queries.GetInventoryOperationItems.GetInventoryOperationItemsQuery { OperationId = id });
        var response = ApiResponse<object>.SuccessResponse(items, "تم جلب عناصر العملية بنجاح");
        return Ok(response);
    }

    #endregion
}
