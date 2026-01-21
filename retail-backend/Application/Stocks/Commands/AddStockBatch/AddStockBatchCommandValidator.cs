using FluentValidation;

namespace Application.Stocks.Commands.AddStockBatch;

public class AddStockBatchCommandValidator : AbstractValidator<AddStockBatchCommand>
{
    public AddStockBatchCommandValidator()
    {
        RuleFor(x => x.StockId)
            .NotEmpty().WithMessage("معرف المخزون مطلوب");

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("رقم الدفعة مطلوب")
            .MaximumLength(50).WithMessage("رقم الدفعة يجب أن لا يتجاوز 50 حرف");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("الكمية لا يمكن أن تكون سالبة");
    }
}
