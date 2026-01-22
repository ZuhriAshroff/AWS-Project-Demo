namespace ImageGalleryApi.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "ok",
            timeUtc = DateTime.UtcNow.ToString("o")
        }));
    }
}
