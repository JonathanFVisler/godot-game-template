using Godot;
using System;

public partial class GameManager : Node2D
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get;
        private set;
    }

    private GameManager() { }

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
    }

    // -----------------------------------------------------------------------

    public override void _Ready()
    {
        Settings.LoadSettings();
        SetFullscreen();
        SetResolution();
    }

    public void SetFullscreen()
    {
        bool fullscreen = Settings.IsFullscreen;
        GD.Print($"Setting fullscreen: {fullscreen}");

        if (fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }

    public void SetResolution()
    {
        int width = Settings.ScreenWidth;
        int height = Settings.ScreenHeight;
        DisplayServer.WindowSetSize(new Vector2I(width, height));
    }
}
