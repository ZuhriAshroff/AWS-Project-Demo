namespace ImageGalleryApi.Config;

public class AppConfig
{
    public string AwsRegion { get; }
    public string S3Bucket { get; }
    public string DdbTable { get; }
    public int PresignTtlSeconds { get; }
    public long MaxUploadBytes { get; }

    public AppConfig()
    {
        AwsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
        S3Bucket = Environment.GetEnvironmentVariable("S3_BUCKET") 
            ?? throw new InvalidOperationException("S3_BUCKET is required");
        DdbTable = Environment.GetEnvironmentVariable("DDB_TABLE") 
            ?? throw new InvalidOperationException("DDB_TABLE is required");
        PresignTtlSeconds = int.TryParse(Environment.GetEnvironmentVariable("PRESIGN_TTL_SECONDS"), out var ttl) 
            ? ttl : 600;
        MaxUploadBytes = long.TryParse(Environment.GetEnvironmentVariable("MAX_UPLOAD_BYTES"), out var max) 
            ? max : 10_000_000;
    }
}
