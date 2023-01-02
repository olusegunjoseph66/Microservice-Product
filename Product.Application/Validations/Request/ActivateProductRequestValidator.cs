using FluentValidation;

namespace Product.Application.Validations.Request
{
    public class ActivateProductRequestValidator : AbstractValidator<ActivateProductRequest>
    {
        public ActivateProductRequestValidator()
        {
            RuleFor(b => b.ProductId)
           .NotNull().NotEmpty().GreaterThan(0).WithMessage("kindly, enter a valid ProductId");
        }
    }
}
