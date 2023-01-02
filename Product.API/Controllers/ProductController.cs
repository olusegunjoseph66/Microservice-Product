using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.DTOs;
using Product.Application.DTOs.APIDataFormatters;
using Product.Application.ViewModels.QueryFilters;
using Product.Application.ViewModels.Requests;
using Product.Application.ViewModels.Responses;
using Product.Infrastructure.Services;
using Shared.Utilities.DTO.Pagination;

namespace Product.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public partial class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<EmptyResponse>))]
        public async Task<IActionResult> AddProducts([FromBody] List<SapProductDto> request, CancellationToken cancellationToken) => Response(await _productService.AddProducts(request));

        [HttpGet("cache-products")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<List<SapProductDto>>))]
        public async Task<IActionResult> GetCacheProducts(CancellationToken cancellationToken) => Response(await _productService.GetCacheProducts());

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<PaginatedList<ProductResponse>>))]
        public async Task<IActionResult> GetProducts([FromQuery]ProductQueryFilter filter, CancellationToken cancellationToken = default) => Response(await _productService.GetProducts(filter, cancellationToken));

        [HttpGet("{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<ProductDetailResponse>))]
        public async Task<IActionResult> ViewProductByproductId(int productId, CancellationToken cancellationToken) => Response(await _productService.GetProductById(productId, cancellationToken));

        [HttpPost("activate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<EmptyResponse>))]
        public async Task<IActionResult> ActivateOrDeactivateProducts([FromBody] ActivateProductRequest request, CancellationToken cancellationToken) => Response(await _productService.ActivateDeactivateProduct(request, cancellationToken));

        [HttpGet("autoRefresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SwaggerResponse<EmptyResponse>))]
        public async Task<IActionResult> AutoRefreshProducts(CancellationToken cancellationToken) => Response(await _productService.AutoRefreshProducts(cancellationToken));
    }
}
