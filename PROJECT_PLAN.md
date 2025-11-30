# Resizarr Project Plan

## Overview
Resizarr is a .NET 8 and React application designed to automate video transcoding by monitoring folders, analyzing media files via FFprobe, and executing FFmpeg transcoding jobs based on user-defined Quality Profiles. The system follows the *arr ecosystem (Sonarr/Radarr) design patterns.

## Architecture

### High-Level Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                    React Frontend (SPA)                      │
│  - Dashboard - Quality Profiles - Activity Queue - Settings  │
└─────────────────────────┬───────────────────────────────────┘
                          │ REST API
┌─────────────────────────┴───────────────────────────────────┐
│                    .NET 8 Web API                            │
│  - Controllers - Authentication - Validation                 │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│               Application Layer (CQRS)                       │
│  - Commands/Queries - Business Logic - DTOs                  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│                   Core/Domain Layer                          │
│  - Entities - Interfaces - Domain Logic - Events             │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────┴───────────────────────────────────┐
│              Infrastructure Layer                            │
│  - EF Core/SQLite - File System - FFmpeg/FFprobe Services   │
│  - Background Services - Task Queue - Hardware Acceleration  │
└─────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Folder Monitoring Service
- **Technology**: FileSystemWatcher with BackgroundService
- **Responsibilities**:
  - Monitor configured folders for new/modified video files
  - Trigger media analysis on file detection
  - Handle file locking and retry logic
  - Support multiple folder configurations

### 2. Media Analysis Service (FFprobe)
- **Responsibilities**:
  - Execute FFprobe to extract media metadata
  - Parse video codec, resolution, bitrate, audio tracks
  - Determine if file matches Quality Profile criteria
  - Store media information in database

### 3. Transcoding Service (FFmpeg)
- **Responsibilities**:
  - Execute FFmpeg with appropriate parameters
  - Support hardware acceleration (NVENC, QSV, etc.)
  - Monitor transcoding progress
  - Implement atomic file replacement
  - Handle errors and retries

### 4. Task Queue System
- **Technology**: Custom queue with BackgroundService or Hangfire
- **Responsibilities**:
  - Queue transcoding jobs
  - Manage concurrent transcoding (configurable limit)
  - Priority-based job execution
  - Pause/resume/cancel capabilities
  - Job status tracking and history

### 5. Quality Profiles
- **Configuration**:
  - Name and description
  - Target resolution (e.g., 1080p, 720p, 480p)
  - Target codec (H.264, H.265/HEVC, VP9, AV1)
  - Bitrate limits (min/max)
  - Audio codec and settings
  - Hardware acceleration preference
  - Source file criteria (trigger conditions)

## Database Schema

### SQLite Database Structure

#### QualityProfiles
```sql
CREATE TABLE QualityProfiles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Description TEXT,
    Enabled BOOLEAN NOT NULL DEFAULT 1,
    
    -- Video Settings
    TargetResolution TEXT NOT NULL, -- e.g., "1920x1080", "1280x720"
    TargetCodec TEXT NOT NULL, -- e.g., "h264", "hevc", "vp9"
    TargetBitrate INTEGER, -- in kbps
    MinBitrate INTEGER,
    MaxBitrate INTEGER,
    TargetFrameRate REAL, -- null means keep source
    PixelFormat TEXT,
    
    -- Audio Settings
    AudioCodec TEXT NOT NULL,
    AudioBitrate INTEGER,
    AudioChannels INTEGER,
    
    -- Hardware Acceleration
    HardwareAcceleration TEXT, -- "nvenc", "qsv", "vaapi", "none"
    
    -- Trigger Conditions
    MinSourceWidth INTEGER, -- Only process if source >= this width
    MinSourceHeight INTEGER,
    SourceCodecs TEXT, -- Comma-separated list of codecs to process
    
    -- Metadata
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);
```

