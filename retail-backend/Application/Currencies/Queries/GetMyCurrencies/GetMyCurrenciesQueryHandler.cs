using Application.Currencies.DTOs;
using Application.Common.Services;
using AutoMapper;
using Domains.Common.Currency.Repositories;
using Domains.Shared.Base;
using MediatR;

namespace Application.Currencies.Queries.GetMyCurrencies;

public class GetMyCurrenciesQueryHandler : IRequestHandler<GetMyCurrenciesQuery, PagedResult<CurrencyDto>>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IOrganizationContext _organizationContext;
    private readonly IMapper _mapper;

    public GetMyCurrenciesQueryHandler(
        ICurrencyRepository currencyRepository,
        IOrganizationContext organizationContext,
        IMapper mapper)
    {
        _currencyRepository = currencyRepository;
        _organizationContext = organizationContext;
        _mapper = mapper;
    }

    public async Task<PagedResult<CurrencyDto>> Handle(GetMyCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var organizationId = _organizationContext.OrganizationId;

        var pagingParams = new PagingParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var currencies = await _currencyRepository.GetByOrganizationAsync(
            organizationId,
            request.Status,
            request.SearchTerm,
            pagingParams,
            cancellationToken);

        var currencyDtos = _mapper.Map<List<CurrencyDto>>(currencies.Items);

        return new PagedResult<CurrencyDto>(
            currencyDtos,
            currencies.TotalCount,
            currencies.PageNumber,
            currencies.PageSize);
    }
}
