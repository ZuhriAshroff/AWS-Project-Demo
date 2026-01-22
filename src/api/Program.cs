using Amazon.DynamoDBv2;
using Amazon.S3;
using ImageGalleryApi.Config;
using ImageGalleryApi.Endpoints;
using ImageGalleryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var config = new AppConfig();
builder.Services.AddSingleton(config);

// AWS SDK clients (uses default credential provider chain)
var region = Amazon.RegionEndpoint.GetBySystemName(config.AwsRegion);
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(region));
builder.Services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(region));

// Application services
builder.Services.AddSingleton<S3Service>();
builder.Services.AddSingleton<DynamoDbService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// Serve static files (React build in wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Map endpoints
app.MapHealthEndpoints();
app.MapImageEndpoints();

// SPA fallback
app.MapFallbackToFile("index.html");

app.Run();
