using FluentValidation;

namespace Application.POS.Commands.CompletePosSession;

public class CompletePosSessionCommandValidator : AbstractValidator<CompletePosSessionCommand>
{
    public CompletePosSessionCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("معرف المخزن مطلوب");
    }
}
