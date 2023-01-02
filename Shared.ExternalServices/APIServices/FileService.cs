using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shared.ExternalServices.Configurations;
using Shared.ExternalServices.DTOs;
using Shared.ExternalServices.Enums;
using Shared.ExternalServices.Interfaces;
using Shared.Utilities.Helpers;
using System;
using System.Reflection.Metadata;

namespace Shared.ExternalServices.APIServices
{
    public class FileService : IFileService
    {
        private readonly FileServiceSetting _fileSetting;

        public FileService(IOptions<FileServiceSetting> fileSetting)
        {
            _fileSetting = fileSetting.Value;
        }

        public async Task<UploadResponse> FileUpload(IFormFile file, CancellationToken cancellationToken)
        {
            var memory = new MemoryStream();
            await file.CopyToAsync(memory, cancellationToken);
            memory.Position = 0;

            string generatedFileName =  $"{DateTime.Now.Ticks}.{file.FileName.Split('.').Last()}";

            var blobClient = new BlobContainerClient(_fileSetting.ConnectionString, _fileSetting.ContainerName);
            var blob = blobClient.GetBlobClient(generatedFileName);
            await blob.UploadAsync(memory, cancellationToken);

            UploadResponse fileUrl = new()
            {
                CloudUrl = blob.Uri.AbsoluteUri,
                PublicUrl = blob.Uri.AbsoluteUri
            };
            memory.Close();
            return fileUrl;
        }

        public async Task<(UploadResponse?, bool)> FileUpload(string base64String, CancellationToken cancellationToken)
        {
            if (!base64String.IsValidBase64String())
                return new(null, false);

            var extension = base64String.GetExtension();
            var base64FileString = base64String.GetBase64String();
            var byteArray = Convert.FromBase64String(base64FileString);

            var memory = new MemoryStream(byteArray, 0, byteArray.Length);

            string generatedFileName = $"{DateTime.Now.Ticks}{extension}";

            var blobClient = new BlobContainerClient(_fileSetting.ConnectionString, _fileSetting.ContainerName);
            var blob = blobClient.GetBlobClient(generatedFileName);
            await blob.UploadAsync(memory, cancellationToken);

            if (string.IsNullOrWhiteSpace(blob.Uri.AbsoluteUri))
                return new(null, false);

            UploadResponse fileUrl = new()
            {
                CloudUrl = blob.Uri.AbsoluteUri,
                PublicUrl = blob.Uri.AbsoluteUri
            };
            memory.Close();
            return (fileUrl, true);
        }

        public async Task DeleteFile(string imageUrl)
        {
            var blobClient = new BlobContainerClient(_fileSetting.ConnectionString, _fileSetting.ContainerName);
            var blob = blobClient.GetBlobClient(imageUrl);
            await blob.DeleteIfExistsAsync();
        }
    }
}
