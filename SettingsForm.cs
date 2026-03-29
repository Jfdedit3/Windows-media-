public sealed class SettingsForm : Form
{
    private readonly CheckBox _darkMode = new() { Text = "Dark mode", AutoSize = true };
    private readonly CheckBox _showNames = new() { Text = "Show file names", AutoSize = true };
    private readonly NumericUpDown _thumbSize = new() { Minimum = 96, Maximum = 256, Increment = 16, Width = 100 };
    private readonly Button _ok = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 100 };
    private readonly Button _cancel = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 100 };

    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings settings)
    {
        Settings = new AppSettings
        {
            DarkMode = settings.DarkMode,
            ShowFileNames = settings.ShowFileNames,
            LastFolder = settings.LastFolder,
            ThumbnailSize = settings.ThumbnailSize
        };

        Text = "Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(320, 180);

        _darkMode.Checked = Settings.DarkMode;
        _showNames.Checked = Settings.ShowFileNames;
        _thumbSize.Value = Settings.ThumbnailSize;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 4 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layout.Controls.Add(_darkMode, 0, 0);
        layout.SetColumnSpan(_darkMode, 2);
        layout.Controls.Add(_showNames, 0, 1);
        layout.SetColumnSpan(_showNames, 2);
        layout.Controls.Add(new Label { Text = "Thumbnail size", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        layout.Controls.Add(_thumbSize, 1, 2);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.Add(_cancel);
        buttons.Controls.Add(_ok);
        layout.Controls.Add(buttons, 0, 3);
        layout.SetColumnSpan(buttons, 2);
        Controls.Add(layout);

        AcceptButton = _ok;
        CancelButton = _cancel;

        FormClosing += (_, e) =>
        {
            if (DialogResult == DialogResult.OK)
            {
                Settings.DarkMode = _darkMode.Checked;
                Settings.ShowFileNames = _showNames.Checked;
                Settings.ThumbnailSize = (int)_thumbSize.Value;
            }
        };
    }
}
