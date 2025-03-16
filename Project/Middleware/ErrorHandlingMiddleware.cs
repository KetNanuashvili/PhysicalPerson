
    using Microsoft.AspNetCore.Http;


    namespace Shared.Middleware
    {
        public class ErrorHandlingMiddleware
        {
            private readonly RequestDelegate _next;  

        
            public ErrorHandlingMiddleware(RequestDelegate next)
            {
                _next = next; 
            }

            // Middleware ლოგიკა
            public async Task InvokeAsync(HttpContext httpContext)
            {
                try
                {
                  
                    await _next(httpContext);  
                }
                catch (Exception ex)
                {
                    
                    httpContext.Response.StatusCode = 500;
                    await httpContext.Response.WriteAsync($"An error occurred: {ex.Message}");
                }
            }
        }
    }


