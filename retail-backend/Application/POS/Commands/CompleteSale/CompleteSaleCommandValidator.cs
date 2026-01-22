using FluentValidation;

namespace Application.POS.Commands.CompleteSale;

public class CompleteSaleCommandValidator : AbstractValidator<CompleteSaleCommand>
{
    public CompleteSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("معرف البيع مطلوب");

        RuleFor(x => x.InventoryId)
            .NotEmpty()
            .WithMessage("معرف المخزن مطلوب");

        RuleFor(x => x.AmountPaid)
            .GreaterThan(0)
            .WithMessage("المبلغ المدفوع يجب أن يكون أكبر من صفر");
    }
}
