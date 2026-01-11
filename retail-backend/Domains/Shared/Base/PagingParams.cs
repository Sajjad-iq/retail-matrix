namespace Domains.Shared.Base;

/// <summary>
/// Base pagination parameters
/// </summary>
public class PagingParams
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    private int _pageNumber = 1;
    private int _pageSize = DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
                _pageSize = DefaultPageSize;
            else if (value > MaxPageSize)
                _pageSize = MaxPageSize;
            else
                _pageSize = value;
        }
    }

    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;

    public PagingParams()
    {
    }

    public PagingParams(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
