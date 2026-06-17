using System.Net;
using System.Text.Json;

namespace SmartLedgerAPI.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> logger;
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }
          

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
                //passes request if nothing crashes normal
            }
            catch(Exception ex)
            {
                var errorId = Guid.NewGuid();

                logger.LogError(ex , ex.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var errorResponseObject = new
                {
                    ErrorId = errorId,
                    ErrorMessage = "Something Went Wrong , We'll Resolve It Asap!!!!!"
                };

                //await httpContext.Response.WriteAsJsonAsync(errorResponseObject);

                var JsonResponse = JsonSerializer.Serialize(errorResponseObject);
                await httpContext.Response.WriteAsync(JsonResponse);

            }
        }
    }
}
