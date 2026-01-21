using API.Models;
using Application.Inventory.Commands.CreateInventoryOperation;
using Application.Inventory.Queries.GetInventoryOperationById;
using Application.Inventory.Queries.GetMyInventoryOperations;
using Application.Inventory.Queries.GetMyInventories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryOperationController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryOperationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new inventory operation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateInventoryOperation([FromBody] CreateInventoryOperationCommand command)
    {
        var operationId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(operationId, "تم إنشاء عملية المخزون بنجاح");
        return CreatedAtAction(nameof(GetInventoryOperationById), new { id = operationId }, response);
    }

    /// <summary>
    /// Get inventory operation by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetInventoryOperationById(Guid id)
    {
        var operation = await _mediator.Send(new GetInventoryOperationByIdQuery { OperationId = id });
        var response = ApiResponse<object>.SuccessResponse(operation, "تم جلب بيانات عملية المخزون بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my inventory operations (for current user's organization)
    /// </summary>
    [HttpGet("my")]
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
    /// Get my inventories (storage locations for current user's organization)
    /// </summary>
    [HttpGet("inventories/my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyInventories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var inventories = await _mediator.Send(new GetMyInventoriesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var response = ApiResponse<object>.SuccessResponse(inventories, "تم جلب قائمة المواقع التخزينية بنجاح");
        return Ok(response);
    }
}
