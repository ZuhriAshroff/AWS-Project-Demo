namespace ImageGalleryApi.Models;

public record PresignUploadRequest(string FileName, string ContentType, long SizeBytes);
