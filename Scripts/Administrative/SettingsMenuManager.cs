using Godot;
using System;

public partial class SettingsMenuManager : Node2D
{
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

    public Resolution[] availableResolutions { get; private set; } =
    [
        new Resolution(640,  360),
        new Resolution(854,  480),
        new Resolution(960,  540),
        new Resolution(1024, 576),
        new Resolution(1280, 720),
        new Resolution(1366, 768),
        new Resolution(1600, 900),
        new Resolution(1920, 1080),
        new Resolution(2560, 1440),
        new Resolution(3840, 2160)
    ];

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

    public void SetResolution(int index)
    {
        if (index < 0 || index >= availableResolutions.Length) return;

        Resolution res = availableResolutions[index];
        Settings.SetResolution(res.width, res.height);
    }
}
