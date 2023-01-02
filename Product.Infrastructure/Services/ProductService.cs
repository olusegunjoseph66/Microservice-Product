using Shared.Data.Repository;
using Shared.Data.Models;
using Product.Application.Interfaces.Services;
using Product.Application.DTOs.APIDataFormatters;
using Shared.Utilities.DTO.Pagination;
using Product.Application.ViewModels.Responses;
using Product.Application.ViewModels.QueryFilters;
using Product.Application.DTOs.Sortings;
using Product.Application.DTOs.Filters;
using Product.Infrastructure.QueryObjects;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Extensions;
using Product.Application.Constants;
using Microsoft.Extensions.Configuration;
using Product.Application.Enums;
using Shared.Utilities.Helpers;
using Product.Application.ViewModels.Requests;
using Shared.ExternalServices.Interfaces;
using Shared.ExternalServices.DTOs;
using Product.Application.Exceptions;
using Product.Application.DTOs.Events;
using Product.Application.DTOs;
using System.Linq;
using Product.Application.Configurations.MicroservicesCall;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Product.Infrastructure.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IAsyncRepository<Shared.Data.Models.Product> _productRepository;
        public readonly IMessagingService _messageBus;
        private readonly IMemoryCache _cache;

        private readonly AppMicroservices _microServiceSetting;

        public ProductService(IAsyncRepository<Shared.Data.Models.Product> productRepository, IMemoryCache cache, IMessagingService messageBus, IOptions<AppMicroservices> microServiceSetting,
            IAuthenticatedUserService authenticatedUserService) : base(authenticatedUserService)
        {
            _cache = cache;
            _messageBus = messageBus;
            _productRepository = productRepository;

            _microServiceSetting = microServiceSetting.Value;
        }

        public async Task<ApiResponse> AddProducts(List<SapProductDto> products)
        {
            var productKey = "SapProducts";
            List<SapProductDto> productsToAdd = new();

            if (_cache.TryGetValue(productKey, out List<SapProductDto> cacheProducts))
            {
                cacheProducts.AddRange(products);
                productsToAdd = cacheProducts.DistinctBy(x => x.ProductSapNumber).ToList();

                _cache.Remove(productKey);
            }
            else
                productsToAdd = products;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(6))
                            .SetAbsoluteExpiration(TimeSpan.FromDays(30))
                            .SetPriority(CacheItemPriority.NeverRemove)
                            .SetSize(1024);
            _cache.Set(productKey, productsToAdd, cacheEntryOptions);

            return ResponseHandler.SuccessResponse("Product Successfully added to Memory", productsToAdd);
        }

        public async Task<ApiResponse> GetProducts(ProductQueryFilter filter, CancellationToken cancellationToken)
        {
            BasePageFilter pageFilter = new(filter.PageSize, filter.PageIndex);
            ProductSortingDto sorting = new();
            if (filter.Sort == ProductSortingEnum.NameDescending)
                sorting.IsNameDescending = true;
            else if (filter.Sort == ProductSortingEnum.NameAscending)
                sorting.IsNameAscending = true;

            #pragma warning disable CS8601 // Possible null reference assignment.
            ProductFilterDto productFilter = new()
            {
                CompanyCode = filter.CompanyCode,
                ProductStatusCode = filter.ProductStatusCode,
                SearchText = filter.SearchKeyword
            };
            #pragma warning restore CS8601 // Possible null reference assignment.

            var expression = new ProductQueryObject(productFilter).Expression;
            var orderExpression = ProcessOrderFunc(sorting);
            var query = _productRepository.Table.AsNoTrackingWithIdentityResolution()
                    .OrderByWhere(expression, orderExpression);
            var totalCount = await query.CountAsync(cancellationToken);

            query = query.Select(x => new Shared.Data.Models.Product
            {
                CompanyCode = x.CompanyCode,
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ProductType = x.ProductType,
                UnitOfMeasureCode = x.UnitOfMeasureCode,
                ProductImages = x.ProductImages.Select(i => new ProductImage
                {
                    Id = i.Id,
                    IsPrimaryImage = i.IsPrimaryImage,
                    PublicUrl = i.PublicUrl
                }).ToList(),
                ProductStatus = new ProductStatus
                {
                    Id = x.ProductStatus.Id,
                    Name = x.ProductStatus.Name,
                    Code = x.ProductStatus.Code
                },
                DateCreated = x.DateCreated,
                DateModified = x.DateModified
            }).Paginate(pageFilter.PageNumber, pageFilter.PageSize);

            var products = await query.ToListAsync(cancellationToken);
            var totalPages = NumberManipulator.PageCountConverter(totalCount, pageFilter.PageSize);
            var response = new PaginatedList<ProductResponse>(ProcessQuery(products), new PaginationMetaData(filter.PageIndex, filter.PageSize, totalPages, totalCount));

            return ResponseHandler.SuccessResponse(SuccessMessages.SUCCESSFUL_PRODUCT_LIST_RETRIEVAL, response);
        }

        public async Task<ApiResponse> GetProductById(int productId, CancellationToken cancellationToken)
        {
            var productDetail = await _productRepository.Table.Where(p => p.Id == (short)productId).Select(x => new Shared.Data.Models.Product
            {
                Id = x.Id, 
                Name = x.Name, 
                Description = x.Description, 
                ProductType = x.ProductType, 
                ProductStatus = new ProductStatus
                {
                    Id = x.ProductStatus.Id, 
                    Name = x.ProductStatus.Name, 
                    Code = x.ProductStatus.Code
                }, 
                UnitOfMeasureCode = x.UnitOfMeasureCode,
                Price = x.Price, 
                ProductSapNumber = x.ProductSapNumber,
                DateModified = x.DateModified, 
                ProductImages = x.ProductImages.Select(i => new ProductImage
                {
                    Id = i.Id, 
                    PublicUrl = i.PublicUrl, 
                    IsPrimaryImage = i.IsPrimaryImage
                }).ToList()
            }).FirstOrDefaultAsync(cancellationToken);

            var productResponse = ProcessQuery(productDetail);

            return ResponseHandler.SuccessResponse(SuccessMessages.SUCCESSFUL_PRODUCT_RETRIEVAL, productResponse);
        }

        public async Task<ApiResponse> ActivateDeactivateProduct(ActivateProductRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var productDetail = await _productRepository.Table.AsNoTrackingWithIdentityResolution().Where(p => p.Id == (short)request.ProductId).Include(x => x.ProductStatus).FirstOrDefaultAsync(cancellationToken);

            if (productDetail == null)
                throw new NotFoundException(ErrorMessages.PRODUCT_NOT_FOUND, ErrorCodes.PRODUCT_NOTFOUND);

            var responseMessage = string.Empty;
            var oldProduct = new Shared.Data.Models.Product
            {
                Id = productDetail.Id,
                Name = productDetail.Name,
                ProductSapNumber = productDetail.ProductSapNumber,
                DateCreated = productDetail.DateCreated,
                Description = productDetail.Description,
                UnitOfMeasureCode = productDetail.UnitOfMeasureCode,
                ProductStatus = new ProductStatus
                {
                    Code = productDetail.ProductStatus.Code,
                    Name = productDetail.ProductStatus.Name,
                    Id = productDetail.ProductStatus.Id
                }
            };

            if (request.Activate)
            {
                productDetail.ProductStatusId = (int)ProductStatusEnum.Active;
                productDetail.ProductStatus = new ProductStatus 
                {
                    Id = (byte)ProductStatusEnum.Active, 
                    Code = ProductStatusEnum.Active.ToString(), 
                    Name = ProductStatusEnum.Active.ToDescription() 
                };
                responseMessage = SuccessMessages.SUCCESSFUL_PRODUCT_ACTIVATION;
            }
            else
            {
                productDetail.ProductStatusId = (int)ProductStatusEnum.InActive;
                productDetail.ProductStatus = new ProductStatus 
                { 
                    Id = (byte)ProductStatusEnum.InActive, 
                    Code = ProductStatusEnum.InActive.ToString(),
                    Name = ProductStatusEnum.InActive.ToDescription() 
                };
                responseMessage = SuccessMessages.SUCCESSFUL_PRODUCT_DEACTIVATION;
            }

            productDetail.DateModified = DateTime.UtcNow;
            productDetail.ModifiedByUserId = userId;

            _productRepository.Update(productDetail);
            await _productRepository.CommitAsync(cancellationToken);

            //Publish to Azure ServiceBus
            ProductUpdatedMessage updatedProductMessage = new()
            {
                ProductId = oldProduct.Id,
                ProductSapNumber = oldProduct.ProductSapNumber,
                Name = oldProduct.Name,
                DateCreated = oldProduct.DateCreated,
                Description = oldProduct.Description,
                UnitOfMeasureCode = oldProduct.UnitOfMeasureCode,
                ProductStatus = new NameAndCode(oldProduct.ProductStatus.Code, oldProduct.Name)
            };

            await _messageBus.PublishTopicMessage(updatedProductMessage, EventMessages.PRODUCTS_PRODUCT_UPDATED);

            return ResponseHandler.SuccessResponse(responseMessage);
        }

        public async Task<ApiResponse> AutoRefreshProducts(CancellationToken cancellationToken)
        {
            // To be Refactored
            var client = new HttpClient();
            var endpoint = $"{_microServiceSetting.RData.BaseUrl}{_microServiceSetting.RData.GetCompaniesEndpoint}";
            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                throw new InternalServerException();

            TestClass<CompanyListResponse>? responseData = new();
            var content = await response.Content.ReadAsStringAsync();

            
            responseData = content.Deserialize<TestClass<CompanyListResponse>>();
            var companies = responseData.Data.Data;

            var companyCodes = companies.Companies.Select(x => x.Code).ToList();
            var sapProducts = GetSapProducts().Where(x => companyCodes.Contains(x.CompanyCode)).ToList();

            List<Shared.Data.Models.Product> newProducts = new();
            List<Shared.Data.Models.Product> productsToUpdate = new();

            if (sapProducts.Any())
            {
                var productStatuses = Enum.GetValues(typeof(ProductStatusEnum)).Cast<ProductStatusEnum>()
                   .Select(e => new NameAndId<byte>((byte)e, e.ToDescription())).ToList();

                var productsSapNumbers = sapProducts.Select(x => x.ProductSapNumber).ToList();
                var products = await _productRepository.Table.Where(x => productsSapNumbers.Contains(x.ProductSapNumber)).ToListAsync();

                companies.Companies.ForEach(x =>
                {
                    var companyProducts = sapProducts.Where(p => p.CompanyCode == x.Code).ToList();
                    if (companyProducts.Any())
                    {
                        companyProducts.ForEach(c =>
                        {
                            var productStatus = productStatuses.FirstOrDefault(s => s.Name == c.ProductStatus);
                            var productSelected = products.FirstOrDefault(p => p.ProductSapNumber == c.ProductSapNumber);
                            if (productSelected == null)
                            {

                                Shared.Data.Models.Product product = new()
                                {
                                    CompanyCode = c.CompanyCode,
                                    CountryCode = c.CountryCode,
                                    Name = c.Name,
                                    DateCreated = DateTime.UtcNow,
                                    DateRefreshed = DateTime.UtcNow,
                                    Description = c.Description,
                                    Price = c.Price,
                                    ProductSapNumber = c.ProductSapNumber,
                                    ProductType = c.ProductType,
                                    UnitOfMeasureCode = c.UnitOfMeasureCode,
                                    ProductStatusId = productStatus.Id
                                };
                                newProducts.Add(product);
                            }
                            else
                            {
                                productSelected.Name = c.Name;
                                productSelected.Description = c.Description;
                                productSelected.Price = c.Price;
                                productSelected.DateRefreshed = DateTime.UtcNow;
                                productSelected.CompanyCode = c.CompanyCode;
                                productSelected.ProductType = c.ProductType;
                                productSelected.CountryCode = c.CountryCode;
                                productSelected.UnitOfMeasureCode = c.UnitOfMeasureCode;
                                productSelected.ProductStatusId = productStatus.Id;
                                productsToUpdate.Add(productSelected);
                            }
                        });
                    }
                });

                if (newProducts.Any())
                    await _productRepository.AddRangeAsync(newProducts);

                if (productsToUpdate.Any())
                    _productRepository.UpdateRange(productsToUpdate);

                if (newProducts.Any() || productsToUpdate.Any())
                    await _productRepository.CommitAsync(cancellationToken);

                List<Shared.Data.Models.Product> productsCommited = new();
                if (newProducts.Any())
                    productsCommited.AddRange(newProducts);

                if (productsToUpdate.Any())
                    productsCommited.AddRange(productsToUpdate);

                if (productsCommited.Any())
                {
                    List<string> productRefreshMessages = new();
                    productsCommited.ForEach(x =>
                    {
                        var productStatus = productStatuses.FirstOrDefault(p => p.Id == x.ProductStatusId);
                        ProductRefreshedMessage productRefreshMessage = new()
                        {
                            Description = x.Description,
                            CompanyCode = x.CompanyCode,
                            CountryCode = x.CompanyCode,
                            DateRefreshed = x.DateRefreshed.Value,
                            DateCreated = x.DateCreated,
                            Name = x.Name,
                            Price = x.Price,
                            ProductId = x.Id,
                            UnitOfMeasureCode = x.UnitOfMeasureCode,
                            ProductSapNumber = x.ProductSapNumber,
                            ProductStatus = new NameAndCode(productStatus.Name, productStatus.Name), 
                            Id = Guid.NewGuid()
                        };
                        
                        productRefreshMessages.Add(JsonConvert.SerializeObject(productRefreshMessage));
                    });
                    await _messageBus.PublishTopicMessage(productRefreshMessages, EventMessages.PRODUCTS_PRODUCT_REFRESHED);
                }
                

            }

            return ResponseHandler.SuccessResponse(SuccessMessages.SUCCESSFUL_PRODUCT_REFRESH);
        }

        public async Task<ApiResponse> GetCacheProducts()
        {
            var sapProducts = GetSapProducts();
            return ResponseHandler.SuccessResponse("Cache Products Successfully fetched", sapProducts);
        }

        #region Private Methods
        private static Func<IQueryable<Shared.Data.Models.Product>, IOrderedQueryable<Shared.Data.Models.Product>> ProcessOrderFunc(ProductSortingDto? orderExpression = null)
        {
            IOrderedQueryable<Shared.Data.Models.Product> orderFunction(IQueryable<Shared.Data.Models.Product> queryable)
            {
                if (orderExpression == null)
                    return queryable.OrderByDescending(p => p.DateCreated);

                var orderQueryable = orderExpression.IsNameAscending
                   ? queryable.OrderBy(p => p.Name).ThenByDescending(p => p.DateCreated)
                   : orderExpression.IsNameDescending
                       ? queryable.OrderByDescending(p => p.Name).ThenByDescending(p => p.DateCreated)
                       : queryable.OrderByDescending(p => p.DateCreated);
                return orderQueryable;
            }
            return orderFunction;
        }

        private static IReadOnlyList<ProductResponse> ProcessQuery(IReadOnlyList<Shared.Data.Models.Product>? products)
        {
            return products.Select(p =>
            {
                string? fileUrl = p.ProductImages.Any()? p.ProductImages.FirstOrDefault(x => x.IsPrimaryImage)?.PublicUrl : null;
                var item = new ProductResponse(p.Id, p.Name, p.Description, p.ProductType, p.UnitOfMeasureCode, fileUrl, new NameAndCode(p.ProductStatus.Code, p.ProductStatus.Name), p.DateModified);
                return item;
            }).ToList();
        }

        private static ProductDetailResponse ProcessQuery(Shared.Data.Models.Product? product)
        {
            if (product == null)
                return null;

            var productResponse = new ProductDetailDto()
            {
                ProductId = product.Id,
                DateModified = product.DateModified,
                Description = product.Description,
                Name = product.Name,
                ProductType = product.ProductType, 
                Price = product.Price, 
                ProductSapNumber = product.ProductSapNumber,
                UnitOfMeasure = new NameAndCode(product.UnitOfMeasureCode, product.UnitOfMeasureCode),
                ProductStatus = new NameAndCode(product.ProductStatus.Code, product.ProductStatus.Name),
                ProductImages = !product.ProductImages.Any() ? new List<ProductImageResponse>() : product.ProductImages.Select(x => new ProductImageResponse
                {
                    IsPrimaryImage = x.IsPrimaryImage,
                    PublicUrl = x.PublicUrl
                }).ToList()
            };
            return new ProductDetailResponse(productResponse);
        }

        public List<SapProductDto> GetSapProducts()
        {
            var productKey = "SapProducts";

            if (_cache.TryGetValue(productKey, out List<SapProductDto> cacheProducts))
                return cacheProducts;

            return new List<SapProductDto>();
        }
        #endregion
    }
}
