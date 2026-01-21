using FluentValidation;

namespace Application.POS.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("معرف جلسة البيع مطلوب");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("معرف العنصر مطلوب");

        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("معرف المخزن مطلوب");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من صفر");
    }
}
