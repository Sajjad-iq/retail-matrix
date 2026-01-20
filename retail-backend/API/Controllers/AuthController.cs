using Application.Auth.Commands.Login;
using Application.Auth.Commands.Register;
using Application.Auth.Queries.GetCurrentUser;
using API.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Register([FromBody] RegisterCommand command)
    {
        var userId = await _mediator.Send(command);
        var response = ApiResponse<Guid>.SuccessResponse(userId, "تم تسجيل المستخدم بنجاح");
        return CreatedAtAction(nameof(GetCurrentUser), new { id = userId }, response);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        var response = ApiResponse<object>.SuccessResponse(token, "تم تسجيل الدخول بنجاح");
        return Ok(response);
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiErrorResponse.Create("غير مصرح لك بالوصول"));
        }

        var user = await _mediator.Send(new GetCurrentUserQuery(userId));

        if (user == null)
        {
            return NotFound(ApiErrorResponse.Create("المستخدم غير موجود"));
        }

        var response = ApiResponse<object>.SuccessResponse(user, "تم جلب بيانات المستخدم بنجاح");
        return Ok(response);
    }
}
