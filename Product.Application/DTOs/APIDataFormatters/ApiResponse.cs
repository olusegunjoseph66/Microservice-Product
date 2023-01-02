namespace Product.Application.DTOs.APIDataFormatters
{
    public partial class ApiResponse
    {
        public ApiResponse(string code, string status, string message, object? data = null)
        {
            Status = status;
            StatusCode = code;
            Message = message;
            Data = data;
        }

        public object Data { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
    }
}
