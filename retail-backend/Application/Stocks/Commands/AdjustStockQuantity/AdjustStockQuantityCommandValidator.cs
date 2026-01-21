using FluentValidation;

namespace Application.Stocks.Commands.AdjustStockQuantity;

public class AdjustStockQuantityCommandValidator : AbstractValidator<AdjustStockQuantityCommand>
{
    public AdjustStockQuantityCommandValidator()
    {
        RuleFor(x => x.StockId)
            .NotEmpty().WithMessage("معرف المخزون مطلوب");

        RuleFor(x => x.BatchId)
            .NotEmpty().WithMessage("معرف الدفعة مطلوب");

        RuleFor(x => x.QuantityChange)
            .NotEqual(0).WithMessage("تغيير الكمية يجب أن يكون مختلف عن صفر");
    }
}
