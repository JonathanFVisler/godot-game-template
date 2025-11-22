using Godot;
using System;

public partial class SceneLoadButton : NavigationalUI
{
    [Export] private GameScene sceneToLoad = GameScene.MAINMENU;
    [Export] public string actionName = "enter";
    [Export] public bool autoFocusOnInitialisation = false;


    public override void _Ready()
    {
        if(autoFocusOnInitialisation)
        {
            GrabFocus();
        }
    }

    private void OnButtonPressed()
    {
        SceneLoader.Instance.LoadScene(sceneToLoad);
    }

    public override void _Input(InputEvent @event)
    {
        if(this.HasFocus())
        {
            if(@event.IsActionPressed(actionName))
            {
                OnButtonPressed();
            }
        }
    }
}
