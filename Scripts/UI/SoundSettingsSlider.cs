using Godot;
using System;

public partial class SoundSettingsSlider : HSlider
{
    public enum SoundType
    {
        Master,
        Music,
        SFX
    }

    [Export]
    public SoundType soundType = SoundType.Master;

    public override void _Ready()
    {
        CallDeferred(nameof(Initialize));
    }

    private void Initialize()
    {
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        switch (soundType)
        {
            case SoundType.Master:
                Value = Settings.MasterVolume * 100.0f;
                break;
            case SoundType.Music:
                Value = Settings.MusicVolume * 100.0f;
                break;
            case SoundType.SFX:
                Value = Settings.SFXVolume * 100.0f;
                break;
        }
    }
}
