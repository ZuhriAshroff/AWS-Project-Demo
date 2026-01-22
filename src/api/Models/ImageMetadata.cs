namespace ImageGalleryApi.Models;

public record ImageMetadata(
    string Id,
    string ObjectKey,
    string FileName,
    string ContentType,
    long SizeBytes,
    string CreatedAtUtc
);
