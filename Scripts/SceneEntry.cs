using Godot;

[GlobalClass]
public partial class SceneEntry : Resource
{
    [Export] public GameScene SceneType { get; set; }
    [Export] public PackedScene Scene { get; set; }
}
