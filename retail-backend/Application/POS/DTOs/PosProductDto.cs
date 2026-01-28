namespace Application.POS.DTOs;

/// <summary>
/// DTO for product with all packagings and stock information for POS
/// </summary>
public record PosProductDto
{
    // Product Information
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public List<string> ImageUrls { get; init; } = new();
    
    // All available packagings with stock information
    public List<PosPackagingDto> Packagings { get; init; } = new();
    
    // Computed Properties
    public int TotalPackagings => Packagings.Count;
    public int InStockPackagings => Packagings.Count(p => p.InStock);
    public bool HasStock => Packagings.Any(p => p.InStock);
    public int TotalAvailableStock => Packagings.Sum(p => p.AvailableStock);
}
