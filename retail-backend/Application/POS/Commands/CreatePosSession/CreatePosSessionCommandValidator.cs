using FluentValidation;

namespace Application.POS.Commands.CreatePosSession;

public class CreatePosSessionCommandValidator : AbstractValidator<CreatePosSessionCommand>
{
    public CreatePosSessionCommandValidator()
    {
        // InventoryId is optional, but if provided must be valid
        When(x => x.InventoryId.HasValue, () =>
        {
            RuleFor(x => x.InventoryId)
                .NotEqual(Guid.Empty).WithMessage("معرف المخزن غير صالح");
        });
    }
}
