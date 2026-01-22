using System.Text.RegularExpressions;
using ImageGalleryApi.Config;
using ImageGalleryApi.Models;
using ImageGalleryApi.Services;

namespace ImageGalleryApi.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var config = app.Services.GetRequiredService<AppConfig>();

        app.MapPost("/api/images/presign-upload", (PresignUploadRequest request, S3Service s3) =>
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
                return Results.BadRequest(new { error = "fileName is required" });
            if (string.IsNullOrWhiteSpace(request.ContentType) || !request.ContentType.StartsWith("image/"))
                return Results.BadRequest(new { error = "contentType must start with 'image/'" });
            if (request.SizeBytes <= 0)
                return Results.BadRequest(new { error = "sizeBytes must be positive" });
            if (request.SizeBytes > config.MaxUploadBytes)
                return Results.BadRequest(new { error = $"sizeBytes exceeds max of {config.MaxUploadBytes}" });

            var id = Guid.NewGuid().ToString().ToLowerInvariant();
            var sanitizedFileName = SanitizeFileName(request.FileName);
            var objectKey = $"images/{id}/{sanitizedFileName}";
            var uploadUrl = s3.GeneratePresignedUploadUrl(objectKey, request.ContentType);

            return Results.Ok(new PresignUploadResponse(id, objectKey, uploadUrl, s3.ExpiresInSeconds));
        });

        app.MapPost("/api/images/confirm", async (ConfirmUploadRequest request, DynamoDbService dynamo) =>
        {
            if (!Guid.TryParse(request.Id, out _))
                return Results.BadRequest(new { error = "id must be a valid GUID" });
            if (string.IsNullOrWhiteSpace(request.ObjectKey))
                return Results.BadRequest(new { error = "objectKey is required" });
            if (!request.ObjectKey.StartsWith($"images/{request.Id}/"))
                return Results.BadRequest(new { error = "objectKey must match pattern images/{id}/" });
            if (string.IsNullOrWhiteSpace(request.FileName))
                return Results.BadRequest(new { error = "fileName is required" });
            if (string.IsNullOrWhiteSpace(request.ContentType))
                return Results.BadRequest(new { error = "contentType is required" });
            if (request.SizeBytes <= 0)
                return Results.BadRequest(new { error = "sizeBytes must be positive" });

            var metadata = new ImageMetadata(
                Id: request.Id,
                ObjectKey: request.ObjectKey,
                FileName: request.FileName,
                ContentType: request.ContentType,
                SizeBytes: request.SizeBytes,
                CreatedAtUtc: DateTime.UtcNow.ToString("o")
            );

            await dynamo.SaveImageMetadataAsync(metadata);
            return Results.Ok(new { ok = true });
        });

        app.MapGet("/api/images", async (DynamoDbService dynamo) =>
        {
            var images = await dynamo.ListImagesAsync();
            return Results.Ok(images);
        });

        app.MapGet("/api/images/{id}/presign-download", async (string id, S3Service s3, DynamoDbService dynamo) =>
        {
            if (!Guid.TryParse(id, out _))
                return Results.BadRequest(new { error = "id must be a valid GUID" });

            var image = await dynamo.GetImageByIdAsync(id);
            if (image == null)
                return Results.NotFound(new { error = "Image not found" });

            var downloadUrl = s3.GeneratePresignedDownloadUrl(image.ObjectKey);
            return Results.Ok(new PresignDownloadResponse(downloadUrl, s3.ExpiresInSeconds));
        });
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove path separators and trim
        var name = Path.GetFileName(fileName).Trim();
        // Replace invalid chars with underscore
        var sanitized = Regex.Replace(name, @"[^a-zA-Z0-9._-]", "_");
        // Limit to 120 chars
        if (sanitized.Length > 120)
            sanitized = sanitized[..120];
        return string.IsNullOrEmpty(sanitized) ? "file" : sanitized;
    }
}
