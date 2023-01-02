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
using Product.Application.Configurations.MicroservicesCall;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Product.Application.DTOs;

namespace Product.Infrastructure.Services
{
    public class ProductImageService : BaseService, IProductImageService
    {
        private readonly IAsyncRepository<Shared.Data.Models.Product> _productRepository;
        private readonly IAsyncRepository<ProductImage> _productImagesRepository;
        public readonly IMessagingService _messageBus;
        private readonly IFileService _azureBlobStorage;
        private readonly IConfiguration _configuration;

        
        public ProductImageService(IAsyncRepository<Shared.Data.Models.Product> productRepository,
            IAsyncRepository<ProductImage> productImagesRepository,
            IConfiguration configuration, IMessagingService messageBus, IFileService azureBlobStorage,
            IAuthenticatedUserService authenticatedUserService) : base(authenticatedUserService)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _productRepository = productRepository;
            _productImagesRepository = productImagesRepository;
            _azureBlobStorage = azureBlobStorage;

        }

        public async Task<ApiResponse> AddProductImages(UploadProductImageRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var productDetail = await _productRepository.Table.Where(p => p.Id == (short)request.ProductId).Include(x => x.ProductStatus).FirstOrDefaultAsync(cancellationToken);

            if (productDetail == null)
                throw new NotFoundException(ErrorMessages.PRODUCT_NOT_FOUND, ErrorCodes.PRODUCT_NOTFOUND);

            var guid = RandomValueGenerator.GenerateRandomDigits(8);
            var filename = $"{request.ProductId}{guid}";

            (UploadResponse? upload, bool isValid) = await _azureBlobStorage.FileUpload(request.Image, cancellationToken);

            if (!isValid)
                throw new ValidationException(ErrorMessages.BAD_IMAGE_UPLOADED, ErrorCodes.DEFAULT_VALIDATION_CODE);

            if (upload == null)
                throw new InternalServerException(ErrorMessages.IMAGE_UPLOADING_FAILURE, ErrorCodes.SERVER_ERROR_CODE);

            if (request.IsLeadImage)
            {
                var productImages = await _productImagesRepository.Table
                    .Where(p => p.ProductId == request.ProductId && p.IsPrimaryImage).ToListAsync(cancellationToken);

                List<ProductImage> productImagesToUpdate = new();
                productImages.ForEach(x =>
                {
                    x.IsPrimaryImage = false;
                    productImagesToUpdate.Add(x);
                });
                _productImagesRepository.UpdateRange(productImagesToUpdate);
            }


            ProductImage newProductImage = new()
            {
                PublicUrl = upload.PublicUrl,
                CloudPath = upload.CloudUrl,
                DateCreated = DateTime.UtcNow,
                IsPrimaryImage = request.IsLeadImage,
                CreatedByUserId = userId,
                ProductId = (short)request.ProductId
            };

            await _productImagesRepository.AddAsync(newProductImage, cancellationToken);
            await _productImagesRepository.CommitAsync(cancellationToken);

            //Azure ServiceBus
            ProductImageUploadedMessage uploadedMessage = new()
            {
                ProductId = newProductImage.ProductId,
                CreatedByUserId = userId,
                DateCreated = DateTime.UtcNow,
                CloudPath = upload.CloudUrl,
                PublicUrl = upload.PublicUrl,
                ProductImageId = newProductImage.Id
            };

            await _messageBus.PublishTopicMessage(uploadedMessage, EventMessages.PRODUCTS_PRODUCT_IMAGE_UPLOADED);

            return ResponseHandler.SuccessResponse(SuccessMessages.SUCCESSFUL_PRODUCT_IMAGE_ADDITION);
        }

        public async Task<ApiResponse> RemoveProductImages(int productImageId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            if (productImageId == 0)
                throw new ValidationException(ErrorMessages.INVALID_OR_INCORRECT_VALUE_PROVIDED, ErrorCodes.INVALID_PRODUCT_REQUEST);

            var productImage = await _productImagesRepository.GetByIdAsync(productImageId, cancellationToken);
            if (productImage == null)
                throw new NotFoundException(ErrorMessages.PRODUCT_IMAGE_NOTFOUND, ErrorCodes.PRODUCT_IMAGE_NOTFOUND);

            await _azureBlobStorage.DeleteFile(productImage.CloudPath);

            if (productImage.IsPrimaryImage)
            {
                var productImages = await _productImagesRepository.Table
                    .Where(p => p.ProductId == productImage.ProductId && !p.IsPrimaryImage)
                    .OrderByDescending(pi => pi.DateCreated).ToListAsync();

                if (productImages.Any())
                {
                    var newleadImage = productImages.First();

                    newleadImage.IsPrimaryImage = true;
                    _productImagesRepository.Update(newleadImage);
                }
            }

            _productImagesRepository.Delete(productImage);
            await _productImagesRepository.CommitAsync(cancellationToken);

            //Azure ServiceBus
            ProductDeletedMessage deletedMessage = new()
            {
                DateDeleted = DateTime.UtcNow,
                DeletedByUserId = userId,
                DateCreated = DateTime.UtcNow,
                ProductImageId = productImageId
            };

            await _messageBus.PublishTopicMessage(deletedMessage, EventMessages.PRODUCTS_PRODUCT_IMAGE_DELETED);

            return ResponseHandler.SuccessResponse(SuccessMessages.SUCCESSFUL_PRODUCT_IMAGE_REMOVAL);
        }

    }
}
