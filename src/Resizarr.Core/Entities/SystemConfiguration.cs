namespace Resizarr.Core.Entities;

public class SystemConfiguration
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    public SystemConfiguration()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
