using Product.Application.DTOs;
using Product.Application.DTOs.APIDataFormatters;
using Product.Application.ViewModels.QueryFilters;
using Product.Application.ViewModels.Requests;
using Product.Application.ViewModels.Responses;
using Shared.Utilities.DTO.Pagination;

namespace Product.Infrastructure.Services
{
    public interface IProductService
    {
        Task<ApiResponse> AddProducts(List<SapProductDto> products);
        Task<ApiResponse> GetCacheProducts();

        Task<ApiResponse> GetProductById(int productId, CancellationToken cancellationToken);
        Task<ApiResponse> ActivateDeactivateProduct(ActivateProductRequest request, CancellationToken cancellationToken);
        Task<ApiResponse> GetProducts(ProductQueryFilter filter, CancellationToken cancellationToken);
        Task<ApiResponse> AutoRefreshProducts(CancellationToken cancellationToken);
    }
}