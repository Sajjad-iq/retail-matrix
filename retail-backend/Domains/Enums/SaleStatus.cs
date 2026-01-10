namespace Domains.Enums;

public enum SaleStatus
{
    Draft,          // Being created
    Completed,      // Finalized and paid
    PartiallyPaid,  // Some payment received
    Cancelled,      // Cancelled before completion
    Returned        // Fully returned
}
