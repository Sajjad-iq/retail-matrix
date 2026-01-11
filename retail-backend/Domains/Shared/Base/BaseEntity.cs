namespace Domains.Shared.Base;

public class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime InsertDate { get; protected set; }
    public DateTime UpdateDate { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }

}
