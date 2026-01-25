using API.Models;
using Application.Products.Commands.CreateProductPackaging;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProductPackaging;
using Application.Products.Commands.DeleteProductPackaging;
using Application.Products.Queries.GetProductPackagingById;
using Application.Products.Queries.GetProductPackagingByBarcode;
using Application.Products.Queries.GetMyProducts;
using Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[OrganizationAuthorize]
public class ProductPackagingController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductPackagingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new product packaging (sellable item)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateProductPackaging([FromBody] CreateProductPackagingCommand command)
    {
        var packagingId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(packagingId, "تم إنشاء العبوة بنجاح");
        return CreatedAtAction(nameof(GetProductPackagingById), new { id = packagingId }, response);
    }

    /// <summary>
    /// Update existing product
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        await _mediator.Send(command);
        var response = ApiResponse<Unit>.SuccessResponse(Unit.Value, "تم تحديث المنتج بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteProduct(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand { Id = id });
        var response = ApiResponse<Unit>.SuccessResponse(Unit.Value, "تم حذف المنتج بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Update existing product packaging
    /// </summary>
    [HttpPut("packaging")]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Unit>>> UpdateProductPackaging([FromBody] UpdateProductPackagingCommand command)
    {
        await _mediator.Send(command);
        var response = ApiResponse<Unit>.SuccessResponse(Unit.Value, "تم تحديث وحدة البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Delete product packaging
    /// </summary>
    [HttpDelete("packaging/{id}")]
    [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteProductPackaging(Guid id)
    {
        await _mediator.Send(new DeleteProductPackagingCommand { Id = id });
        var response = ApiResponse<Unit>.SuccessResponse(Unit.Value, "تم حذف وحدة البيع بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get product packaging by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetProductPackagingById(Guid id)
    {
        var packaging = await _mediator.Send(new GetProductPackagingByIdQuery { PackagingId = id });
        var response = ApiResponse<object>.SuccessResponse(packaging, "تم جلب بيانات العبوة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get product packaging by barcode (for POS scanning)
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetProductPackagingByBarcode(string barcode)
    {
        var packaging = await _mediator.Send(new GetProductPackagingByBarcodeQuery { Barcode = barcode });
        var response = ApiResponse<object>.SuccessResponse(packaging, "تم جلب بيانات العبوة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my products with their packagings (for current user's organization)
    /// </summary>
    [HttpGet("products/my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _mediator.Send(new GetMyProductsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var response = ApiResponse<object>.SuccessResponse(products, "تم جلب قائمة المنتجات بنجاح");
        return Ok(response);
    }
}
