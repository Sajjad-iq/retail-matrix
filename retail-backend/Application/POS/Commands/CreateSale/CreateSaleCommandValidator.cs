using FluentValidation;

namespace Application.POS.Commands.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.InventoryId)
            .NotEmpty()
            .WithMessage("معرف المخزن مطلوب");

        When(x => x.Items.Any(), () =>
        {
            RuleForEach(x => x.Items)
                .SetValidator(new SaleItemInputValidator());
        });
    }
}

public class SaleItemInputValidator : AbstractValidator<SaleItemInput>
{
    public SaleItemInputValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.ProductPackagingId.HasValue)
            .WithMessage("يجب توفير الباركود أو معرف المنتج");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من صفر");

        When(x => x.Discount != null, () =>
        {
            RuleFor(x => x.Discount!)
                .SetValidator(new DiscountInputValidator());
        });
    }
}

public class DiscountInputValidator : AbstractValidator<DiscountInput>
{
    public DiscountInputValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("قيمة الخصم يجب أن تكون أكبر من صفر");

        When(x => x.IsPercentage, () =>
        {
            RuleFor(x => x.Amount)
                .LessThanOrEqualTo(100)
                .WithMessage("نسبة الخصم يجب ألا تتجاوز 100%");
        });
    }
}
