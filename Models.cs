public enum MediaType
{
    All,
    Image,
    Video,
    Audio
}

public sealed class AppSettings
{
    public bool DarkMode { get; set; } = true;
    public bool ShowFileNames { get; set; } = true;
    public string LastFolder { get; set; } = "";
    public int ThumbnailSize { get; set; } = 160;
}

public sealed class MediaItem
{
    public string FilePath { get; init; } = "";
    public string Name => Path.GetFileName(FilePath);
    public MediaType Type { get; init; }
    public DateTime DateAdded { get; init; }
}
