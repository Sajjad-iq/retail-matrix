using FluentValidation;

namespace Application.POS.Commands.CancelPosSession;

public class CancelPosSessionCommandValidator : AbstractValidator<CancelPosSessionCommand>
{
    public CancelPosSessionCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");
    }
}
