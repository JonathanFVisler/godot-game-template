using Godot;
using System;

public partial class PercentateLabel : Label
{
    public void SetValue(float value)
    {
        int percentage = (int)(value);
        Text = $"{percentage}%";
    }
}
