namespace ReservationSystem.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value.ToLower();

        // Strony dostępne bez logowania
        var allowed = new[] { "/login", "/register", "/css", "/js" };

        if (!allowed.Any(a => path.StartsWith(a)))
        {
            if (context.Session.GetInt32("UserId") == null)
            {
                context.Response.Redirect("/Login");
                return;
            }
        }

        await _next(context);
    }
}