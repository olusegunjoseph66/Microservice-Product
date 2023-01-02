using Shared.ExternalServices.DTOs;

namespace Product.Application.DTOs.Events
{
    public class ProductDeletedMessage : IntegrationBaseMessage
    {
        public int ProductImageId { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
    }
}
