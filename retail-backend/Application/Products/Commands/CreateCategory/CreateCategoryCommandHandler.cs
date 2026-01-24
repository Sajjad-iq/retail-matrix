using Application.Common.Exceptions;
using Application.Common.Services;
using Domains.Products.Entities;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrganizationContext _organizationContext;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IOrganizationContext organizationContext)
    {
        _categoryRepository = categoryRepository;
        _organizationContext = organizationContext;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Get organization ID from context
        var organizationId = _organizationContext.OrganizationId;

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
