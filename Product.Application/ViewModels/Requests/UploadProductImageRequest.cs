using Microsoft.AspNetCore.Http;

namespace Product.Application.ViewModels.Requests
{
    public class UploadProductImageRequest
    {
        public string Image { get; set; }
        public bool IsLeadImage { get; set; }
        public int ProductId { get; set; }    
        
        
    }

    
}
