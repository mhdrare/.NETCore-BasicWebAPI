using System;
using System.Threading.Tasks;
using BasicWebAPI.Models.View;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BasicWebAPI.Middlewares
{
    public class ApiExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        public ApiExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (System.Exception ex)
            {
                await handleException(httpContext, ex);
            }
        }

        private Task handleException(HttpContext httpContext, Exception ex)
        {
            var error = ex is ApiErrorException ? 
                ((ApiErrorException)ex).Response : 
                ApiError.GenericError.AddMessageParameter(ex.Message);
            httpContext.Response.StatusCode = error.httpStatusCode;
            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(error));
        }
    }
}