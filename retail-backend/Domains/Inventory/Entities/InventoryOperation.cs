using Domains.Inventory.Enums;
using Domains.Shared.Base;
using InventoryEntity = Domains.Inventory.Entities.Inventory;

namespace Domains.Inventory.Entities;

/// <summary>
/// Represents a bulk inventory operation (header) containing multiple product movements
/// </summary>
public class InventoryOperation : BaseEntity
{
    // Parameterless constructor for EF Core
    private InventoryOperation()
    {
        OperationNumber = string.Empty;
        Items = new List<InventoryOperationItem>();
    }

    // Private constructor to enforce factory methods
    private InventoryOperation(
        InventoryOperationType operationType,
        string operationNumber,
        Guid userId,
        Guid organizationId,
        Guid? sourceInventoryId = null,
        Guid? destinationInventoryId = null,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        OperationType = operationType;
        OperationNumber = operationNumber;
        OperationDate = DateTime.UtcNow;
        UserId = userId;
        OrganizationId = organizationId;
        SourceInventoryId = sourceInventoryId;
        DestinationInventoryId = destinationInventoryId;
        Notes = notes;
        Status = InventoryOperationStatus.Draft;
        Items = new List<InventoryOperationItem>();
        InsertDate = DateTime.UtcNow;
    }

    // Properties
    public InventoryOperationType OperationType { get; private set; }
    public string OperationNumber { get; private set; }
    public DateTime OperationDate { get; private set; }
    public Guid? SourceInventoryId { get; private set; }
    public Guid? DestinationInventoryId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public InventoryOperationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public InventoryEntity? SourceInventory { get; private set; }
    public InventoryEntity? DestinationInventory { get; private set; }
    public List<InventoryOperationItem> Items { get; private set; }

    /// <summary>
    /// Factory method to create a new inventory operation
    /// </summary>
    public static InventoryOperation Create(
        InventoryOperationType operationType,
        string operationNumber,
        Guid userId,
        Guid organizationId,
        Guid? sourceInventoryId = null,
        Guid? destinationInventoryId = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(operationNumber))
            throw new ArgumentException("رقم العملية مطلوب", nameof(operationNumber));

        if (userId == Guid.Empty)
            throw new ArgumentException("معرف المستخدم مطلوب", nameof(userId));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المؤسسة مطلوب", nameof(organizationId));

        // Validate inventory requirements based on operation type
        if (operationType == InventoryOperationType.Transfer)
        {
            if (sourceInventoryId == null || destinationInventoryId == null)
                throw new ArgumentException("عمليات النقل تتطلب موقع المصدر والوجهة");
        }

        return new InventoryOperation(
            operationType,
            operationNumber,
            userId,
            organizationId,
            sourceInventoryId,
            destinationInventoryId,
            notes
        );
    }

    // Domain methods
    public void AddItem(InventoryOperationItem item)
    {
        if (Status != InventoryOperationStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل عملية مكتملة أو ملغاة");

        Items.Add(item);
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != InventoryOperationStatus.Draft)
            throw new InvalidOperationException("لا يمكن تعديل عملية مكتملة أو ملغاة");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
            Items.Remove(item);
    }

    public void Complete()
    {
        if (Status != InventoryOperationStatus.Draft)
            throw new InvalidOperationException("يمكن إكمال العمليات في حالة المسودة فقط");

        if (!Items.Any())
            throw new InvalidOperationException("لا يمكن إكمال عملية بدون عناصر");

        Status = InventoryOperationStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == InventoryOperationStatus.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء عملية مكتملة");

        Status = InventoryOperationStatus.Cancelled;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
