using Product.Application.Constants;
using Product.Application.Enums;
using Shared.Utilities.Helpers;

namespace Product.Application.DTOs.APIDataFormatters
{
    public static class ResponseHandler
    {
        public static ApiResponse FailureResponse(string code, string message)
        {
            return new ApiResponse(code, ResponseStatusEnum.Failed.ToDescription(), message);
        }
        public static ApiResponse SuccessResponse(string message, string code, object data)
        {
            return new ApiResponse(code, ResponseStatusEnum.Successful.ToDescription(), message, data);
        }
        public static ApiResponse SuccessResponse(string message, object data)
        {
            return new ApiResponse(SuccessCodes.DEFAULT_SUCCESS_CODE, ResponseStatusEnum.Successful.ToDescription(), message, data);
        }
        public static ApiResponse SuccessResponse(string message)
        {
            return new ApiResponse(SuccessCodes.DEFAULT_SUCCESS_CODE, ResponseStatusEnum.Successful.ToDescription(), message);
        }
    }
}
