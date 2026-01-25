using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrganizationContext _organizationContext;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        IOrganizationContext organizationContext)
    {
        _productRepository = productRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            // If strictly following idempotency, might return success, but usually we throw generic not found equivalent
            throw new NotFoundException($"Product with ID {request.Id} not found");
        }

        if (product.OrganizationId != _organizationContext.OrganizationId)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found");
        }

        // Soft delete is handled by repository usually, or we call domain method to deactivate first?
        // Repository DeleteAsync documents "Delete entity (soft delete)"

        // Check if it can be deleted (e.g. if it has sales history etc - domain logic might prevent this, but for now we trust repository/domain constraints)

        await _productRepository.DeleteAsync(request.Id, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
