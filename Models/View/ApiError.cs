namespace BasicWebAPI.Models.View
{
    public class ApiError
    {
        public static ApiErrorResponse DataNotFound = new ApiErrorResponse ("Tidak ditemukan dengan id {0}")
        {
            httpStatusCode = 404,
            appStatusCode = 10001
        };

        public static ApiErrorResponse GenericError = new ApiErrorResponse ("Something wrong!")
        {
            httpStatusCode = 500,
            appStatusCode = 11111
        };
    }
}