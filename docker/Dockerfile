# Build stage for .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src

# Copy solution and project files
COPY ["Resizarr.sln", "./"]
COPY ["src/Resizarr.Api/Resizarr.Api.csproj", "src/Resizarr.Api/"]
COPY ["src/Resizarr.Application/Resizarr.Application.csproj", "src/Resizarr.Application/"]
COPY ["src/Resizarr.Core/Resizarr.Core.csproj", "src/Resizarr.Core/"]
COPY ["src/Resizarr.Infrastructure/Resizarr.Infrastructure.csproj", "src/Resizarr.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Resizarr.Api/Resizarr.Api.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/Resizarr.Api"
RUN dotnet build "Resizarr.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Resizarr.Api.csproj" -c Release -o /app/publish

# Build stage for React frontend
FROM node:20-alpine AS build-web
WORKDIR /app

# Copy package files
COPY ["src/Resizarr.Web/package*.json", "./"]

# Install dependencies
RUN npm install

# Copy source code
COPY ["src/Resizarr.Web/", "./"]

# Build
RUN npm run build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install FFmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*

# Copy API build
COPY --from=build-api /app/publish .

# Copy React build
COPY --from=build-web /app/dist ./wwwroot

# Create data directory
RUN mkdir -p /data

# Expose ports
EXPOSE 5200

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5200
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "Resizarr.Api.dll"]
