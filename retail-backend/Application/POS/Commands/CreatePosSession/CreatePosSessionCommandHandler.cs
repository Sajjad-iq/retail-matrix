using Application.Common.Exceptions;
using Domains.Sales.Entities;
using Domains.Sales.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.POS.Commands.CreatePosSession;

public class CreatePosSessionCommandHandler : IRequestHandler<CreatePosSessionCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePosSessionCommandHandler(
        ISaleRepository saleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _saleRepository = saleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreatePosSessionCommand request, CancellationToken cancellationToken)
    {
        // Extract organization ID and user ID from JWT claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("معرف المستخدم مطلوب");
        }

        // Create a new Draft sale
        var sale = Sale.Create(
            organizationId: organizationId,
            salesPersonId: userId
        );

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}
