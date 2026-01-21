using Application.Common.Exceptions;
using Domains.Products.Entities;
using Domains.Products.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Products.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from claims
        var orgIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("OrganizationId")?.Value;
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var organizationId))
        {
            throw new UnauthorizedException("معرف المؤسسة مطلوب");
        }

        // 2. Verify parent category exists if provided
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                throw new NotFoundException("الفئة الأب غير موجودة");
            }
        }

        // 3. Create category using domain factory
        var category = Category.Create(
            name: request.Name,
            organizationId: organizationId,
            description: request.Description,
            parentCategoryId: request.ParentCategoryId
        );

        // 4. Persist changes
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
