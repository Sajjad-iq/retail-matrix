using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Commands.DeleteProductPackaging;

public class DeleteProductPackagingCommandHandler : IRequestHandler<DeleteProductPackagingCommand, Unit>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IOrganizationContext _organizationContext;

    public DeleteProductPackagingCommandHandler(
        IProductPackagingRepository productPackagingRepository,
        IOrganizationContext organizationContext)
    {
        _productPackagingRepository = productPackagingRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Unit> Handle(DeleteProductPackagingCommand request, CancellationToken cancellationToken)
    {
        var packaging = await _productPackagingRepository.GetByIdWithProductAsync(request.Id, cancellationToken);
        if (packaging == null)
        {
            throw new NotFoundException($"وحدة البيع بالمعرف {request.Id} غير موجودة");
        }

        if (packaging.Product.OrganizationId != _organizationContext.OrganizationId)
        {
            throw new NotFoundException($"وحدة البيع بالمعرف {request.Id} غير موجودة");
        }

        // Potential check: Cannot delete the last packaging? Or isDefault? 
        // Business rule: If it's the only packaging or default, maybe restrict?
        // For now, allow deletion. Repository/Database constraints (Cascade) might handle Product deletion if empty? 
        // Usually Product remains even if empty packagings?

        await _productPackagingRepository.DeleteAsync(request.Id, cancellationToken);
        await _productPackagingRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
