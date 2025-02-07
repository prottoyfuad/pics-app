
namespace WebAppController.Middlewares {
    public class MyLogger {
        private readonly RequestDelegate _next;

        public MyLogger(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            {
                var auth = context.Request.Headers.Authorization;
                Console.WriteLine($"cookie: {auth.ToString().Length} {auth.ToString()}");
            }
            Console.WriteLine("\nBefore next: ");
            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
            Console.WriteLine($"Response: {context.Response.StatusCode} {context.Response.ContentType}");

            await _next(context);
            
            Console.WriteLine("\nAfter next: ");
            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} {context.Request.Query}");
            Console.WriteLine($"Response: {context.Response.StatusCode} {context.Response.ContentType}");
        }
    }
}
