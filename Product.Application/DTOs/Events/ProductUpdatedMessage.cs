using Shared.ExternalServices.DTOs;

namespace Product.Application.DTOs.Events
{
    public class ProductUpdatedMessage : IntegrationBaseMessage
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public string ProductSapNumber { get; set; }
        public DateTime DateModified { get; set; }
        public int ModifiedByUserId { get; set; }
        public NameAndCode ProductStatus { get; set; }
    }
}
