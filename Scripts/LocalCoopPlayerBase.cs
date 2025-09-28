using Godot;
using System;

public partial class LocalCoopPlayerBase : Node2D
{
    [Export] private bool isKeyboard { get; set; }
    [Export] private int controllerId { get; set; }

    public override void _Input(InputEvent @event)
    {
        KeyboardInput(@event);
        GamepadInput(@event);
    }

    private void KeyboardInput(InputEvent @event)
    {
        if (!isKeyboard) { return; }
        if (@event is not InputEventKey eventKey) { return; }
        GD.Print("Keyboard Input:" + eventKey.Keycode);

        // Handle keyboard input here
    }

    private void GamepadInput(InputEvent @event)
    {
        if (isKeyboard) { return; }
        if (@event is not InputEventJoypadButton eventJoypadButton) { return; }
        if (eventJoypadButton.Device != controllerId) { return; }
        GD.Print("Gamepad Input from controller " + controllerId + ": " + eventJoypadButton.ButtonIndex);
        // Handle gamepad input here
    }
}
