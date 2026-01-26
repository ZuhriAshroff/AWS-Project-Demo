# ---------- Build React ----------
FROM node:20-alpine AS web-build
WORKDIR /app/src/web

COPY src/web/package*.json ./
RUN npm ci

COPY src/web/ ./
RUN npm run build


# ---------- Build .NET API ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS api-build
WORKDIR /app

COPY src/api/*.csproj ./src/api/
RUN dotnet restore ./src/api

COPY src/api/ ./src/api/

# Copy React build into API wwwroot
RUN rm -rf ./src/api/wwwroot && mkdir -p ./src/api/wwwroot
COPY --from=web-build /app/src/web/dist/ ./src/api/wwwroot/

RUN dotnet publish ./src/api -c Release -o /out


# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=api-build /out ./

ENTRYPOINT ["dotnet", "ImageGalleryApi.dll"]
