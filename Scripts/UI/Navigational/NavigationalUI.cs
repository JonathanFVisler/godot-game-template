using Godot;
using System;

public partial class NavigationalUI : Control
{
    [Export] public NavigationalStep[] navigationalSteps;
    public override void _Input(InputEvent @event)
    {
        if(this.HasFocus())
        {
            foreach(NavigationalStep step in navigationalSteps)
            {
                if(@event.IsActionPressed(step.actionName))
                {
                    if(step.nextUIElement != null)
                    {
                        Control target = GetNodeOrNull<Control>(step.nextUIElement);
                        if (target != null)
                        {
                            target.GrabFocus();
                        }
                    }
                }
            }
        }
    }
}
