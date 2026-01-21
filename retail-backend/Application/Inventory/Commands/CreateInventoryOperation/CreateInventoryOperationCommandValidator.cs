using FluentValidation;

namespace Application.Inventory.Commands.CreateInventoryOperation;

public class CreateInventoryOperationCommandValidator : AbstractValidator<CreateInventoryOperationCommand>
{
    public CreateInventoryOperationCommandValidator()
    {
        RuleFor(x => x.OperationType)
            .IsInEnum().WithMessage("نوع العملية غير صحيح");

        RuleFor(x => x.OperationNumber)
            .NotEmpty().WithMessage("رقم العملية مطلوب");
    }
}
