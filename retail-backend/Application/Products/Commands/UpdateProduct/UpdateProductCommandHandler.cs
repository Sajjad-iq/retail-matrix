using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrganizationContext _organizationContext;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IOrganizationContext organizationContext)
    {
        _productRepository = productRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found");
        }

        // Verify organization access
        if (product.OrganizationId != _organizationContext.OrganizationId)
        {
            throw new NotFoundException($"Product with ID {request.Id} not found"); // Hide existence
        }

        // Update product info
        product.UpdateInfo(request.ProductName);

        // Update category
        if (request.CategoryId.HasValue)
        {
            product.AssignCategory(request.CategoryId.Value);
        }
        else
        {
            product.UnassignCategory();
        }

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
