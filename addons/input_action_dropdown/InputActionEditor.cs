#if TOOLS
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class InputActionEditor : EditorProperty
{
    private readonly OptionButton _optionButton;
    private bool _updating;

    public InputActionEditor()
    {
        _optionButton = new OptionButton();
        AddChild(_optionButton);
        SetBottomEditor(_optionButton);
        AddFocusable(_optionButton);

        RefreshItems();
        _optionButton.ItemSelected += OnItemSelected;
    }

    private void RefreshItems()
    {
        _optionButton.Clear();

        // Read actions from project.godot's [input] section
        var cfg = new ConfigFile();
        var err = cfg.Load("res://project.godot");
        if (err != Error.Ok)
        {
            GD.PrintErr($"[InputActionEditor] Failed to load project.godot: {err}");
            return;
        }

        if (!cfg.HasSection("input"))
        {
            GD.Print("[InputActionEditor] No [input] section in project.godot");
            return;
        }

        string[] keys = cfg.GetSectionKeys("input"); // action names
        Array.Sort(keys, StringComparer.Ordinal);

        foreach (string actionName in keys)
        {
            // Optional: skip built-in ui_* actions if you only want your own
            // if (actionName.StartsWith("ui_"))
            //     continue;

            _optionButton.AddItem(actionName);
        }
    }

    private void OnItemSelected(long index)
    {
        if (_updating)
            return;

        string selectedText = _optionButton.GetItemText((int)index);
        // Tell the inspector that this property's value changed.
        EmitChanged(GetEditedProperty(), selectedText);
    }

    public override void _UpdateProperty()
    {
        var obj = GetEditedObject();
        if (obj == null)
            return;

        var property = GetEditedProperty();
        if (property == default)
            return;

        var value = obj.Get(property);
        string current = value.ToString() ?? string.Empty;

        RefreshItems();

        _updating = true;

        int foundIndex = -1;
        for (int i = 0; i < _optionButton.ItemCount; i++)
        {
            if (_optionButton.GetItemText(i) == current)
            {
                foundIndex = i;
                break;
            }
        }

        if (foundIndex >= 0)
            _optionButton.Select(foundIndex);
        else
            _optionButton.Select(-1);

        _updating = false;
    }

    public override void _SetReadOnly(bool readOnly)
    {
        _optionButton.Disabled = readOnly;
    }
}
#endif
