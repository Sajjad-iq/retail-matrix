using API.Models;
using Application.Organizations.Commands.ChangeOrganizationStatus;
using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.UpdateOrganization;
using Application.Organizations.Commands.UpdateOrganizationDomain;
using Application.Organizations.Queries.GetMyOrganizations;
using Application.Organizations.Queries.GetOrganizationById;
using Domains.Organizations.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new organization
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateOrganization([FromBody] CreateOrganizationCommand command)
    {
        var organizationId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(organizationId, "تم إنشاء المؤسسة بنجاح");
        return CreatedAtAction(nameof(GetOrganizationById), new { id = organizationId }, response);
    }

    /// <summary>
    /// Get organization by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetOrganizationById(Guid id)
    {
        var organization = await _mediator.Send(new GetOrganizationByIdQuery { OrganizationId = id });
        var response = ApiResponse<object>.SuccessResponse(organization, "تم جلب بيانات المؤسسة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my organizations (created by current user)
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyOrganizations()
    {
        var organizations = await _mediator.Send(new GetMyOrganizationsQuery());
        var response = ApiResponse<object>.SuccessResponse(organizations, "تم جلب قائمة مؤسساتك بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Update organization profile (including logo)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateOrganization(Guid id, [FromBody] UpdateOrganizationCommand command)
    {
        if (id != command.OrganizationId)
        {
            return BadRequest(ApiErrorResponse.Create("معرف المؤسسة غير متطابق"));
        }

        await _mediator.Send(command);
        var response = ApiResponse<object>.SuccessResponse(new { }, "تم تحديث المؤسسة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Update organization domain
    /// </summary>
    [HttpPut("{id}/domain")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateOrganizationDomain(Guid id, [FromBody] UpdateOrganizationDomainCommand command)
    {
        if (id != command.OrganizationId)
        {
            return BadRequest(ApiErrorResponse.Create("معرف المؤسسة غير متطابق"));
        }

        await _mediator.Send(command);
        var response = ApiResponse<object>.SuccessResponse(new { }, "تم تحديث نطاق المؤسسة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Change organization status
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> ChangeOrganizationStatus(Guid id, [FromBody] ChangeOrganizationStatusCommand command)
    {
        if (id != command.OrganizationId)
        {
            return BadRequest(ApiErrorResponse.Create("معرف المؤسسة غير متطابق"));
        }

        await _mediator.Send(command);
        var response = ApiResponse<object>.SuccessResponse(new { }, "تم تغيير حالة المؤسسة بنجاح");
        return Ok(response);
    }
}
