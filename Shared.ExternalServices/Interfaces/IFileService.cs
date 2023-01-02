using Microsoft.AspNetCore.Http;
using Shared.ExternalServices.DTOs;
using Shared.ExternalServices.Enums;

namespace Shared.ExternalServices.Interfaces
{
    public interface IFileService
    {
        Task<UploadResponse> FileUpload(IFormFile file, CancellationToken cancellationToken);
        Task<(UploadResponse?, bool)> FileUpload(string base64String, CancellationToken cancellationToken);

        Task DeleteFile(string cloudpath);
    }
}
