using Domains.Installments.Enums;
using Domains.Shared.ValueObjects;
using Domains.Shared.Base;

namespace Domains.Installments.Entities;

/// <summary>
/// Aggregate root for installment payment plans.
/// Customer can pay any amount at any time.
/// </summary>
public class InstallmentPlan : BaseEntity
{
    // Parameterless constructor for EF Core
    private InstallmentPlan()
    {
        PlanNumber = string.Empty;
        TotalAmount = null!;
        DownPayment = null!;
        InterestAmount = null!;
        Payments = new List<InstallmentPayment>();
    }

    private InstallmentPlan(
        string planNumber,
        Guid organizationId,
        Guid saleId,
        Guid customerId,
        Price totalAmount,
        Price downPayment,
        Price interestAmount,
        PaymentFrequency paymentFrequency,
        Guid createdByUserId)
    {
        Id = Guid.NewGuid();
        PlanNumber = planNumber;
        OrganizationId = organizationId;
        SaleId = saleId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        DownPayment = downPayment;
        InterestAmount = interestAmount;
        PaymentFrequency = paymentFrequency;
        CreatedByUserId = createdByUserId;
        Status = InstallmentPlanStatus.Draft;
        InsertDate = DateTime.UtcNow;
        Payments = new List<InstallmentPayment>();
    }

    // Properties
    public string PlanNumber { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Price TotalAmount { get; private set; }
    public Price DownPayment { get; private set; }
    public Price InterestAmount { get; private set; }
    public PaymentFrequency PaymentFrequency { get; private set; }
    public InstallmentPlanStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public List<InstallmentPayment> Payments { get; private set; }

    /// <summary>
    /// Factory method to create a new installment plan
    /// </summary>
    public static InstallmentPlan Create(
        Guid organizationId,
        Guid saleId,
        Guid customerId,
        Price totalAmount,
        PaymentFrequency paymentFrequency,
        Guid createdByUserId,
        Price? downPayment = null,
        Price? interestAmount = null)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("معرف العميل مطلوب", nameof(customerId));

        if (totalAmount.Amount <= 0)
            throw new ArgumentException("المبلغ الإجمالي يجب أن يكون أكبر من صفر", nameof(totalAmount));

        var down = downPayment ?? Price.Create(0, totalAmount.Currency);

        if (down.Amount < 0)
            throw new ArgumentException("الدفعة المقدمة لا يمكن أن تكون سالبة", nameof(downPayment));

        if (down.Amount >= totalAmount.Amount)
            throw new ArgumentException("الدفعة المقدمة يجب أن تكون أقل من المبلغ الإجمالي", nameof(downPayment));

        if (down.Currency != totalAmount.Currency)
            throw new ArgumentException("عملة الدفعة المقدمة يجب أن تتطابق مع عملة المبلغ الإجمالي", nameof(downPayment));

        var interest = interestAmount ?? Price.Create(0, totalAmount.Currency);

        if (interest.Amount < 0)
            throw new ArgumentException("مبلغ الفائدة لا يمكن أن يكون سالباً", nameof(interestAmount));

        if (interest.Currency != totalAmount.Currency)
            throw new ArgumentException("عملة الفائدة يجب أن تتطابق مع عملة المبلغ الإجمالي", nameof(interestAmount));

        var planNumber = GeneratePlanNumber();

        return new InstallmentPlan(
            planNumber,
            organizationId,
            saleId,
            customerId,
            totalAmount,
            down,
            interest,
            paymentFrequency,
            createdByUserId
        );
    }

    /// <summary>
    /// Activates the plan
    /// </summary>
    public void Activate(Guid approvedByUserId)
    {
        if (Status != InstallmentPlanStatus.Draft)
            throw new InvalidOperationException("يمكن تفعيل الخطط في حالة المسودة فقط");

        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        Status = InstallmentPlanStatus.Active;
    }



