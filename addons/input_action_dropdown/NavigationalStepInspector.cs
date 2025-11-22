#if TOOLS
using Godot;

[Tool]
public partial class NavigationalStepInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        // Only for your NavigationalStep Resource.
        return @object is NavigationalStep;
    }

    public override bool _ParseProperty(
        GodotObject @object,
        Variant.Type type,
        string name,
        PropertyHint hintType,
        string hintString,
        PropertyUsageFlags usageFlags,
        bool wide)
    {
        // Replace the editor for the "actionName" export.
        if (name == "actionName")
        {
            var editor = new InputActionEditor();
            AddPropertyEditor(name, editor);
            // Returning true tells Godot to NOT create the default editor.
            return true;
        }

        return false;
    }
}
#endif
