namespace ImageGalleryApi.Models;

public record PresignDownloadResponse(string DownloadUrl, int ExpiresInSeconds);
