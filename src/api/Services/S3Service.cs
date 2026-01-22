using Amazon.S3;
using Amazon.S3.Model;
using ImageGalleryApi.Config;

namespace ImageGalleryApi.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly int _ttlSeconds;

    public S3Service(IAmazonS3 s3Client, AppConfig config)
    {
        _s3Client = s3Client;
        _bucketName = config.S3Bucket;
        _ttlSeconds = config.PresignTtlSeconds;
    }

    public string GeneratePresignedUploadUrl(string objectKey, string contentType)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddSeconds(_ttlSeconds),
            ContentType = contentType
        };
        return _s3Client.GetPreSignedURL(request);
    }

    public string GeneratePresignedDownloadUrl(string objectKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddSeconds(_ttlSeconds)
        };
        return _s3Client.GetPreSignedURL(request);
    }

    public int ExpiresInSeconds => _ttlSeconds;
}
