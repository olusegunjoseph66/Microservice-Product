using Shared.ExternalServices.DTOs;

namespace Product.Application.DTOs.Events
{
    public class ProductImageUploadedMessage : IntegrationBaseMessage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public int CreatedByUserId { get; set; }
        public string PublicUrl { get; set; }
        public string CloudPath { get; set; }

    }
}
