namespace Product.Application.ViewModels.Responses
{
    public class ProductStatusResponse
    {
        public ProductStatusResponse(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
