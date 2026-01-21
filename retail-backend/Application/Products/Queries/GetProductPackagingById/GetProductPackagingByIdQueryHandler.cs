using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Queries.GetProductPackagingById;

public class GetProductPackagingByIdQueryHandler : IRequestHandler<GetProductPackagingByIdQuery, ProductPackagingDto>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IMapper _mapper;

    public GetProductPackagingByIdQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IMapper mapper)
    {
        _productPackagingRepository = productPackagingRepository;
        _mapper = mapper;
    }

    public async Task<ProductPackagingDto> Handle(GetProductPackagingByIdQuery request, CancellationToken cancellationToken)
    {
        var packaging = await _productPackagingRepository.GetByIdAsync(request.PackagingId, cancellationToken);

        if (packaging == null)
        {
            throw new NotFoundException("العبوة غير موجودة");
        }

        return _mapper.Map<ProductPackagingDto>(packaging);
    }
}
