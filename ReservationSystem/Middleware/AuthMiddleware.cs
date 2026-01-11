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

        // Ścieżki publiczne
        var allowed = new[]
        {
            "/",
            "/home",
            "/home/index",
            "/login",
            "/login/index",
            "/register",
            "/register/index"
        };

        // Pliki statyczne
        if (path.StartsWith("/css") ||
            path.StartsWith("/js") ||
            path.StartsWith("/images") ||
            path.StartsWith("/lib"))
        {
            await _next(context);
            return;
        }

        // Strony publiczne
        if (allowed.Any(a => path.StartsWith(a)))
        {
            await _next(context);
            return;
        }

        // Wymagana sesja
        if (context.Session.GetInt32("UserId") == null)
        {
            context.Response.Redirect("/Login");
            return;
        }

        await _next(context);
    }
}