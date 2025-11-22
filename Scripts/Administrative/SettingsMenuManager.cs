using Godot;
using System;

public partial class SettingsMenuManager : Node2D
{
    private static SettingsMenuManager _instance;
    public static SettingsMenuManager Instance
    {
        get;
        private set;
    }

    private SettingsMenuManager() { }

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

    public AspectRatio[] availableAspectRatios { get; private set; } =
    [
        new AspectRatio(16, 9),
        new AspectRatio(8, 5),
        new AspectRatio(4, 3),
        new AspectRatio(5, 4),
        new AspectRatio(21, 9)
    ];

    public Resolution[] availableResolutions { get; private set; } =
    [
        // Common resolutions 16:9
        new Resolution(640,  360),
        new Resolution(854,  480),
        new Resolution(960,  540),
        new Resolution(1024, 576),
        new Resolution(1280, 720),
        new Resolution(1366, 768),
        new Resolution(1600, 900),
        new Resolution(1920, 1080),
        new Resolution(2560, 1440),
        new Resolution(3840, 2160),

        // Common resolutions 8:5
        new Resolution(800,  500),
        new Resolution(1280, 800),
        new Resolution(1440, 900),
        new Resolution(1680, 1050),
        new Resolution(1920, 1200),
        new Resolution(2560, 1600),
        new Resolution(3840, 2400),

        // Common resolutions 4:3
        new Resolution(800,  600),
        new Resolution(1024, 768),
        new Resolution(1152, 864),
        new Resolution(1280, 960),
        new Resolution(1400, 1050),
        new Resolution(1600, 1200),
        new Resolution(2048, 1536),
        new Resolution(2560, 1920),
        new Resolution(3200, 2400),
        new Resolution(4096, 3072),

        // Common resolutions 5:4
        new Resolution(1280, 1024),
        new Resolution(2560, 2048),
        new Resolution(3840, 3072),

        // Common resolutions 21:9
        new Resolution(2560, 1080),
        new Resolution(3440, 1440)
    ];

    [Signal]
    public delegate void AspectRatioChangedEventHandler();

    public void SetMasterVolume(float volume)
    {
        Settings.MasterVolume = volume / 100.0f;
    }

    public void SetMusicVolume(float volume)
    {
        Settings.MusicVolume = volume / 100.0f;
    }

    public void SetSFXVolume(float volume)
    {
        Settings.SFXVolume = volume / 100.0f;
    }


    public void SetFullscreen(int isFullscreen)
    {
        GD.Print($"SetFullscreen({isFullscreen})");
        SetFullscreen(isFullscreen == 1);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        GD.Print($"SetFullscreen({isFullscreen})");
        Settings.IsFullscreen = isFullscreen;
    }

    public void SetAspectRatio(int index)
    {
        if (index < 0 || index >= availableAspectRatios.Length) return;

        AspectRatio ar = availableAspectRatios[index];
        Settings.TargetAspectRatio = ar;
        EmitSignal(SignalName.AspectRatioChanged);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= availableResolutions.Length) return;

        Resolution res = availableResolutions[index];
        Settings.SetResolution(res.width, res.height);
    }
}
