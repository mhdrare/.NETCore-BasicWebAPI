using System;

namespace BasicWebAPI.Models.View
{
    public class ApiErrorException : Exception
    {
        public ApiErrorException(ApiErrorResponse response)
        {
            Response = response;
        }

        public ApiErrorResponse Response { get; }
    }
}