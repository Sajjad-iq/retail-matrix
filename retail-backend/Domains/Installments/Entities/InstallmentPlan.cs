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
        OriginalAmount = null!;
        DownPayment = null!;
        InterestAmount = null!;
        Payments = new List<InstallmentPayment>();
    }

    private InstallmentPlan(
        string planNumber,
        Guid organizationId,
        Guid saleId,
        Guid customerId,
        Price originalAmount,
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
        OriginalAmount = originalAmount;
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
    public Price OriginalAmount { get; private set; }
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
        Price originalAmount,
        PaymentFrequency paymentFrequency,
        Guid createdByUserId,
        Price? downPayment = null,
        Price? interestAmount = null)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("معرف المنظمة مطلوب", nameof(organizationId));

        if (saleId == Guid.Empty)
            throw new ArgumentException("معرف البيع مطلوب", nameof(saleId));

        if (customerId == Guid.Empty)
            throw new ArgumentException("معرف العميل مطلوب", nameof(customerId));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("معرف منشئ الخطة مطلوب", nameof(createdByUserId));

        if (originalAmount.Amount <= 0)
            throw new ArgumentException("المبلغ الإجمالي يجب أن يكون أكبر من صفر", nameof(originalAmount));

        var down = downPayment ?? Price.Create(0, originalAmount.Currency);

        if (down.Amount < 0)
            throw new ArgumentException("الدفعة المقدمة لا يمكن أن تكون سالبة", nameof(downPayment));

        if (down.Amount >= originalAmount.Amount)
            throw new ArgumentException("الدفعة المقدمة يجب أن تكون أقل من المبلغ الإجمالي", nameof(downPayment));

        if (down.Currency != originalAmount.Currency)
            throw new ArgumentException("عملة الدفعة المقدمة يجب أن تتطابق مع عملة المبلغ الإجمالي", nameof(downPayment));

        var interest = interestAmount ?? Price.Create(0, originalAmount.Currency);

        if (interest.Amount < 0)
            throw new ArgumentException("مبلغ الفائدة لا يمكن أن يكون سالباً", nameof(interestAmount));

        if (interest.Currency != originalAmount.Currency)
            throw new ArgumentException("عملة الفائدة يجب أن تتطابق مع عملة المبلغ الإجمالي", nameof(interestAmount));

        var planNumber = GeneratePlanNumber();

        return new InstallmentPlan(
            planNumber,
            organizationId,
            saleId,
            customerId,
            originalAmount,
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

        if (!Payments.Any())
            throw new InvalidOperationException("لا يمكن تفعيل خطة بدون جدول دفعات");

        if (approvedByUserId == Guid.Empty)
            throw new ArgumentException("معرف المعتمد مطلوب", nameof(approvedByUserId));

        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        Status = InstallmentPlanStatus.Active;
    }

    /// <summary>
    /// Generates the payment schedule based on frequency and installment amount
    /// </summary>
    /// <param name="installmentAmount">Amount the customer can pay per installment</param>
    /// <param name="startDate">When the first payment is due (optional, defaults to today)</param>
    public void GeneratePaymentSchedule(Price installmentAmount, DateTime? startDate = null)
    {
        if (Status != InstallmentPlanStatus.Draft)
            throw new InvalidOperationException("يمكن إنشاء جدول الدفعات للخطط في حالة المسودة فقط");

        if (installmentAmount.Currency != OriginalAmount.Currency)
            throw new ArgumentException("عملة الدفعة يجب أن تتطابق مع عملة الخطة", nameof(installmentAmount));

        if (installmentAmount.Amount <= 0)
            throw new ArgumentException("مبلغ القسط يجب أن يكون أكبر من صفر", nameof(installmentAmount));

        // Calculate total amount to be paid in installments (after down payment + interest)
        var totalToPay = GetTotalToPay();

        if (installmentAmount.Amount > totalToPay.Amount)
            throw new ArgumentException($"مبلغ القسط ({installmentAmount.Amount}) يتجاوز المبلغ الإجمالي المطلوب ({totalToPay.Amount})", nameof(installmentAmount));

        // Clear any existing payments
        Payments.Clear();

        // Calculate number of installments needed
        var numberOfInstallments = (int)Math.Ceiling(totalToPay.Amount / installmentAmount.Amount);
        var remainingAmount = totalToPay.Amount;
        var currentDueDate = startDate ?? DateTime.UtcNow;

        for (int i = 1; i <= numberOfInstallments; i++)
        {
            // For the last installment, use the remaining amount (might be less than installmentAmount)
            var paymentAmount = i == numberOfInstallments
                ? Price.Create(remainingAmount, OriginalAmount.Currency)
                : installmentAmount;

            var payment = InstallmentPayment.CreateScheduled(
                Id,
                paymentAmount,
                currentDueDate,
                i
            );

            Payments.Add(payment);

            remainingAmount -= paymentAmount.Amount;
            currentDueDate = CalculateNextDueDate(currentDueDate, PaymentFrequency);
        }
    }

    /// <summary>
    /// Calculates the next due date based on payment frequency
    /// </summary>
    private DateTime CalculateNextDueDate(DateTime currentDate, PaymentFrequency frequency)
    {
        return frequency switch
        {
            PaymentFrequency.Weekly => currentDate.AddDays(7),
            PaymentFrequency.BiWeekly => currentDate.AddDays(14),
            PaymentFrequency.Monthly => currentDate.AddMonths(1),
            _ => throw new ArgumentException($"تكرار الدفع غير مدعوم: {frequency}")
        };
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

        if (paymentAmount.Currency != OriginalAmount.Currency)
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

            // Calculate how much is still owed for this installment
            var remainingForThisInstallment = installment.DueAmount.Amount - installment.PaidAmount.Amount;
            var amountToPay = Math.Min(remainingPayment, remainingForThisInstallment);

            if (amountToPay >= remainingForThisInstallment)
            {
                // Full payment of this installment (or completing a partial)
                installment.MarkAsPaid(receivedByUserId, reference, notes);
                remainingPayment -= remainingForThisInstallment;
            }
            else
            {
                // Partial payment of this installment
                var partialAmount = Price.Create(amountToPay, OriginalAmount.Currency);
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

        if (Payments.Any(p => p.PaidAmount.Amount > 0))
            throw new InvalidOperationException("لا يمكن إلغاء خطة تم دفع أقساط منها");

        Status = InstallmentPlanStatus.Cancelled;
    }

    /// <summary>
    /// Adds notes to the plan
    /// </summary>
    public void AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            throw new ArgumentException("الملاحظات لا يمكن أن تكون فارغة", nameof(notes));

        if (notes.Length > 2000)
            throw new ArgumentException("الملاحظات طويلة جداً (الحد الأقصى 2000 حرف)", nameof(notes));

        Notes = notes;
    }

    /// <summary>
    /// Gets the total amount to be paid (remaining after down payment + interest)
    /// </summary>
    public Price GetTotalToPay()
    {
        var remainingAfterDown = OriginalAmount.Amount - DownPayment.Amount;
        return Price.Create(remainingAfterDown + InterestAmount.Amount, OriginalAmount.Currency);
    }

    /// <summary>
    /// Gets total amount paid so far (including down payment)
    /// </summary>
    public Price GetTotalPaid()
    {
        var paymentTotal = Payments.Sum(p => p.PaidAmount.Amount);
        return Price.Create(DownPayment.Amount + paymentTotal, OriginalAmount.Currency);
    }

    /// <summary>
    /// Gets total payments made (excluding down payment)
    /// </summary>
    public Price GetPaymentsTotal()
    {
        var total = Payments.Sum(p => p.PaidAmount.Amount);
        return Price.Create(total, OriginalAmount.Currency);
    }

    /// <summary>
    /// Gets remaining balance to be paid
    /// </summary>
    public Price GetRemainingBalance()
    {
        var totalToPay = GetTotalToPay();
        var paymentTotal = Payments.Sum(p => p.PaidAmount.Amount);
        var remaining = totalToPay.Amount - paymentTotal;
        return Price.Create(Math.Max(0, remaining), OriginalAmount.Currency);
    }

    /// <summary>
    /// Gets the payment progress percentage
    /// </summary>
    public decimal GetProgressPercentage()
    {
        var totalToPay = GetTotalToPay();
        if (totalToPay.Amount == 0)
            return 100;

        var paymentTotal = Payments.Sum(p => p.PaidAmount.Amount);
        return Math.Round((paymentTotal / totalToPay.Amount) * 100, 2);
    }

    /// <summary>
    /// Gets the number of payments made
    /// </summary>
    public int GetPaymentCount()
    {
        return Payments.Count(p => p.Status == Enums.InstallmentPaymentStatus.Paid);
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
