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

        var allowed = new[]
        {
            "/",
            "/home",
            "/home/index",
            "/login",
            "/register",
            "/css",
            "/js",
            "/images",
            "/lib"
        };

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