    /// <summary>
    /// Records a payment of any amount and distributes it across pending installments
    /// </summary>
    public void RecordPayment(
        Price paymentAmount,
        Guid receivedByUserId,
        string? reference = null,
        string? notes = null)
    {
        if (Status != InstallmentPlanStatus.Active)
            throw new InvalidOperationException("لا يمكن تسجيل دفعات إلا للخطط النشطة");

        if (paymentAmount.Currency != TotalAmount.Currency)
            throw new ArgumentException("عملة الدفعة يجب أن تتطابق مع عملة الخطة", nameof(paymentAmount));

        if (paymentAmount.Amount <= 0)
            throw new ArgumentException("مبلغ الدفعة يجب أن يكون أكبر من صفر", nameof(paymentAmount));

        var remainingBalance = GetRemainingBalance();
        if (paymentAmount.Amount > remainingBalance.Amount)
            throw new ArgumentException($"مبلغ الدفعة ({paymentAmount.Amount}) يتجاوز المبلغ المتبقي ({remainingBalance.Amount})", nameof(paymentAmount));

        // Distribute payment across pending installments
        var remainingPayment = paymentAmount.Amount;
        var pendingPayments = Payments
            .Where(p => p.Status == Enums.InstallmentPaymentStatus.Pending)
            .OrderBy(p => p.DueDate)
            .ToList();

        foreach (var installment in pendingPayments)
        {
            if (remainingPayment <= 0)
                break;

            var amountToPay = Math.Min(remainingPayment, installment.DueAmount.Amount);

            if (amountToPay >= installment.DueAmount.Amount)
            {
                // Full payment of this installment
                installment.MarkAsPaid(receivedByUserId, reference, notes);
                remainingPayment -= installment.DueAmount.Amount;
            }
            else
            {
                // Partial payment of this installment
                var partialAmount = Price.Create(amountToPay, TotalAmount.Currency);
                installment.RecordPartialPayment(partialAmount, receivedByUserId, reference, notes);
                remainingPayment = 0;
            }
        }

        // Check if fully paid
        if (Payments.All(p => p.Status == Enums.InstallmentPaymentStatus.Paid))
        {
            Status = InstallmentPlanStatus.Completed;
        }
    }


    /// <summary>
    /// Marks the plan as defaulted
    /// </summary>
    public void MarkAsDefaulted()
    {
        if (Status != InstallmentPlanStatus.Active)
            throw new InvalidOperationException("يمكن تعليم الخطط النشطة فقط كمتعثرة");

        Status = InstallmentPlanStatus.Defaulted;
    }

    /// <summary>
    /// Cancels the plan
    /// </summary>
    public void Cancel()
    {
        if (Status == InstallmentPlanStatus.Completed)
            throw new InvalidOperationException("لا يمكن إلغاء خطة مكتملة");

        if (Payments.Any())
            throw new InvalidOperationException("لا يمكن إلغاء خطة تم دفع أقساط منها");

        Status = InstallmentPlanStatus.Cancelled;
    }

    /// <summary>
    /// Adds notes to the plan
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    /// <summary>
    /// Gets the total amount to be paid (remaining after down payment + interest)
    /// </summary>
    public Price GetTotalToPay()
    {
        var remainingAfterDown = TotalAmount.Amount - DownPayment.Amount;
        return Price.Create(remainingAfterDown + InterestAmount.Amount, TotalAmount.Currency);
    }

    /// <summary>
    /// Gets total amount paid so far (including down payment)
    /// </summary>
    public Price GetTotalPaid()
    {
        var paymentTotal = Payments.Sum(p => p.DueAmount.Amount);
        return Price.Create(DownPayment.Amount + paymentTotal, TotalAmount.Currency);
    }

    /// <summary>
    /// Gets total payments made (excluding down payment)
    /// </summary>
    public Price GetPaymentsTotal()
    {
        var total = Payments.Sum(p => p.DueAmount.Amount);
        return Price.Create(total, TotalAmount.Currency);
    }

    /// <summary>
    /// Gets remaining balance to be paid
    /// </summary>
    public Price GetRemainingBalance()
    {
        var totalToPay = GetTotalToPay();
        var paymentTotal = Payments.Sum(p => p.DueAmount.Amount);
        var remaining = totalToPay.Amount - paymentTotal;
        return Price.Create(Math.Max(0, remaining), TotalAmount.Currency);
    }

    /// <summary>
    /// Gets the payment progress percentage
    /// </summary>
    public decimal GetProgressPercentage()
    {
        var totalToPay = GetTotalToPay();
        if (totalToPay.Amount == 0)
            return 100;

        var paymentTotal = Payments.Sum(p => p.DueAmount.Amount);
        return Math.Round((paymentTotal / totalToPay.Amount) * 100, 2);
    }

    /// <summary>
    /// Gets the number of payments made
    /// </summary>
    public int GetPaymentCount()
    {
        return Payments.Count;
    }

    /// <summary>
    /// Checks if the plan is fully paid
    /// </summary>
    public bool IsFullyPaid()
    {
        return GetRemainingBalance().Amount <= 0;
    }

    private static string GeneratePlanNumber()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var uniqueId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"INS-{date}-{uniqueId}";
    }
}
