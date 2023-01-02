using FluentValidation;
using Product.Application.ViewModels.QueryFilters;
using Shared.Utilities.Constants;

namespace Product.Application.Validations.Filters
{
    public class ProductQueryFilterValidator : AbstractValidator<ProductQueryFilter>
    {
        public ProductQueryFilterValidator()
        {
            RuleFor(b => b.PageIndex)
           .NotNull().NotEmpty().GreaterThan(0).WithMessage("kindly, enter a valid PageIndex");

            RuleFor(b => b.PageSize)
           .NotNull().NotEmpty().GreaterThan(0).LessThanOrEqualTo(PaginationConstants.DEFAULT_PAGE_SIZE).WithMessage($"PageSize can only be from 1 to {PaginationConstants.DEFAULT_PAGE_SIZE}. Kindly, enter a valid PageSize");

            RuleFor(b => b.Sort)
           .NotEmpty().WithMessage("kindly, supply the sort parameter.");
        }
    }
}
