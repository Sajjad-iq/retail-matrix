using FluentValidation;

namespace Application.Products.Commands.CreateProductPackaging;

public class CreateProductPackagingCommandValidator : AbstractValidator<CreateProductPackagingCommand>
{
    public CreateProductPackagingCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("اسم المنتج مطلوب")
            .When(x => !x.ProductId.HasValue); // Only required when auto-creating product

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم العبوة مطلوب");

        RuleFor(x => x.SellingPriceAmount)
            .GreaterThan(0).WithMessage("سعر البيع يجب أن يكون أكبر من صفر");

        RuleFor(x => x.UnitOfMeasure)
            .IsInEnum().WithMessage("وحدة القياس غير صحيحة");
    }
}
