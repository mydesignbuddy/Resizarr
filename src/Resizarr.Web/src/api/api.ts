import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5200/api/v1';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export interface QualityProfile {
  id: number;
  name: string;
  description?: string;
  enabled: boolean;
  targetResolution: string;
  targetCodec: string;
  targetBitrate?: number;
  minBitrate?: number;
  maxBitrate?: number;
  targetFrameRate?: number;
  audioCodec: string;
  audioBitrate?: number;
  audioChannels?: number;
  hardwareAcceleration: string;
  minSourceWidth?: number;
  minSourceHeight?: number;
  sourceCodecs?: string;
  createdAt: string;
  updatedAt: string;
}

export interface MediaFile {
  id: number;
  filePath: string;
  fileName: string;
  folderPath: string;
  fileSize: number;
  videoCodec?: string;
  videoWidth?: number;
  videoHeight?: number;
  videoBitrate?: number;
  videoFrameRate?: number;
  duration?: number;
  audioCodec?: string;
  audioBitrate?: number;
  audioChannels?: number;
  status: string;
  qualityProfileId?: number;
  qualityProfileName?: string;
  discoveredAt: string;
  lastScannedAt?: string;
  processedAt?: string;
}

export interface TranscodingJob {
  id: number;
  mediaFileId: number;
  mediaFileName: string;
  qualityProfileId: number;
  qualityProfileName: string;
  sourcePath: string;
  outputPath: string;
  status: string;
  priority: number;
  progress: number;
  startedAt?: string;
  completedAt?: string;
  processingTime?: number;
  outputFileSize?: number;
  errorMessage?: string;
  retryCount: number;
  queuedAt: string;
}

export interface DashboardStats {
  totalMediaFiles: number;
  pendingFiles: number;
  processingFiles: number;
  completedFiles: number;
  failedFiles: number;
  activeJobs: number;
  queuedJobs: number;
  totalBytesProcessed: number;
  totalBytesSaved: number;
}

export const qualityProfilesApi = {
  getAll: () => api.get<QualityProfile[]>('/qualityprofiles'),
  getById: (id: number) => api.get<QualityProfile>(`/qualityprofiles/${id}`),
  create: (profile: Partial<QualityProfile>) => api.post<QualityProfile>('/qualityprofiles', profile),
  update: (id: number, profile: Partial<QualityProfile>) => api.put(`/qualityprofiles/${id}`, profile),
  delete: (id: number) => api.delete(`/qualityprofiles/${id}`),
};

export const mediaApi = {
  getAll: (status?: string, qualityProfileId?: number) => {
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (qualityProfileId) params.append('qualityProfileId', qualityProfileId.toString());
    return api.get<MediaFile[]>(`/media?${params.toString()}`);
  },
  getById: (id: number) => api.get<MediaFile>(`/media/${id}`),
  delete: (id: number) => api.delete(`/media/${id}`),
  triggerScan: () => api.post('/media/scan'),
};

export const jobsApi = {
  getAll: (status?: string) => {
    const params = status ? `?status=${status}` : '';
    return api.get<TranscodingJob[]>(`/jobs${params}`);
  },
  getById: (id: number) => api.get<TranscodingJob>(`/jobs/${id}`),
  getQueue: () => api.get('/jobs/queue'),
  delete: (id: number) => api.delete(`/jobs/${id}`),
};

export const dashboardApi = {
  getStats: () => api.get<DashboardStats>('/dashboard/stats'),
  getQueue: () => api.get<TranscodingJob[]>('/dashboard/queue'),
};

export const systemApi = {
  getStatus: () => api.get('/system/status'),
  getHealth: () => api.get('/system/health'),
  getCapabilities: () => api.get('/system/capabilities'),
};

export default api;
