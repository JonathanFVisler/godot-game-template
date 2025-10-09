using Godot;
using System;

public partial class AspectRatioDropdown : OptionButton
{
    public override void _Ready()
    {
        CallDeferred(nameof(PopulateMenu));

        ItemSelected += OnItemSelected;
    }

    private void PopulateMenu()
    {

        Clear();
        foreach (var ar in SettingsMenuManager.Instance.availableAspectRatios)
        {
            AddItem(ar.ToString());
        }

        int currentIndex = 0;
        for (int i = 0; i < SettingsMenuManager.Instance.availableAspectRatios.Length; i++)
        {
            var ar = SettingsMenuManager.Instance.availableAspectRatios[i];
            if (ar.width == Settings.TargetAspectRatio.width && ar.height == Settings.TargetAspectRatio.height)
            {
                currentIndex = i;
                break;
            }
        }
        Selected = currentIndex;
    }

    private void OnItemSelected(long index)
    {
        GD.Print($"AspectRatioDropdown: Selected index {index}");
    }
}