#### MediaFiles
```sql
CREATE TABLE MediaFiles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FilePath TEXT NOT NULL UNIQUE,
    FileName TEXT NOT NULL,
    FolderPath TEXT NOT NULL,
    
    -- File Information
    FileSize INTEGER NOT NULL,
    FileHash TEXT,
    
    -- Video Metadata
    VideoCodec TEXT,
    VideoWidth INTEGER,
    VideoHeight INTEGER,
    VideoBitrate INTEGER,
    VideoFrameRate REAL,
    Duration REAL, -- in seconds
    
    -- Audio Metadata
    AudioCodec TEXT,
    AudioBitrate INTEGER,
    AudioChannels INTEGER,
    AudioSampleRate INTEGER,
    
    -- Processing Status
    Status TEXT NOT NULL, -- "Pending", "Analyzing", "Queued", "Processing", "Completed", "Failed", "Skipped"
    QualityProfileId INTEGER,
    
    -- Timestamps
    DiscoveredAt TEXT NOT NULL,
    LastScannedAt TEXT,
    ProcessedAt TEXT,
    
    FOREIGN KEY (QualityProfileId) REFERENCES QualityProfiles(Id)
);
```

#### TranscodingJobs
```sql
CREATE TABLE TranscodingJobs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MediaFileId INTEGER NOT NULL,
    QualityProfileId INTEGER NOT NULL,
    
    -- Job Configuration
    SourcePath TEXT NOT NULL,
    OutputPath TEXT NOT NULL,
    TempPath TEXT NOT NULL,
    FFmpegCommand TEXT NOT NULL,
    
    -- Status
    Status TEXT NOT NULL, -- "Queued", "Running", "Completed", "Failed", "Cancelled"
    Priority INTEGER NOT NULL DEFAULT 0,
    Progress REAL DEFAULT 0, -- 0-100
    
    -- Performance Metrics
    StartedAt TEXT,
    CompletedAt TEXT,
    ProcessingTime INTEGER, -- in seconds
    OutputFileSize INTEGER,
    
    -- Error Handling
    ErrorMessage TEXT,
    RetryCount INTEGER DEFAULT 0,
    MaxRetries INTEGER DEFAULT 3,
    
    -- Timestamps
    QueuedAt TEXT NOT NULL,
    
    FOREIGN KEY (MediaFileId) REFERENCES MediaFiles(Id),
    FOREIGN KEY (QualityProfileId) REFERENCES QualityProfiles(Id)
);
```

#### FolderConfigurations
```sql
CREATE TABLE FolderConfigurations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Path TEXT NOT NULL UNIQUE,
    Enabled BOOLEAN NOT NULL DEFAULT 1,
    Recursive BOOLEAN NOT NULL DEFAULT 1,
    QualityProfileId INTEGER,
    
    -- Filters
    FileExtensions TEXT, -- Comma-separated: "mp4,mkv,avi,mov"
    ExcludePatterns TEXT, -- Regex patterns to exclude
    
    -- Processing Options
    DeleteSourceAfterSuccess BOOLEAN DEFAULT 0,
    
    -- Metadata
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    
    FOREIGN KEY (QualityProfileId) REFERENCES QualityProfiles(Id)
);
```

#### SystemConfiguration
```sql
CREATE TABLE SystemConfiguration (
    Key TEXT PRIMARY KEY,
    Value TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);
```

#### JobHistory
```sql
CREATE TABLE JobHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MediaFileId INTEGER NOT NULL,
    TranscodingJobId INTEGER NOT NULL,
    
    Action TEXT NOT NULL, -- "Created", "Started", "Completed", "Failed", "Cancelled"
    Message TEXT,
    Metadata TEXT, -- JSON for additional data
    
    CreatedAt TEXT NOT NULL,
    
    FOREIGN KEY (MediaFileId) REFERENCES MediaFiles(Id),
    FOREIGN KEY (TranscodingJobId) REFERENCES TranscodingJobs(Id)
);
```

## API Structure

### REST API Endpoints

