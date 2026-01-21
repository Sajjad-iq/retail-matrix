using FluentValidation;

namespace Application.Stocks.Commands.CreateStock;

public class CreateStockCommandValidator : AbstractValidator<CreateStockCommand>
{
    public CreateStockCommandValidator()
    {
        RuleFor(x => x.ProductPackagingId)
            .NotEmpty().WithMessage("معرف العبوة مطلوب");

        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("معرف المخزن مطلوب");

        // If initial batch number is provided, quantity must be provided too
        When(x => !string.IsNullOrWhiteSpace(x.InitialBatchNumber), () =>
        {
            RuleFor(x => x.InitialQuantity)
                .NotNull().WithMessage("الكمية الأولية مطلوبة عند تحديد رقم الدفعة")
                .GreaterThanOrEqualTo(0).WithMessage("الكمية لا يمكن أن تكون سالبة");
        });

        // If initial quantity is provided, batch number must be provided too
        When(x => x.InitialQuantity.HasValue, () =>
        {
            RuleFor(x => x.InitialBatchNumber)
                .NotEmpty().WithMessage("رقم الدفعة مطلوب عند تحديد الكمية الأولية");
        });
    }
}
