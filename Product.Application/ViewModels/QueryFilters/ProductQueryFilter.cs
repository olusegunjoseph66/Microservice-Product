using Product.Application.Enums;

namespace Product.Application.ViewModels.QueryFilters
{
    public class ProductQueryFilter
    {
        public string? CompanyCode { get; set; }
        public string? SearchKeyword { get; set; }
        public string? ProductStatusCode { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public ProductSortingEnum Sort { get; set; }
    }
}
