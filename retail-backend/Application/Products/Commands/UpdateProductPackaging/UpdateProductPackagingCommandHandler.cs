using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Commands.UpdateProductPackaging;

public class UpdateProductPackagingCommandHandler : IRequestHandler<UpdateProductPackagingCommand, Unit>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrganizationContext _organizationContext;

    public UpdateProductPackagingCommandHandler(
        IProductPackagingRepository productPackagingRepository,
        IProductRepository productRepository,
        IOrganizationContext organizationContext)
    {
        _productPackagingRepository = productPackagingRepository;
        _productRepository = productRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Unit> Handle(UpdateProductPackagingCommand request, CancellationToken cancellationToken)
    {
        // We need to fetch with Product to check OrganizationId since ProductPackaging might not have it directly mapped in all contexts,
        // but typically it's navigated via Product.
        // Assuming GetByIdWithProductAsync or similar. 
        // Or we can check if IProductPackagingRepository has a generic GetByIdAsync which returns entity that includes ProductId, then check Product?

        // Let's use GetByIdWithProductAsync as verified in Step 313
        var packaging = await _productPackagingRepository.GetByIdWithProductAsync(request.Id, cancellationToken);

        if (packaging == null)
        {
            throw new NotFoundException($"وحدة البيع بالمعرف {request.Id} غير موجودة");
        }

        if (packaging.Product.OrganizationId != _organizationContext.OrganizationId)
        {
            throw new NotFoundException($"وحدة البيع بالمعرف {request.Id} غير موجودة");
        }

        // Check barcode uniqueness if changed
        if (request.Barcode != packaging.Barcode?.Value)
        {
            if (!string.IsNullOrEmpty(request.Barcode))
            {
                var exists = await _productPackagingRepository.ExistsByBarcodeAsync(request.Barcode, cancellationToken);
                if (exists)
                {
                    throw new ValidationException("الباركود مستخدم بالفعل");
                }
            }
        }

        // Update properties
        packaging.UpdateInfo(
            name: request.Name,
            sellingPrice: request.SellingPrice,
            unitOfMeasure: request.UnitOfMeasure,
            barcode: request.Barcode,
            description: request.Description,
            unitsPerPackage: request.UnitsPerPackage,
            isDefault: request.IsDefault,
            imageUrls: request.ImageUrls,
            dimensions: request.Dimensions,
            weight: request.Weight,
            color: request.Color
        );

        await _productPackagingRepository.UpdateAsync(packaging, cancellationToken);
        await _productPackagingRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
