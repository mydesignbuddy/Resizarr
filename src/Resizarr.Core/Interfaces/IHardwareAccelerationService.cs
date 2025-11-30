using Resizarr.Core.Enums;

namespace Resizarr.Core.Interfaces;

public interface IHardwareAccelerationService
{
    Task<HardwareAccelerationType> DetectAvailableAccelerationAsync(CancellationToken cancellationToken = default);
    bool IsAccelerationSupported(HardwareAccelerationType type);
    string GetFFmpegAccelerationArgs(HardwareAccelerationType type, string codec);
}
