using Godot;
using System;

public partial class ResolutionDropdown : OptionButton
{
    public override void _Ready()
    {
        CallDeferred(nameof(PopulateMenu));

        ItemSelected += OnItemSelected;
    }

    private void PopulateMenu()
    {
        GD.Print("ResolutionDropdown: Populating menu for aspect ratio " + Settings.TargetAspectRatio);
        Clear();
        foreach (var res in SettingsMenuManager.Instance.availableResolutions)
        {
            if (!res.aspectRatio.Equals(Settings.TargetAspectRatio)) { continue; }
            AddItem(res.ToString());
        }

        int currentIndex = 0;
        for (int i = 0; i < SettingsMenuManager.Instance.availableResolutions.Length; i++)
        {
            var res = SettingsMenuManager.Instance.availableResolutions[i];
            if (res.width == Settings.ScreenWidth && res.height == Settings.ScreenHeight)
            {
                currentIndex = i;
                break;
            }
        }
        Selected = currentIndex;
    }

    private void OnItemSelected(long index)
    {
        GD.Print($"ResolutionDropdown: Selected index {index}");
    }
}
