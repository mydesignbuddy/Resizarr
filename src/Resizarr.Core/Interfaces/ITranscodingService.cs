using Resizarr.Core.Entities;
using Resizarr.Core.Enums;

namespace Resizarr.Core.Interfaces;

public interface ITranscodingService
{
    Task<bool> TranscodeAsync(TranscodingJob job, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    string BuildFFmpegCommand(TranscodingJob job, QualityProfile profile);
    Task<bool> ValidateOutputFileAsync(string filePath, CancellationToken cancellationToken = default);
}
