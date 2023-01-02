namespace Product.Application.ViewModels.Requests
{
    public class AllProductsRequest
    {

        public string companyCode { get; set; }
        public string searchKeyword { get; set; }
        public string productStatusCode { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string sort { get; set; }
    }
}
