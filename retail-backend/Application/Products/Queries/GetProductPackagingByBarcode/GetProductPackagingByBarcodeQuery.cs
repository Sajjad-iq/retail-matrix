using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries.GetProductPackagingByBarcode;

public record GetProductPackagingByBarcodeQuery : IRequest<ProductPackagingDto>
{
    public string Barcode { get; init; } = string.Empty;
}
