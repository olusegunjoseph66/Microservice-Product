namespace Product.Application.Constants
{
    public class ErrorMessages
    {
        internal const string DEFAULT_VALIDATION_MESSAGE = "Sorry, you have supplied one or more wrong inputs. Kindly check your input and try again.";
        internal const string DEFAULT_AUTHORIZATION_MESSAGE = "Sorry, you do not have the right to perform this operation.";

        public const string SERVER_ERROR = "Sorry, we are unable to fulfill your request at the moment, kindly try again later.";
        public const string DATABASE_CONFLICT_ERROR = "One or more unique fields already exist, kindly try again later.";
        public const string NOT_FOUND_ERROR = "Sorry, the resource you have requested for is not available at the moment.";
        

        public const string PRODUCT_NOT_FOUND = "Sorry, the Product you have requested operations to be performed on does not exist.";
        public const string UPDATE_FAILURE = "One or more unique fields already exist, kindly try again later.";
        public const string INVALID_OR_INCORRECT_VALUE_PROVIDED = "Sorry, you have supplied a wrong input. Kindly check and try again later.";
        public const string PRODUCT_IMAGE_NOTFOUND = "Sorry, the Product Image you have requested for is not available.";
        public const string BAD_IMAGE_UPLOADED = "Sorry, the Image you uploaded was in a wrong format.";
        public const string IMAGE_UPLOADING_FAILURE = "Sorry, we are unable to uplaod your image at the moment.";
    }
}
