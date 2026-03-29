public sealed partial class MainForm
{
    private List<MediaItem> ScanMedia(string? rootFolder)
    {
        var folders = ResolveFolders(rootFolder);
        var items = new List<MediaItem>();

        foreach (var folder in folders.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!Directory.Exists(folder))
                continue;

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories);
            }
            catch
            {
                continue;
            }

            foreach (var path in files)
            {
                var ext = Path.GetExtension(path);
                var type = ResolveType(ext);
                if (type == null)
                    continue;

                DateTime date;
                try { date = File.GetCreationTime(path); }
                catch { date = DateTime.MinValue; }

                items.Add(new MediaItem
                {
                    FilePath = path,
                    Type = type.Value,
                    DateAdded = date
                });
            }
        }

        return items;
    }

    private static IEnumerable<string> ResolveFolders(string? rootFolder)
    {
        if (!string.IsNullOrWhiteSpace(rootFolder) && Directory.Exists(rootFolder))
            return new[] { rootFolder };

        return new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
        };
    }

    private static MediaType? ResolveType(string extension)
    {
        if (ImageExtensions.Contains(extension)) return MediaType.Image;
        if (VideoExtensions.Contains(extension)) return MediaType.Video;
        if (AudioExtensions.Contains(extension)) return MediaType.Audio;
        return null;
    }

    private Image CreateThumbnail(MediaItem item)
    {
        var size = _settings.ThumbnailSize;
        var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);
        g.Clear(_settings.DarkMode ? Color.FromArgb(30, 30, 30) : Color.FromArgb(235, 235, 235));

        try
        {
            if (item.Type == MediaType.Image)
            {
                using var source = Image.FromFile(item.FilePath);
                var rect = FitRect(source.Width, source.Height, size, size);
                g.DrawImage(source, rect);
            }
            else
            {
                using var font = new Font("Segoe UI", 18, FontStyle.Bold);
                using var brush = new SolidBrush(_settings.DarkMode ? Color.White : Color.Black);
                var text = item.Type == MediaType.Video ? "VIDEO" : "AUDIO";
                var measured = g.MeasureString(text, font);
                g.DrawString(text, font, brush, (size - measured.Width) / 2, (size - measured.Height) / 2);
            }
        }
        catch
        {
            using var font = new Font("Segoe UI", 12, FontStyle.Bold);
            using var brush = new SolidBrush(Color.IndianRed);
            g.DrawString("ERROR", font, brush, 10, 10);
        }

        return bitmap;
    }

    private static Rectangle FitRect(int sourceWidth, int sourceHeight, int maxWidth, int maxHeight)
    {
        var ratio = Math.Min((double)maxWidth / sourceWidth, (double)maxHeight / sourceHeight);
        var width = (int)(sourceWidth * ratio);
        var height = (int)(sourceHeight * ratio);
        var x = (maxWidth - width) / 2;
        var y = (maxHeight - height) / 2;
        return new Rectangle(x, y, width, height);
    }
}
