using API.Models;
using Application.Currencies.Commands.CreateCurrency;
using Application.Currencies.Commands.DeleteCurrency;
using Application.Currencies.Commands.SetBaseCurrency;
using Application.Currencies.Commands.UpdateCurrency;
using Application.Currencies.DTOs;
using Application.Currencies.Queries.GetMyCurrencies;
using Domains.Common.Currency.Enums;
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
public class CurrencyController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrencyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get my currencies
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CurrencyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<CurrencyDto>>>> GetMyCurrencies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] CurrencyStatus? status = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetMyCurrenciesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query);
        var response = ApiResponse<PagedResult<CurrencyDto>>.SuccessResponse(result, "تم جلب قائمة العملات بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Create currency
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCurrency([FromBody] CreateCurrencyCommand command)
    {
        var currencyId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(currencyId, "تم إنشاء العملة بنجاح");
        return CreatedAtAction(nameof(GetMyCurrencies), new { }, response);
    }

    /// <summary>
    /// Update currency
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCurrency(
        Guid id,
        [FromBody] UpdateCurrencyCommand command)
    {
        var commandWithId = command with { Id = id };
        var result = await _mediator.Send(commandWithId);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تحديث العملة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Set as base currency
    /// </summary>
    [HttpPost("{id}/set-base")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> SetBaseCurrency(Guid id)
    {
        var command = new SetBaseCurrencyCommand { CurrencyId = id };
        var result = await _mediator.Send(command);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم تعيين العملة الأساسية بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Delete currency
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCurrency(Guid id)
    {
        var command = new DeleteCurrencyCommand { Id = id };
        var result = await _mediator.Send(command);
        var response = ApiResponse<bool>.SuccessResponse(result, "تم حذف العملة بنجاح");
        return Ok(response);
    }
}
