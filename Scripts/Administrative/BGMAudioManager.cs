using Godot;
using System;

public partial class BGMAudioManager : Node2D
{
    public static BGMAudioManager Instance
    {
        get;
        private set;
    }

    private BGMAudioManager() { }

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
        Settings.OnAudioSettingsChanged -= AdjustVolume;
    }

    // -----------------------------------------------------------------------

    [Export] AudioStreamPlayer2D[] musicPlayer;
    float modifier = 4.0f;
    public override void _Ready()
    {
        AdjustVolume();
        Settings.OnAudioSettingsChanged += AdjustVolume;
    }

    public void AdjustVolume()
    {
        foreach (var player in musicPlayer)
        {
            player.VolumeDb = (float)Mathf.LinearToDb(Settings.MusicVolume * Settings.MasterVolume * modifier);
        }
    }

    public void PlayMusic(AudioStream music)
    {
        if (musicPlayer.Length == 0) return;

        int index = (int)(GD.Randi() % musicPlayer.Length);
        var player = musicPlayer[index];

        if (player.Stream == music && player.Playing) return;

        foreach (var p in musicPlayer)
        {
            p.Stop();
        }

        player.Stream = music;
        player.VolumeDb = (float)Mathf.LinearToDb(Settings.MusicVolume * Settings.MasterVolume);
        player.Play();
    }
}
