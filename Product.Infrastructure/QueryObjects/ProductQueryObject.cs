using Product.Application.DTOs.Filters;
using Product.Application.Enums;
using Shared.Data.Extensions;
using Shared.Utilities.Helpers;

namespace Product.Infrastructure.QueryObjects
{
    public class ProductQueryObject : QueryObject<Shared.Data.Models.Product>
    {
        public ProductQueryObject(ProductFilterDto filter)
        {
            if (filter == null)
                And(p => p.ProductStatus.Code == ProductStatusEnum.Active.ToDescription()); 

            if(!string.IsNullOrWhiteSpace(filter.ProductStatusCode))
                And(p => p.ProductStatus.Code == filter.ProductStatusCode);
            else
                And(p => p.ProductStatus.Code == ProductStatusEnum.Active.ToDescription());

            if (!string.IsNullOrWhiteSpace(filter.CompanyCode))
                And(p => p.CompanyCode == filter.CompanyCode);

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                And(p => p.Name.Contains(filter.SearchText)
                  || p.Description.Contains(filter.SearchText)
                  || p.ProductSapNumber.Contains(filter.SearchText));
            }
        }
    }
}
