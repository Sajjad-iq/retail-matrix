namespace Domains.Installments.Enums;

public enum InstallmentPlanStatus
{
    Draft,          // Being created
    Active,         // Approved and payments in progress
    Completed,      // All installments paid
    Defaulted,      // Customer failed to pay
    Cancelled       // Cancelled before completion
}
