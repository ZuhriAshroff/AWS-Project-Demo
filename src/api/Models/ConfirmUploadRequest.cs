namespace ImageGalleryApi.Models;

public record ConfirmUploadRequest(string Id, string ObjectKey, string FileName, string ContentType, long SizeBytes);
