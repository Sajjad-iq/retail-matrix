using FluentValidation;

namespace Application.POS.Commands.RemoveCartItem;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("معرف العنصر مطلوب");
    }
}
