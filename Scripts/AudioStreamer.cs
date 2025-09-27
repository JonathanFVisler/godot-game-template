using Godot;
using System;

[GlobalClass]
public partial class AudioStreamer : Resource
{
    [Export] private NodePath audioStreamPlayerPath;
    private AudioStreamPlayer2D _audioStreamPlayer2D;
    private Node _rootNode; // Store a reference to the root node

    public void Initialize(Node rootNode)
    {
        _rootNode = rootNode;
    }

    public AudioStreamPlayer2D AudioPlayer2D
    {
        get
        {
            if (_audioStreamPlayer2D == null && !string.IsNullOrEmpty(audioStreamPlayerPath) && _rootNode != null)
            {
                _audioStreamPlayer2D = _rootNode.GetNode<AudioStreamPlayer2D>(audioStreamPlayerPath);
            }
            return _audioStreamPlayer2D;
        }
    }

    [Export] public float volumeModifier { get; set; } = 1f;
}
