using Godot;
using System;

public partial class AspectRatio : Node
{
    public int width;
    public int height;
    public string label => ToString();

    public AspectRatio(int w, int h)
    {
        width = w;
        height = h;
    }

    public override string ToString()
    {
        return $"{width}:{height}";
    }

    public override bool Equals(object obj)
    {
        object other = obj as AspectRatio;
        if (other == null) return false;
        return width == ((AspectRatio)other).width && height == ((AspectRatio)other).height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(width, height);
    }
}
