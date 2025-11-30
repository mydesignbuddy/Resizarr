namespace Resizarr.Core.Interfaces;

public class MediaInfo
{
    public string VideoCodec { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public int VideoBitrate { get; set; }
    public double FrameRate { get; set; }
    public double Duration { get; set; }
    public string AudioCodec { get; set; } = string.Empty;
    public int AudioBitrate { get; set; }
    public int AudioChannels { get; set; }
    public int AudioSampleRate { get; set; }
}

public interface IMediaAnalysisService
{
    Task<MediaInfo?> AnalyzeMediaFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> IsValidMediaFileAsync(string filePath, CancellationToken cancellationToken = default);
}
