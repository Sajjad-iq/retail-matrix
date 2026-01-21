using Application.Common.Exceptions;
using Application.Stocks.DTOs;
using AutoMapper;
using Domains.Stocks.Repositories;
using MediatR;

namespace Application.Stocks.Queries.GetStockById;

public class GetStockByIdQueryHandler : IRequestHandler<GetStockByIdQuery, StockDto>
{
    private readonly IStockRepository _stockRepository;
    private readonly IMapper _mapper;

    public GetStockByIdQueryHandler(
        IStockRepository stockRepository,
        IMapper mapper)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
    }

    public async Task<StockDto> Handle(GetStockByIdQuery request, CancellationToken cancellationToken)
    {
        var stock = await _stockRepository.GetWithBatchesAsync(request.StockId, cancellationToken);

        if (stock == null)
        {
            throw new NotFoundException("المخزون غير موجود");
        }

        return _mapper.Map<StockDto>(stock);
    }
}
