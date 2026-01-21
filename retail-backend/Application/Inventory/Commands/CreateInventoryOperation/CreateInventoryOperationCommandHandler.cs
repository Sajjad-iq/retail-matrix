using Application.Common.Exceptions;
using Domains.Inventory.Entities;
using Domains.Inventory.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Inventory.Commands.CreateInventoryOperation;

public class CreateInventoryOperationCommandHandler : IRequestHandler<CreateInventoryOperationCommand, Guid>
{
    private readonly IInventoryOperationRepository _inventoryOperationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateInventoryOperationCommandHandler(
        IInventoryOperationRepository inventoryOperationRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _inventoryOperationRepository = inventoryOperationRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreateInventoryOperationCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user ID from claims
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("المستخدم غير مصرح له");
        }

        // 2. Get organization ID from claims (assuming it's stored in claims)
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // 3. Create inventory operation using factory method
        var operation = InventoryOperation.Create(
            operationType: request.OperationType,
            operationNumber: request.OperationNumber,
            userId: userId,
            organizationId: organizationId,
            sourceInventoryId: request.SourceInventoryId,
            destinationInventoryId: request.DestinationInventoryId,
            notes: request.Notes
        );

        // 4. Persist operation
        await _inventoryOperationRepository.AddAsync(operation, cancellationToken);
        await _inventoryOperationRepository.SaveChangesAsync(cancellationToken);

        // 5. Return operation ID
        return operation.Id;
    }
}
