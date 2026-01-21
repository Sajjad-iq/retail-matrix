using FluentValidation;

namespace Application.Products.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم الفئة مطلوب")
            .MaximumLength(100).WithMessage("اسم الفئة يجب أن لا يتجاوز 100 حرف");
    }
}
