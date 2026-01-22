using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ImageGalleryApi.Config;
using ImageGalleryApi.Models;

namespace ImageGalleryApi.Services;

public class DynamoDbService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;

    public DynamoDbService(IAmazonDynamoDB dynamoDb, AppConfig config)
    {
        _dynamoDb = dynamoDb;
        _tableName = config.DdbTable;
    }

    public async Task SaveImageMetadataAsync(ImageMetadata metadata)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["id"] = new() { S = metadata.Id },
            ["objectKey"] = new() { S = metadata.ObjectKey },
            ["fileName"] = new() { S = metadata.FileName },
            ["contentType"] = new() { S = metadata.ContentType },
            ["sizeBytes"] = new() { N = metadata.SizeBytes.ToString() },
            ["createdAtUtc"] = new() { S = metadata.CreatedAtUtc }
        };
        await _dynamoDb.PutItemAsync(_tableName, item);
    }

    public async Task<List<ImageMetadata>> ListImagesAsync()
    {
        var response = await _dynamoDb.ScanAsync(new ScanRequest { TableName = _tableName });
        var images = response.Items.Select(item => new ImageMetadata(
            Id: item["id"].S,
            ObjectKey: item["objectKey"].S,
            FileName: item["fileName"].S,
            ContentType: item["contentType"].S,
            SizeBytes: long.Parse(item["sizeBytes"].N),
            CreatedAtUtc: item["createdAtUtc"].S
        )).ToList();

        return images.OrderByDescending(i => i.CreatedAtUtc).ToList();
    }

    public async Task<ImageMetadata?> GetImageByIdAsync(string id)
    {
        var response = await _dynamoDb.GetItemAsync(_tableName, new Dictionary<string, AttributeValue>
        {
            ["id"] = new() { S = id }
        });

        if (response.Item == null || response.Item.Count == 0)
            return null;

        var item = response.Item;
        return new ImageMetadata(
            Id: item["id"].S,
            ObjectKey: item["objectKey"].S,
            FileName: item["fileName"].S,
            ContentType: item["contentType"].S,
            SizeBytes: long.Parse(item["sizeBytes"].N),
            CreatedAtUtc: item["createdAtUtc"].S
        );
    }
}
