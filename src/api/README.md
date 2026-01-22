# Image Gallery API

ASP.NET Core Web API for S3 presigned URL image uploads with DynamoDB metadata.

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `AWS_REGION` | No | us-east-1 | AWS region |
| `S3_BUCKET` | Yes | - | S3 bucket name |
| `DDB_TABLE` | Yes | - | DynamoDB table name |
| `PRESIGN_TTL_SECONDS` | No | 600 | Presigned URL expiry |
| `MAX_UPLOAD_BYTES` | No | 10000000 | Max upload size (10MB) |

## How Presigned URLs Work

1. Client requests upload URL from API (`POST /api/images/presign-upload`)
2. API generates time-limited S3 presigned PUT URL with specific Content-Type
3. Client uploads file directly to S3 using presigned URL
4. Client confirms upload to API, which stores metadata in DynamoDB
5. For downloads, API generates presigned GET URL pointing to S3

## Run Locally

```bash
cd src/api
export S3_BUCKET=your-bucket
export DDB_TABLE=your-table
dotnet run
```

## AWS CLI Setup Commands

```bash
# 1. Verify your identity
aws sts get-caller-identity

# 2. Create bucket (if needed)
aws s3api create-bucket --bucket $S3_BUCKET --region us-east-1

# 3. Ensure bucket is private (Block Public Access)
aws s3api put-public-access-block --bucket $S3_BUCKET \
  --public-access-block-configuration \
  BlockPublicAcls=true,IgnorePublicAcls=true,BlockPublicPolicy=true,RestrictPublicBuckets=true

# 4. Apply bucket CORS (required for browser uploads)
aws s3api put-bucket-cors --bucket $S3_BUCKET --cors-configuration '{
  "CORSRules": [{
    "AllowedOrigins": ["http://localhost:5173", "http://localhost:5000"],
    "AllowedMethods": ["GET", "PUT"],
    "AllowedHeaders": ["*"],
    "ExposeHeaders": ["ETag"],
    "MaxAgeSeconds": 3000
  }]
}'

# 5. Create DynamoDB table (if needed)
aws dynamodb create-table \
  --table-name $DDB_TABLE \
  --attribute-definitions AttributeName=id,AttributeType=S \
  --key-schema AttributeName=id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST

# 6. Verify table exists
aws dynamodb describe-table --table-name $DDB_TABLE --query 'Table.TableStatus'
```

## API Endpoints

- `GET /api/health` - Health check
- `POST /api/images/presign-upload` - Get S3 upload URL
- `POST /api/images/confirm` - Save metadata
- `GET /api/images` - List images
- `GET /api/images/{id}/presign-download` - Get S3 download URL
