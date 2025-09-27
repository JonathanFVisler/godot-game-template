using Godot;
using System;
using System.Linq;

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
        // Settings.OnAudioSettingsChanged -= AdjustVolume;
    }

    // -----------------------------------------------------------------------

    [Export] private AudioStreamer[] musicPlayers;
    private float defaultModifier = 1f;

    public override void _Ready()
    {
        foreach (var player in musicPlayers)
        {
            player.Initialize(this);
        }
        CallDeferred(nameof(AdjustVolume));
        Settings.OnAudioSettingsChanged += AdjustVolume;
    }

    public void AdjustVolume()
    {
        foreach (var player in musicPlayers)
        {
            GD.Print("Adjusting BGM Volume");
            AudioStreamPlayer2D player2D = player.AudioPlayer2D;
            GD.Print($"player: {player}");
            GD.Print($"player2D: {player2D}");
            if (player2D == null) { continue; }
            GD.Print($"Volume: {AudioMaster.GetMusicVolume(player.volumeModifier)}");
            player2D.VolumeDb = AudioMaster.GetMusicVolume(player.volumeModifier);
        }
    }

    public void PlayMusic(AudioStream music, AudioStreamPlayer2D player)
    {
        if (musicPlayers.Length == 0) { return; }

        if (player.Stream == music && player.Playing) { return; }

        player.Stream = music;
        // AdjustVolume();
        player.Play();
    }

    public void StopMusic(AudioStreamPlayer2D player)
    {
        if (musicPlayers.Length == 0) { return; }

        player.Stop();
    }

    public void StopAllMusic()
    {
        if (musicPlayers.Length == 0) { return; }

        foreach (var player in musicPlayers)
        {
            player.AudioPlayer2D.Stop();
        }
    }

    public AudioStreamer GetMusicPlayer(int index)
    {
        if (index < 0 || index >= musicPlayers.Length) { return null; }
        return musicPlayers[index];
    }

    public int GetMusicPlayerCount()
    {
        return musicPlayers.Length;
    }
}
