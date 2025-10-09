using System;
using Godot;

public partial class Settings
{
    private static string savePath = "user://settings.cfg";
    private static bool isFullscreen { get; set; } = false;
    private static AspectRatio aspectRatio { get; set; } = new AspectRatio(16, 9);
    private static int screenWidth { get; set; } = 1280;
    private static int screenHeight { get; set; } = 720;
    private static float masterVolume { get; set; } = 0.5f;
    private static float musicVolume { get; set; } = 0.5f;
    private static float sfxVolume { get; set; } = 0.5f;
    private static float uiScale { get; set; } = 1.0f;

    public static event Action OnUISettingsChanged;
    public static event Action OnAudioSettingsChanged;
    private static void NotifyUIChanged() => OnUISettingsChanged?.Invoke();
    private static void NotifyAudioChanged() => OnAudioSettingsChanged?.Invoke();

    public static bool IsFullscreen
    {
        get => isFullscreen;
        set
        {
            if (isFullscreen == value) { return; }
            isFullscreen = value;
            GameManager.Instance?.SetFullscreen();
            SaveSettings();
        }
    }
    public static void SetResolution(int width, int height)
    {
        bool changed = (screenWidth != width) || (screenHeight != height);
        if (!changed) return;

        screenWidth  = width;
        screenHeight = height;

        GameManager.Instance?.SetResolution();
        NotifyUIChanged();
        SaveSettings();
    }

    public static AspectRatio TargetAspectRatio
    {
        get => aspectRatio;
        set
        {
            if (aspectRatio.width == value.width && aspectRatio.height == value.height) { return; }
            aspectRatio = value;
            SaveSettings();
        }
    }

    public static int ScreenWidth
    {
        get => screenWidth;
        set => SetResolution(value, screenHeight);
    }

    public static int ScreenHeight
    {
        get => screenHeight;
        set => SetResolution(screenWidth, value);
    }

    public static float MasterVolume
    {
        get => masterVolume;
        set
        {
            if (masterVolume == value) { return; }
            masterVolume = value;
            NotifyAudioChanged();
            SaveSettings();
        }
    }
    public static float MusicVolume
    {
        get => musicVolume;
        set
        {
            if (musicVolume == value) { return; }
            musicVolume = value;
            NotifyAudioChanged();
            SaveSettings();
        }
    }
    public static float SFXVolume
    {
        get => sfxVolume;
        set
        {
            if (sfxVolume == value) { return; }
            sfxVolume = value;
            NotifyAudioChanged();
            SaveSettings();
        }
    }
    public static float UIScale
    {
        get => uiScale;
        set
        {
            if (uiScale == value) { return; }
            if (value < 0.0001f) return;
            uiScale = value;
            NotifyUIChanged();
            SaveSettings();
        }
    }

    public static void SaveSettings()
    {
        ConfigFile cfg = new ConfigFile();

        cfg.SetValue("Display", "IsFullscreen", isFullscreen);
        cfg.SetValue("Display", "AspectRatioWidth", aspectRatio.width);
        cfg.SetValue("Display", "AspectRatioHeight", aspectRatio.height);
        cfg.SetValue("Display", "ScreenWidth", screenWidth);
        cfg.SetValue("Display", "ScreenHeight", screenHeight);
        cfg.SetValue("Audio", "MasterVolume", masterVolume);
        cfg.SetValue("Audio", "MusicVolume", musicVolume);
        cfg.SetValue("Audio", "SFXVolume", sfxVolume);
        cfg.SetValue("UI", "UIScale", uiScale);

        Error err = cfg.Save(savePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to save settings: {err.ToString()}");
        }
    }

    public static void LoadSettings()
    {
        ConfigFile cfg = new ConfigFile();
        Error err = cfg.Load(savePath);
        if (err != Error.Ok)
        {
            // First run or corrupt file: save defaults and return.
            SaveSettings();
            return;
        }

        // ConfigFile.GetValue returns a Variant; use As<T>() to get typed values.
        isFullscreen = cfg.GetValue("Display", "IsFullscreen", isFullscreen).As<bool>();
        int arWidth  = cfg.GetValue("Display", "AspectRatioWidth", aspectRatio.width).As<int>();
        int arHeight = cfg.GetValue("Display", "AspectRatioHeight", aspectRatio.height).As<int>();
        aspectRatio = new AspectRatio(arWidth, arHeight);
        screenWidth = (int)cfg.GetValue("Display", "ScreenWidth", screenWidth).As<long>();
        screenHeight = (int)cfg.GetValue("Display", "ScreenHeight", screenHeight).As<long>();

        masterVolume = cfg.GetValue("Audio", "MasterVolume", masterVolume).As<float>();
        musicVolume = cfg.GetValue("Audio", "MusicVolume", musicVolume).As<float>();
        sfxVolume = cfg.GetValue("Audio", "SFXVolume", sfxVolume).As<float>();

        uiScale = cfg.GetValue("UI", "UIScale", uiScale).As<float>();
    }
}
