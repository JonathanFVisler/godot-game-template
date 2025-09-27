using System;
using Godot;

public partial class AudioMaster
{
    [Export] private static float defaultMusicModifier = 4f;
    [Export] private static float defaultSFXModifier = 4f;
    public static float GetMusicVolume(float modifier = 0f)
    {
        switch (modifier)
        {
            case > 0f:
                return (float)Mathf.LinearToDb(Settings.MusicVolume * Settings.MasterVolume * modifier);
            default:
                return (float)Mathf.LinearToDb(Settings.MusicVolume * Settings.MasterVolume * defaultMusicModifier);
        }
    }

    public static float GetSFXVolume(float modifier = 0f)
    {
        switch (modifier)
        {
            case > 0f:
                return (float)Mathf.LinearToDb(Settings.SFXVolume * Settings.MasterVolume * modifier);
            default:
                return (float)Mathf.LinearToDb(Settings.SFXVolume * Settings.MasterVolume * defaultSFXModifier);
        }
    }
}
