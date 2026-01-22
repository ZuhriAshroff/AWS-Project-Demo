namespace ImageGalleryApi.Models;

public record PresignUploadResponse(string Id, string ObjectKey, string UploadUrl, int ExpiresInSeconds);
