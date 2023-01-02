using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs.APIDataFormatters;
using Product.Application.Interfaces.Services;
using Product.Application.ViewModels.QueryFilters;
using Product.Application.ViewModels.Requests;
using Product.Application.ViewModels.Responses;
using Product.Infrastructure.Services;
using Shared.Utilities.DTO.Pagination;

namespace Product.API.Controllers
{
    [Route("api/v1/product/image")]
    [ApiController]
    public class ProductImageController : BaseController
    {
        private readonly IProductImageService _productImageService;
        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }
        
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<EmptyResponse>))]
        public async Task<IActionResult> UploadProductImages([FromBody] UploadProductImageRequest request, CancellationToken cancellationToken) => Response(await _productImageService.AddProductImages(request, cancellationToken));

        [HttpDelete("{productImageId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<EmptyResponse>))]
        public async Task<IActionResult> RemoveProductImage(int productImageId, CancellationToken cancellationToken) => Response(await _productImageService.RemoveProductImages(productImageId, cancellationToken));
    }
}
