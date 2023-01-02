namespace Product.Application.Constants
{
    public class ErrorCodes
    {
        public const string DEFAULT_VALIDATION_CODE = "Error-P-03";
        public const string DEFAULT_AUTHORIZATION_CODE = "E02";
        public const string DATABASE_INSERT_CONFLICT_CODE = "E03";
        public const string SERVER_ERROR_CODE = "E04";
        public const string NOTFOUND_ERROR_CODE = "E05";

        public const string PRODUCT_NOTFOUND = "Error-P-01";
        public const string UPDATE_FAILURE = "Error-P-02";
        public const string INVALID_PRODUCT_REQUEST = "Error-P-03";
        public const string PRODUCT_IMAGE_NOTFOUND = "Error-P-04";

        public const int SqlServerViolationOfUniqueIndex = 2601;
        public const int SqlServerViolationOfUniqueConstraint = 2627;
    }
}
