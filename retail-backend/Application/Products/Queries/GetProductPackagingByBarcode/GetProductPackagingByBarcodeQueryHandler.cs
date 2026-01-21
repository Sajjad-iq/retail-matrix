using Application.Common.Exceptions;
using Application.Products.DTOs;
using AutoMapper;
using Domains.Products.Repositories;
using MediatR;

namespace Application.Products.Queries.GetProductPackagingByBarcode;

public class GetProductPackagingByBarcodeQueryHandler : IRequestHandler<GetProductPackagingByBarcodeQuery, ProductPackagingDto>
{
    private readonly IProductPackagingRepository _productPackagingRepository;
    private readonly IMapper _mapper;

    public GetProductPackagingByBarcodeQueryHandler(
        IProductPackagingRepository productPackagingRepository,
        IMapper mapper)
    {
        _productPackagingRepository = productPackagingRepository;
        _mapper = mapper;
    }

    public async Task<ProductPackagingDto> Handle(GetProductPackagingByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var packaging = await _productPackagingRepository.GetByBarcodeAsync(request.Barcode, cancellationToken);

        if (packaging == null)
        {
            throw new NotFoundException("العبوة غير موجودة");
        }

        return _mapper.Map<ProductPackagingDto>(packaging);
    }
}
