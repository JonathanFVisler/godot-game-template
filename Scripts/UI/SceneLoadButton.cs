using Godot;
using System;

public partial class SceneLoadButton : Button
{
    [Export] private GameScene sceneToLoad = GameScene.MAINMENU;

    public override void _Ready()
    {
        this.Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        SceneLoader.Instance.LoadScene(sceneToLoad);
    }
}
