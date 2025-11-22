using Godot;
using System;

[Tool]
[GlobalClass]
public partial class NavigationalStep : Resource
{
    [Export] public string actionName;
    [Export] public NodePath nextUIElement;
}
