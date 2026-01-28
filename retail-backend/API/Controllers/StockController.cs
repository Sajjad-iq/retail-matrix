using API.Models;
using Application.Stocks.Commands.AddStockBatch;
using Application.Stocks.Commands.AdjustStockQuantity;
using Application.Stocks.Commands.CreateStock;
using Application.Stocks.Queries.GetMyBatches;
using Application.Stocks.Queries.GetMyStocks;
using Application.Stocks.Queries.GetStockById;
using Domains.Stocks.Enums;
using Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockStatus = Application.Stocks.Queries.GetMyStocks.StockStatus;
using BatchStatus = Application.Stocks.Queries.GetMyBatches.BatchStatus;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[OrganizationAuthorize]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new stock record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateStock([FromBody] CreateStockCommand command)
    {
        var stockId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(stockId, "تم إنشاء المخزون بنجاح");
        return CreatedAtAction(nameof(GetStockById), new { id = stockId }, response);
    }

    /// <summary>
    /// Add a new batch to an existing stock
    /// </summary>
    [HttpPost("{id}/batch")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddStockBatch(Guid id, [FromBody] AddStockBatchCommand command)
    {
        // Ensure the stock ID from route matches the command
        var commandWithId = command with { StockId = id };
        var batchId = await _mediator.Send(commandWithId);
        var response = ApiResponse<Guid>.SuccessResponse(batchId, "تم إضافة الدفعة بنجاح");
        return CreatedAtAction(nameof(GetStockById), new { id }, response);
    }

    /// <summary>
    /// Adjust the quantity of a specific batch
    /// </summary>
    [HttpPut("{id}/adjust")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> AdjustStockQuantity(Guid id, [FromBody] AdjustStockQuantityCommand command)
    {
        // Ensure the stock ID from route matches the command
        var commandWithId = command with { StockId = id };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تعديل الكمية بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get stock by ID with all batches
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetStockById(Guid id)
    {
        var stock = await _mediator.Send(new GetStockByIdQuery { StockId = id });
        var response = ApiResponse<object>.SuccessResponse(stock, "تم جلب بيانات المخزون بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my stocks (for current user's organization) with optional filters
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyStocks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? inventoryId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? productPackagingId = null,
        [FromQuery] string? productName = null,
        [FromQuery] StockStatus stockStatus = StockStatus.All,
        [FromQuery] int? reorderLevel = null)
    {
        var stocks = await _mediator.Send(new GetMyStocksQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            InventoryId = inventoryId,
            ProductId = productId,
            ProductPackagingId = productPackagingId,
            ProductName = productName,
            StockStatus = stockStatus,
            ReorderLevel = reorderLevel
        });
        var response = ApiResponse<object>.SuccessResponse(stocks, "تم جلب قائمة المخزون بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my batches (for current user's organization) with optional filters
    /// </summary>
    [HttpGet("batches/my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyBatches(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] BatchStatus batchStatus = BatchStatus.All,
        [FromQuery] int daysThreshold = 30,
        [FromQuery] StockCondition? condition = null)
    {
        var batches = await _mediator.Send(new GetMyBatchesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            BatchStatus = batchStatus,
            DaysThreshold = daysThreshold,
            Condition = condition
        });
        var response = ApiResponse<object>.SuccessResponse(batches, "تم جلب قائمة الدفعات بنجاح");
        return Ok(response);
    }
}
