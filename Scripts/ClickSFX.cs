using Godot;
using System;

public partial class ClickSFX : AudioStreamPlayer2D
{
    float modifier = 4.0f;

    public override void _Ready()
    {
        Settings.OnAudioSettingsChanged += AdjustVolume;
        AdjustVolume();
    }

    public override void _ExitTree()
    {
        Settings.OnAudioSettingsChanged -= AdjustVolume;
    }

    private void AdjustVolume()
    {
        VolumeDb = (float)Mathf.LinearToDb(Settings.SFXVolume * Settings.MasterVolume * modifier);
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("action1"))
        {
            Play();
        }
    }
}
