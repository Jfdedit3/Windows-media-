public sealed class ImageViewerForm : Form
{
    private readonly MediaItem _item;

    public ImageViewerForm(MediaItem item)
    {
        _item = item;
        Text = item.Name;
        Width = 1000;
        Height = 700;
        StartPosition = FormStartPosition.CenterParent;

        var tool = new ToolStrip();
        var copyPath = new ToolStripButton("Copy Path");
        var wallpaper = new ToolStripButton("Set as Wallpaper");
        copyPath.Click += (_, _) => Clipboard.SetText(_item.FilePath);
        wallpaper.Click += (_, _) => WallpaperSetter.Set(_item.FilePath);
        tool.Items.Add(copyPath);
        tool.Items.Add(wallpaper);

        var picture = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            ImageLocation = _item.FilePath
        };

        Controls.Add(picture);
        Controls.Add(tool);
    }
}
