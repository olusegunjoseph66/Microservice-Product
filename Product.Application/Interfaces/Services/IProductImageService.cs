using Product.Application.DTOs.APIDataFormatters;
using Product.Application.ViewModels.Requests;

namespace Product.Application.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<ApiResponse> AddProductImages(UploadProductImageRequest request, CancellationToken cancellationToken);
        Task<ApiResponse> RemoveProductImages(int productImageId, CancellationToken cancellationToken);
    }
}