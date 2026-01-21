using FluentValidation;

namespace Application.POS.Commands.RecordPosPayment;

public class RecordPosPaymentCommandValidator : AbstractValidator<RecordPosPaymentCommand>
{
    public RecordPosPaymentCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.Amount)
            .NotNull().WithMessage("مبلغ الدفع مطلوب")
            .Must(x => x.Amount > 0).WithMessage("مبلغ الدفع يجب أن يكون أكبر من صفر");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("طريقة الدفع مطلوبة");
    }
}
