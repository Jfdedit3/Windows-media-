public sealed partial class MainForm
{
    private void ChooseFolder()
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "Choose a folder to scan";
        dialog.SelectedPath = Directory.Exists(_settings.LastFolder) ? _settings.LastFolder : Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _settings.LastFolder = dialog.SelectedPath;
            SaveSettings();
            LoadMedia();
        }
    }

    private void OpenSettings()
    {
        using var form = new SettingsForm(_settings);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _settings = form.Settings;
            SaveSettings();
            _imageList.ImageSize = new Size(_settings.ThumbnailSize, _settings.ThumbnailSize);
            ApplyTheme();
            ApplyFilters();
        }
    }

    private void LoadMedia()
    {
        try
        {
            UseWaitCursor = true;
            _statusLabel.Text = "Loading media...";
            _allItems = ScanMedia(_settings.LastFolder).OrderByDescending(x => x.DateAdded).ToList();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void ApplyFilters()
    {
        _listView.BeginUpdate();
        _listView.Items.Clear();
        _imageList.Images.Clear();

        var query = _searchBox.Text.Trim();
        var filtered = _allItems.Where(item =>
            (_currentTab == MediaType.All || item.Type == _currentTab) &&
            (string.IsNullOrWhiteSpace(query) || item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        for (var i = 0; i < filtered.Count; i++)
        {
            var item = filtered[i];
            _imageList.Images.Add(CreateThumbnail(item));
            var text = _settings.ShowFileNames ? item.Name : string.Empty;
            var listItem = new ListViewItem(text, i) { Tag = item, ToolTipText = item.FilePath };
            _listView.Items.Add(listItem);
        }

        _pathLabel.Text = string.IsNullOrWhiteSpace(_settings.LastFolder) ? "Libraries: Pictures / Videos / Music" : _settings.LastFolder;
        _listView.EndUpdate();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        _statusLabel.Text = $"Items: {_listView.Items.Count} | Selected: {_listView.SelectedItems.Count}";
    }
}
