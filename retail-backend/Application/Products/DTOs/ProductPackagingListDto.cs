using Domains.Products.Enums;
using Domains.Shared.ValueObjects;
using BarcodeVO = Domains.Shared.ValueObjects.Barcode;

namespace Application.Products.DTOs;

public record ProductPackagingListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public BarcodeVO? Barcode { get; init; }
    public Price SellingPrice { get; init; } = null!;
    public ProductStatus Status { get; init; }
}
