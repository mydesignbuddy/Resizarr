# Resizarr

"Resizarr" uses .NET 8 and React to automate library optimization. It monitors folders, analyzes media via FFprobe, and triggers FFmpeg transcoding based on custom Quality Profiles (e.g., 4K→1080p). Features include an Activity Queue, hardware acceleration, and atomic file replacement, all wrapped in the familiar *arr ecosystem UI.

## Features

- 📁 **Folder Monitoring**: Automatically detect new media files
- 🎬 **Media Analysis**: Extract metadata using FFprobe
- ⚙️ **Quality Profiles**: Define custom transcoding rules
- 🚀 **Hardware Acceleration**: Support for NVENC, QSV, VAAPI
- 📊 **Activity Queue**: Monitor transcoding jobs in real-time
- 🔄 **Atomic File Replacement**: Safe transcoding with rollback
- 🎨 **Modern UI**: React-based web interface
- 📝 **REST API**: Full-featured API with Swagger documentation

## Architecture

- **Backend**: .NET 8 with ASP.NET Core Web API
- **Database**: SQLite with Entity Framework Core
- **Frontend**: React 18 with TypeScript and Vite
- **External Tools**: FFmpeg and FFprobe

## Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- FFmpeg and FFprobe installed

### Backend Setup

```bash
cd src/Resizarr.Api
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5200`  
Swagger documentation at `http://localhost:5200`

### Frontend Setup

```bash
cd src/Resizarr.Web
npm install
npm run dev
```

The React app will be available at `http://localhost:5173`

## Project Structure

```
Resizarr/
├── src/
│   ├── Resizarr.Api/             # Web API
│   ├── Resizarr.Application/     # Application layer
│   ├── Resizarr.Core/            # Domain entities & interfaces
│   ├── Resizarr.Infrastructure/  # Data access & services
│   └── Resizarr.Web/             # React frontend
├── PROJECT_PLAN.md               # Detailed project plan
└── README.md
```

## API Endpoints

- `/api/v1/qualityprofiles` - Manage quality profiles
- `/api/v1/media` - Media file operations
- `/api/v1/jobs` - Transcoding job management
- `/api/v1/folders` - Folder configuration
- `/api/v1/system` - System status and health
- `/api/v1/dashboard` - Dashboard statistics

## Development

### Build Solution

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Database Migrations

```bash
cd src/Resizarr.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Configuration

Edit `src/Resizarr.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=resizarr.db"
  },
  "Resizarr": {
    "MaxConcurrentJobs": 2,
    "TempDirectory": "/tmp/resizarr",
    "FFmpegPath": "/usr/bin/ffmpeg",
    "FFprobePath": "/usr/bin/ffprobe",
    "EnableHardwareAcceleration": true
  }
}
```

## Contributing

See [PROJECT_PLAN.md](PROJECT_PLAN.md) for the full development roadmap and architecture details.

## License

MIT
