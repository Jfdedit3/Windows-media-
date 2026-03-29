public sealed partial class MainForm : Form
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff" };
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".webm", ".m4v" };
    private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".wma" };

    private readonly string _settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MediaGalleryNovaWindows");
    private readonly string _settingsPath;

    private AppSettings _settings;
    private List<MediaItem> _allItems = new();
    private MediaType _currentTab = MediaType.All;

    private readonly ToolStrip _topStrip = new();
    private readonly ToolStripButton _chooseFolderButton = new("Choose Folder");
    private readonly ToolStripButton _refreshButton = new("Refresh");
    private readonly ToolStripButton _settingsButton = new("Settings");
    private readonly ToolStripButton _allButton = new("All");
    private readonly ToolStripButton _imagesButton = new("Images");
    private readonly ToolStripButton _videosButton = new("Videos");
    private readonly ToolStripButton _audioButton = new("Audio");
    private readonly ToolStripTextBox _searchBox = new() { AutoSize = false, Width = 240 };
    private readonly ToolStripLabel _pathLabel = new();

    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    private readonly ListView _listView = new();
    private readonly ImageList _imageList = new();
    private readonly ContextMenuStrip _contextMenu = new();

    public MainForm()
    {
        _settingsPath = Path.Combine(_settingsDir, "settings.json");
        _settings = LoadSettings();

        Text = "MediaGalleryNova Windows";
        Width = 1280;
        Height = 820;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 680);

        _topStrip.GripStyle = ToolStripGripStyle.Hidden;
        _topStrip.Padding = new Padding(8);
        _topStrip.Items.AddRange(new ToolStripItem[]
        {
            _chooseFolderButton,
            _refreshButton,
            _settingsButton,
            new ToolStripSeparator(),
            _allButton,
            _imagesButton,
            _videosButton,
            _audioButton,
            new ToolStripSeparator(),
            new ToolStripLabel("Search:"),
            _searchBox,
            new ToolStripSeparator(),
            _pathLabel
        });

        _chooseFolderButton.Click += (_, _) => ChooseFolder();
        _refreshButton.Click += (_, _) => LoadMedia();
        _settingsButton.Click += (_, _) => OpenSettings();
        _allButton.Click += (_, _) => SetTab(MediaType.All);
        _imagesButton.Click += (_, _) => SetTab(MediaType.Image);
        _videosButton.Click += (_, _) => SetTab(MediaType.Video);
        _audioButton.Click += (_, _) => SetTab(MediaType.Audio);
        _searchBox.TextChanged += (_, _) => ApplyFilters();

        _statusStrip.Items.Add(_statusLabel);

        _imageList.ColorDepth = ColorDepth.Depth32Bit;
        _imageList.ImageSize = new Size(_settings.ThumbnailSize, _settings.ThumbnailSize);

        _listView.Dock = DockStyle.Fill;
        _listView.View = View.LargeIcon;
        _listView.MultiSelect = true;
        _listView.HideSelection = false;
        _listView.FullRowSelect = false;
        _listView.LargeImageList = _imageList;
        _listView.DoubleClick += (_, _) => OpenSelected();
        _listView.KeyDown += ListView_KeyDown;

        _contextMenu.Items.Add("Open", null, (_, _) => OpenSelected());
        _contextMenu.Items.Add("Copy Path", null, (_, _) => CopyPath());
        _contextMenu.Items.Add("Set as Wallpaper", null, (_, _) => SetWallpaper());
        _contextMenu.Items.Add("Delete", null, (_, _) => DeleteSelected());
        _listView.ContextMenuStrip = _contextMenu;

        Controls.Add(_listView);
        Controls.Add(_topStrip);
        Controls.Add(_statusStrip);

        ApplyTheme();
        SetTab(MediaType.All);
        LoadMedia();
    }

    private void ListView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            DeleteSelected();
            e.Handled = true;
        }
        else if (e.Control && e.KeyCode == Keys.A)
        {
            foreach (ListViewItem item in _listView.Items)
                item.Selected = true;
            UpdateStatus();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Enter)
        {
            OpenSelected();
            e.Handled = true;
        }
    }

    private void SetTab(MediaType tab)
    {
        _currentTab = tab;
        _allButton.Checked = tab == MediaType.All;
        _imagesButton.Checked = tab == MediaType.Image;
        _videosButton.Checked = tab == MediaType.Video;
        _audioButton.Checked = tab == MediaType.Audio;
        ApplyFilters();
    }
}
