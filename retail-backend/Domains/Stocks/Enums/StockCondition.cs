namespace Domains.Stocks.Enums;

/// <summary>
/// Represents the physical condition of stock items
/// </summary>
public enum StockCondition
{
    /// <summary>
    /// جديد - New/unused stock
    /// </summary>
    New = 0,

    /// <summary>
    /// جيد - Good condition
    /// </summary>
    Good = 1,

    /// <summary>
    /// تالف - Damaged stock
    /// </summary>
    Damaged = 2,

    /// <summary>
    /// منتهي الصلاحية - Expired stock
    /// </summary>
    Expired = 3,

    /// <summary>
    /// معيب - Defective stock
    /// </summary>
    Defective = 4
}