#### Quality Profiles
```
GET    /api/v1/qualityprofiles          - List all quality profiles
GET    /api/v1/qualityprofiles/{id}     - Get specific profile
POST   /api/v1/qualityprofiles          - Create new profile
PUT    /api/v1/qualityprofiles/{id}     - Update profile
DELETE /api/v1/qualityprofiles/{id}     - Delete profile
GET    /api/v1/qualityprofiles/test     - Test profile settings
```

#### Media Files
```
GET    /api/v1/media                    - List all media files (with filters)
GET    /api/v1/media/{id}               - Get specific media file
POST   /api/v1/media/scan               - Trigger manual scan
DELETE /api/v1/media/{id}               - Remove media file from tracking
POST   /api/v1/media/{id}/reanalyze     - Re-analyze media file
```

#### Transcoding Jobs
```
GET    /api/v1/jobs                     - List all jobs (with filters)
GET    /api/v1/jobs/{id}                - Get specific job
GET    /api/v1/jobs/queue               - Get current queue status
POST   /api/v1/jobs                     - Create manual job
PUT    /api/v1/jobs/{id}/pause          - Pause job
PUT    /api/v1/jobs/{id}/resume         - Resume job
PUT    /api/v1/jobs/{id}/cancel         - Cancel job
DELETE /api/v1/jobs/{id}                - Delete job record
GET    /api/v1/jobs/{id}/history        - Get job history
```

#### Folders
```
GET    /api/v1/folders                  - List configured folders
GET    /api/v1/folders/{id}             - Get specific folder
POST   /api/v1/folders                  - Add new folder
PUT    /api/v1/folders/{id}             - Update folder configuration
DELETE /api/v1/folders/{id}             - Remove folder
POST   /api/v1/folders/{id}/scan        - Trigger folder scan
```

#### System
```
GET    /api/v1/system/status            - Get system status
GET    /api/v1/system/config            - Get system configuration
PUT    /api/v1/system/config            - Update system configuration
GET    /api/v1/system/health            - Health check endpoint
GET    /api/v1/system/capabilities      - Get hardware capabilities (NVENC, QSV, etc.)
```

#### Activity/Dashboard
```
GET    /api/v1/activity                 - Get recent activity
GET    /api/v1/dashboard/stats          - Get dashboard statistics
GET    /api/v1/dashboard/queue          - Get queue overview
```

## Key Features Implementation

### 1. Hardware Acceleration
- Detect available hardware encoders at startup
- Support NVENC (NVIDIA), QSV (Intel), VAAPI (Linux), VideoToolbox (macOS)
- Fallback to software encoding if hardware unavailable
- Allow per-profile hardware acceleration preference

### 2. Atomic File Replacement
1. Transcode to temporary location (same filesystem)
2. Verify output file integrity
3. Create backup of original (optional)
4. Atomic rename/move operation
5. Delete backup after verification period
6. Rollback on failure

### 3. Task Queue Management
- Priority queue (FIFO with priority)
- Configurable concurrent job limit
- Job state persistence (survive restarts)
- Progress tracking via FFmpeg output parsing
- Estimated time remaining calculation

### 4. Error Handling & Retry Logic
- Configurable retry attempts
- Exponential backoff
- Different error categories (temporary vs permanent)
- Detailed error logging
- Failed job notifications

### 5. File System Monitoring
- Debouncing to avoid multiple triggers
- Handle file locking (wait for file to be fully written)
- Support for network shares
- Recursive folder monitoring
- Pattern-based filtering

## Technology Stack

### Backend (.NET 8)
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0 with SQLite
- **Background Services**: IHostedService / BackgroundService
- **API**: REST with Swagger/OpenAPI
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Queue**: Custom implementation or Hangfire

### Frontend (React)
- **Framework**: React 18+ with TypeScript
- **State Management**: Redux Toolkit or Zustand
- **UI Library**: Material-UI or similar (matching *arr ecosystem)
- **HTTP Client**: Axios
- **Routing**: React Router
- **Build Tool**: Vite or Create React App

### External Tools
- **FFmpeg**: For transcoding
- **FFprobe**: For media analysis

