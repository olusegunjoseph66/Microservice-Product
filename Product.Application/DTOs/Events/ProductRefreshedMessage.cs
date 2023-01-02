using Shared.ExternalServices.DTOs;

namespace Product.Application.DTOs.Events
{
    public class ProductRefreshedMessage : IntegrationBaseMessage
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public string ProductSapNumber { get; set; }
        public DateTime DateRefreshed { get; set; }
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Price { get; set; }
        public NameAndCode ProductStatus { get; set; }
    }
}
