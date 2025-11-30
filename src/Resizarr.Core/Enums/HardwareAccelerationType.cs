namespace Resizarr.Core.Enums;

public enum HardwareAccelerationType
{
    None,
    Auto,
    NVENC,  // NVIDIA
    QSV,    // Intel Quick Sync
    VAAPI,  // Video Acceleration API (Linux)
    VideoToolbox  // macOS
}
