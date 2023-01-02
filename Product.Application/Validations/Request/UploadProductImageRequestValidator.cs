using FluentValidation;
using Product.Application.ViewModels.Requests;

namespace Product.Application.Validations.Request
{
    public class UploadProductImageRequestValidator : AbstractValidator<UploadProductImageRequest>
    {
        public UploadProductImageRequestValidator()
        {
            RuleFor(b => b.ProductId)
           .NotNull().NotEmpty().GreaterThan(0).WithMessage("kindly, enter a valid ProductId");

            RuleFor(b => b.Image).NotNull().NotEmpty().WithMessage("kindly, upload a valid Image");
        }
    }
}
