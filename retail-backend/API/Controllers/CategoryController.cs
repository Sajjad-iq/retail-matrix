using API.Models;
using Application.Products.Commands.CreateCategory;
using Application.Products.Queries.GetCategoryById;
using Application.Products.Queries.GetMyCategories;
using Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[OrganizationAuthorize]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var categoryId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(categoryId, "تم إنشاء الفئة بنجاح");
        return CreatedAtAction(nameof(GetCategoryById), new { id = categoryId }, response);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetCategoryById(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery { CategoryId = id });
        var response = ApiResponse<object>.SuccessResponse(category, "تم جلب بيانات الفئة بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get my categories (for current user's organization)
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> GetMyCategories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var categories = await _mediator.Send(new GetMyCategoriesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        var response = ApiResponse<object>.SuccessResponse(categories, "تم جلب قائمة الفئات بنجاح");
        return Ok(response);
    }
}
