using Domains.Products.Enums;

namespace Application.Products.DTOs;

public record ProductWithPackagingsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public List<string> ImageUrls { get; init; } = new();
    public ProductStatus Status { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public DateTime InsertDate { get; init; }
    public List<ProductPackagingListDto> Packagings { get; init; } = new();
}
