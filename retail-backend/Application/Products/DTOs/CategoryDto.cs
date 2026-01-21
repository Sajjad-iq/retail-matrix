namespace Application.Products.DTOs;

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public Guid OrganizationId { get; init; }
    public bool IsActive { get; init; }
    public DateTime InsertDate { get; init; }
}
