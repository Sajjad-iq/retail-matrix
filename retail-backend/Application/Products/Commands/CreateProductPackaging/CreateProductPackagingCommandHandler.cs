using Application.Common.Exceptions;
using Domains.Products.Entities;
using Domains.Products.Repositories;
using Domains.Shared.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Products.Commands.CreateProductPackaging;

public class CreateProductPackagingCommandHandler : IRequestHandler<CreateProductPackagingCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateProductPackagingCommandHandler(
        IProductRepository productRepository,
        IProductPackagingRepository productPackagingRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _productPackagingRepository = productPackagingRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreateProductPackagingCommand request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // 2. Check barcode uniqueness if provided
        if (!string.IsNullOrEmpty(request.Barcode))
        {
            var barcodeExists = await _productPackagingRepository.ExistsByBarcodeAsync(request.Barcode, cancellationToken);
            if (barcodeExists)
            {
                throw new ValidationException("الباركود مستخدم بالفعل");
            }
        }

        // 3. Get or create product
        Product product;
        if (request.ProductId.HasValue)
        {
            // Use existing product
            var existingProduct = await _productRepository.GetByIdAsync(request.ProductId.Value, cancellationToken);
            if (existingProduct == null)
            {
                throw new NotFoundException("المنتج غير موجود");
            }
            product = existingProduct;
        }
        else
        {
            // Auto-create new product with name and images
            product = Product.Create(
                name: request.ProductName,
                organizationId: organizationId,
                imageUrls: request.ProductImageUrls
            );

            if (request.CategoryId.HasValue)
            {
                product.AssignCategory(request.CategoryId.Value);
            }
            await _productRepository.AddAsync(product, cancellationToken);
        }

        // 4. Create value objects
        var sellingPrice = Price.Create(request.SellingPriceAmount, request.SellingPriceCurrency);
        Weight? weight = null;
        if (request.WeightValue.HasValue && request.WeightUnit.HasValue)
        {
            weight = Weight.Create(request.WeightValue.Value, request.WeightUnit.Value);
        }

        // 5. Add packaging to product using domain method
        var packaging = product.AddPackaging(
            name: request.Name,
            sellingPrice: sellingPrice,
            unitOfMeasure: request.UnitOfMeasure,
            barcode: request.Barcode,
            description: request.Description,
            unitsPerPackage: request.UnitsPerPackage,
            isDefault: request.IsDefault,
            imageUrls: request.ImageUrls,
            dimensions: request.Dimensions,
            weight: weight,
            color: request.Color
        );

        // 6. Persist changes
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return packaging.Id;
    }
}
