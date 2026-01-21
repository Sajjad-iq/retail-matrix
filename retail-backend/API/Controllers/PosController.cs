using API.Models;
using Application.POS.Commands.AddCartItem;
using Application.POS.Commands.ApplyCartItemDiscount;
using Application.POS.Commands.CancelPosSession;
using Application.POS.Commands.CompletePosSession;
using Application.POS.Commands.CreatePosSession;
using Application.POS.Commands.RecordPosPayment;
using Application.POS.Commands.RemoveCartItem;
using Application.POS.Commands.UpdateCartItemQuantity;
using Application.POS.DTOs;
using Application.POS.Queries.GetPosCart;
using Application.POS.Queries.GetPosSalesHistory;
using Application.POS.Queries.SearchProductByBarcode;
using Domains.Sales.Enums;
using Domains.Shared.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new POS session (Draft Sale)
    /// </summary>
    [HttpPost("session")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSession([FromBody] CreatePosSessionCommand command)
    {
        var saleId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(saleId, "تم إنشاء جلسة البيع بنجاح");
        return CreatedAtAction(nameof(GetCart), new { saleId }, response);
    }

    /// <summary>
    /// Get current cart state
    /// </summary>
    [HttpGet("session/{saleId}")]
    [ProducesResponseType(typeof(ApiResponse<PosCartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PosCartDto>>> GetCart(Guid saleId)
    {
        var cart = await _mediator.Send(new GetPosCartQuery { SaleId = saleId });
        var response = ApiResponse<PosCartDto>.SuccessResponse(cart, "تم جلب بيانات السلة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("session/{saleId}/items")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddItem(Guid saleId, [FromBody] AddCartItemCommand command)
    {
        var commandWithId = command with { SaleId = saleId };
        var itemId = await _mediator.Send(commandWithId);
        var response = ApiResponse<Guid>.SuccessResponse(itemId, "تم إضافة العنصر بنجاح");
        return CreatedAtAction(nameof(GetCart), new { saleId }, response);
    }

    /// <summary>
    /// Update item quantity
    /// </summary>
    [HttpPut("session/{saleId}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateItemQuantity(
        Guid saleId,
        Guid itemId,
        [FromBody] UpdateCartItemQuantityCommand command)
    {
        var commandWithIds = command with { SaleId = saleId, ItemId = itemId };
        var result = await _mediator.Send(commandWithIds);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تحديث الكمية بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("session/{saleId}/items/{itemId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveItem(Guid saleId, Guid itemId)
    {
        var command = new RemoveCartItemCommand { SaleId = saleId, ItemId = itemId };
        var result = await _mediator.Send(command);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم حذف العنصر بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Apply discount to item
    /// </summary>
    [HttpPut("session/{saleId}/items/{itemId}/discount")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> ApplyDiscount(
        Guid saleId,
        Guid itemId,
        [FromBody] ApplyCartItemDiscountCommand command)
    {
        var commandWithIds = command with { SaleId = saleId, ItemId = itemId };
        var result = await _mediator.Send(commandWithIds);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تطبيق الخصم بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Record payment
    /// </summary>
    [HttpPost("session/{saleId}/payment")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> RecordPayment(
        Guid saleId,
        [FromBody] RecordPosPaymentCommand command)
    {
        var commandWithId = command with { SaleId = saleId };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تسجيل الدفع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Complete sale (checkout)
    /// </summary>
    [HttpPost("session/{saleId}/complete")]
    [ProducesResponseType(typeof(ApiResponse<CompletedSaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CompletedSaleDto>>> CompleteSale(
        Guid saleId,
        [FromBody] CompletePosSessionCommand command)
    {
        var commandWithId = command with { SaleId = saleId };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<CompletedSaleDto>.SuccessResponse(result, "تم إتمام البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Cancel sale
    /// </summary>
    [HttpPost("session/{saleId}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> CancelSale(Guid saleId)
    {
        var command = new CancelPosSessionCommand { SaleId = saleId };
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
