using FluentValidation;

namespace Application.POS.Commands.AddCartItem;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("معرف المخزن مطلوب");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر");

        // Either barcode or productPackagingId must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Barcode) || x.ProductPackagingId.HasValue)
            .WithMessage("يجب تحديد الباركود أو معرف المنتج");
    }
}