## Project Structure

```
Resizarr/
├── src/
│   ├── Resizarr.Api/                    # Web API Project
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── Resizarr.Application/            # Application Layer
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   │
│   ├── Resizarr.Core/                   # Domain Layer
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── Events/
│   │   ├── Interfaces/
│   │   └── ValueObjects/
│   │
│   ├── Resizarr.Infrastructure/         # Infrastructure Layer
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/
│   │   │   └── Migrations/
│   │   ├── Services/
│   │   │   ├── FFmpegService.cs
│   │   │   ├── FFprobeService.cs
│   │   │   ├── FolderMonitorService.cs
│   │   │   └── TranscodingQueueService.cs
│   │   └── Repositories/
│   │
│   └── Resizarr.Web/                    # React Frontend
│       ├── public/
│       ├── src/
│       │   ├── components/
│       │   ├── pages/
│       │   ├── services/
│       │   ├── store/
│       │   └── App.tsx
│       └── package.json
│
├── tests/
│   ├── Resizarr.Api.Tests/
│   ├── Resizarr.Application.Tests/
│   ├── Resizarr.Core.Tests/
│   └── Resizarr.Infrastructure.Tests/
│
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
│
├── Resizarr.sln
└── README.md
```

## Development Phases

### Phase 1: Foundation (Week 1-2)
- [ ] Set up solution structure
- [ ] Create domain entities
- [ ] Set up EF Core with SQLite
- [ ] Implement basic API endpoints
- [ ] Set up React frontend structure

### Phase 2: Core Services (Week 3-4)
- [ ] Implement FFprobe media analysis
- [ ] Implement folder monitoring
- [ ] Create quality profile management
- [ ] Build task queue system

### Phase 3: Transcoding (Week 5-6)
- [ ] Implement FFmpeg service
- [ ] Add hardware acceleration detection
- [ ] Implement atomic file replacement
- [ ] Add progress tracking

### Phase 4: UI & Polish (Week 7-8)
- [ ] Build React UI components
- [ ] Implement real-time updates (SignalR)
- [ ] Add comprehensive error handling
- [ ] Performance optimization

### Phase 5: Testing & Deployment (Week 9-10)
- [ ] Write unit and integration tests
- [ ] Create Docker configuration
- [ ] Documentation
- [ ] Release v1.0

## Configuration Examples

### Default Quality Profile (1080p H.264)
```json
{
  "name": "HD-1080p",
  "targetResolution": "1920x1080",
  "targetCodec": "h264",
  "targetBitrate": 5000,
  "audioCodec": "aac",
  "audioBitrate": 192,
  "hardwareAcceleration": "auto",
  "minSourceWidth": 1920
}
```

### System Configuration
```json
{
  "maxConcurrentJobs": 2,
  "tempDirectory": "/tmp/resizarr",
  "ffmpegPath": "/usr/bin/ffmpeg",
  "ffprobePath": "/usr/bin/ffprobe",
  "enableHardwareAcceleration": true,
  "deleteSourceOnSuccess": false,
  "backupOriginalFiles": false
}
```

## Security Considerations
- API authentication (JWT or API keys)
- File system access validation
- Input sanitization for FFmpeg commands
- Rate limiting on API endpoints
- CORS configuration for frontend

## Performance Optimizations
- Lazy loading for large file lists
- Pagination for API responses
- Database indexing on frequently queried fields
- Caching for system capabilities and configuration
- Async/await throughout for non-blocking operations

## Monitoring & Logging
- Structured logging with Serilog
- Log levels: Debug, Information, Warning, Error, Fatal
- Performance metrics (jobs/hour, average processing time)
- Disk space monitoring
- FFmpeg process monitoring

## Future Enhancements
- Multi-language subtitle handling
- Custom FFmpeg filter chains
- Scheduling (process during off-hours)
- Remote folder support (SMB/NFS)
- Email/webhook notifications
- Advanced analytics and reporting
- Plugin system for custom processors
- Distributed processing across multiple nodes
