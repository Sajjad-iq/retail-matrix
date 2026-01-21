using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException("الفئة غير موجودة");
        }

        return _mapper.Map<CategoryDto>(category);
    }
}
