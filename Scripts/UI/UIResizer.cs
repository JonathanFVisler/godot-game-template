using Godot;
using System;

public partial class UIResizer : Control
{
    public override void _Ready()
    {
        UpdateSize();
        Settings.OnUISettingsChanged += UpdateSize;
    }

    public override void _ExitTree()
    {
        Settings.OnUISettingsChanged -= UpdateSize;
    }

    private void UpdateSize()
    {
        int width = Settings.ScreenWidth;
        int height = Settings.ScreenHeight;
        Vector2 size = new Vector2(width, height);

        Vector2 baseSize = new Vector2(1280, 720);

        GetTree().Root.ContentScaleFactor = (size.X / baseSize.X) * Settings.UIScale;
    }
}
