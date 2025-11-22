#if TOOLS
using Godot;

[Tool]
public partial class InputActionPlugin : EditorPlugin
{
    private NavigationalStepInspector _inspector;

    public override void _EnterTree()
    {
        _inspector = new NavigationalStepInspector();
        AddInspectorPlugin(_inspector);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }
    }
}
#endif
