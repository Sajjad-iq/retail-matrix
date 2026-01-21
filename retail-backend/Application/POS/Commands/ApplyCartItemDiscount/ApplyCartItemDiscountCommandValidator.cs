using FluentValidation;

namespace Application.POS.Commands.ApplyCartItemDiscount;

public class ApplyCartItemDiscountCommandValidator : AbstractValidator<ApplyCartItemDiscountCommand>
{
    public ApplyCartItemDiscountCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("معرف العنصر مطلوب");

        RuleFor(x => x.DiscountValue)
            .GreaterThanOrEqualTo(0).WithMessage("قيمة الخصم يجب أن تكون صفر أو أكثر");

        RuleFor(x => x.DiscountType)
            .NotEmpty().WithMessage("نوع الخصم مطلوب")
            .Must(x => x.Equals("Percentage", StringComparison.OrdinalIgnoreCase) ||
                       x.Equals("FixedAmount", StringComparison.OrdinalIgnoreCase))
            .WithMessage("نوع الخصم يجب أن يكون Percentage أو FixedAmount");

        // Percentage discount cannot exceed 100%
        When(x => x.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.DiscountValue)
                .LessThanOrEqualTo(100).WithMessage("نسبة الخصم لا يمكن أن تتجاوز 100%");
        });
    }
}
