# AWS Image Gallery

Full-stack image gallery with S3 presigned URL uploads and DynamoDB metadata storage.

## Structure

```
src/
  api/    # ASP.NET Core Web API (.NET 8)
  web/    # React + Vite + TypeScript
```

## Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `AWS_REGION` | No | us-east-1 | AWS region |
| `S3_BUCKET` | Yes | - | S3 bucket for images |
| `DDB_TABLE` | Yes | - | DynamoDB table name |
| `GIT_SHA` | No | unknown | Git commit for health endpoint |

AWS credentials are resolved via standard SDK chain (env vars, ~/.aws/credentials, instance role).

## Local Development

### Backend (API)

```bash
cd src/api
export S3_BUCKET=your-bucket-name
export DDB_TABLE=your-table-name
dotnet run
# Runs on http://localhost:5000
```

### Frontend

```bash
cd src/web
npm install
npm run dev
# Runs on http://localhost:5173 with API proxy to :5000
```

## Production Build

Build frontend and copy to API's wwwroot:

```bash
cd src/web
npm run build
cp -r dist/* ../api/wwwroot/
cd ../api
dotnet run
# Serves both API and frontend on :5000
```

## Docker

```bash
docker build -t image-gallery .
docker run -p 5000:5000 \
  -e S3_BUCKET=your-bucket \
  -e DDB_TABLE=your-table \
  -e AWS_ACCESS_KEY_ID=xxx \
  -e AWS_SECRET_ACCESS_KEY=xxx \
  image-gallery
```

## API Endpoints

- `GET /api/health` - Health check
- `POST /api/images/presign-upload` - Get S3 upload URL
- `POST /api/images/confirm` - Save metadata after upload
- `GET /api/images` - List all images
- `GET /api/images/{id}/presign-download` - Get S3 download URL

## DynamoDB Table Schema

- Partition Key: `id` (String)

No sort key required. Table requires attributes: id, objectKey, fileName, contentType, sizeBytes, createdAtUtc.
