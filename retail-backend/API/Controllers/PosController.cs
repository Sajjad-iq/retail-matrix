using API.Models;
using Application.POS.Commands.CancelSale;
using Application.POS.Commands.CompleteSale;
using Application.POS.Commands.CreateSale;
using Application.POS.Commands.UpdateSale;
using Application.POS.DTOs;
using Application.POS.Queries.GetPosSalesHistory;
using Application.POS.Queries.GetSale;
using Application.POS.Queries.SearchProductByBarcode;
using Domains.Sales.Enums;
using Domains.Shared.Base;
using Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[OrganizationAuthorize]
public class PosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a draft sale with items
    /// </summary>
    [HttpPost("sales")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSale([FromBody] CreateSaleCommand command)
    {
        var saleId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(saleId, "تم إنشاء البيع بنجاح");
        return CreatedAtAction(nameof(GetSale), new { saleId }, response);
    }

    /// <summary>
    /// Get sale details
    /// </summary>
    [HttpGet("sales/{saleId}")]
    [ProducesResponseType(typeof(ApiResponse<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SaleDto>>> GetSale(Guid saleId)
    {
        var sale = await _mediator.Send(new GetSaleQuery { SaleId = saleId });
        var response = ApiResponse<SaleDto>.SuccessResponse(sale, "تم جلب بيانات البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Update sale items
    /// </summary>
    [HttpPut("sales/{saleId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateSale(
        Guid saleId,
        [FromBody] UpdateSaleCommand command)
    {
        var commandWithId = command with { SaleId = saleId };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تحديث البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Complete sale with payment
    /// </summary>
    [HttpPost("sales/{saleId}/complete")]
    [ProducesResponseType(typeof(ApiResponse<CompletedSaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CompletedSaleDto>>> CompleteSale(
        Guid saleId,
        [FromBody] CompleteSaleCommand command)
    {
        var commandWithId = command with { SaleId = saleId };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<CompletedSaleDto>.SuccessResponse(result, "تم إتمام البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Cancel sale
    /// </summary>
    [HttpPost("sales/{saleId}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> CancelSale(Guid saleId)
    {
        var command = new CancelSaleCommand { SaleId = saleId };
        var result = await _mediator.Send(command);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم إلغاء البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get sales history
    /// </summary>
    [HttpGet("sales")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PosSaleListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<PosSaleListDto>>>> GetSalesHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] SaleStatus? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = new GetPosSalesHistoryQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query);
        var response = ApiResponse<PagedResult<PosSaleListDto>>.SuccessResponse(result, "تم جلب سجل المبيعات بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Search product by barcode
    /// </summary>
    [HttpGet("product/barcode/{barcode}")]
    [ProducesResponseType(typeof(ApiResponse<PosProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PosProductDto>>> SearchByBarcode(
        string barcode,
        [FromQuery] Guid inventoryId)
    {
        var query = new SearchProductByBarcodeQuery
        {
            Barcode = barcode,
            InventoryId = inventoryId
        };

        var result = await _mediator.Send(query);
        if (result == null)
        {
            return NotFound(ApiResponse<PosProductDto>.FailureResponse("المنتج غير موجود"));
        }

        var response = ApiResponse<PosProductDto>.SuccessResponse(result, "تم العثور على المنتج");
        return Ok(response);
    }
}
