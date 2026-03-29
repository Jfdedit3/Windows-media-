public sealed partial class MainForm
{
    private MediaItem? GetFirstSelected()
    {
        return _listView.SelectedItems.Count == 0 ? null : _listView.SelectedItems[0].Tag as MediaItem;
    }

    private List<MediaItem> GetSelectedItems()
    {
        return _listView.SelectedItems.Cast<ListViewItem>().Select(x => (MediaItem)x.Tag).ToList();
    }

    private void OpenSelected()
    {
        var item = GetFirstSelected();
        if (item == null)
            return;

        if (item.Type == MediaType.Image)
        {
            using var viewer = new ImageViewerForm(item);
            viewer.ShowDialog(this);
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = item.FilePath,
            UseShellExecute = true
        });
    }

    private void CopyPath()
    {
        var item = GetFirstSelected();
        if (item == null)
            return;

        Clipboard.SetText(item.FilePath);
        _statusLabel.Text = "Path copied.";
    }

    private void SetWallpaper()
    {
        var item = GetFirstSelected();
        if (item == null || item.Type != MediaType.Image)
            return;

        try
        {
            WallpaperSetter.Set(item.FilePath);
            MessageBox.Show(this, "Wallpaper updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Wallpaper", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteSelected()
    {
        var items = GetSelectedItems();
        if (items.Count == 0)
            return;

        var confirm = MessageBox.Show(this, $"Delete {items.Count} selected file(s) to the Recycle Bin?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes)
            return;

        foreach (var item in items)
        {
            try
            {
                FileSystem.DeleteFile(item.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            catch { }
        }

        LoadMedia();
    }

    private AppSettings LoadSettings()
    {
        try
        {
            Directory.CreateDirectory(_settingsDir);
            if (!File.Exists(_settingsPath))
                return new AppSettings();
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    private void SaveSettings()
    {
        Directory.CreateDirectory(_settingsDir);
        File.WriteAllText(_settingsPath, JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true }));
    }

    private void ApplyTheme()
    {
        var back = _settings.DarkMode ? Color.FromArgb(18, 18, 18) : Color.FromArgb(245, 245, 245);
        var fore = _settings.DarkMode ? Color.White : Color.Black;
        BackColor = back;
        ForeColor = fore;
        _topStrip.BackColor = back;
        _topStrip.ForeColor = fore;
        _statusStrip.BackColor = back;
        _statusStrip.ForeColor = fore;
        _listView.BackColor = _settings.DarkMode ? Color.FromArgb(24, 24, 24) : Color.White;
        _listView.ForeColor = fore;
    }
}
