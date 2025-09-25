public struct Resolution
{
    public string label => ToString();
    public int width;
    public int height;

    public Resolution(int w, int h)
    {
        width = w;
        height = h;
    }

    public override string ToString()
    {
        return $"{width} x {height}";
    }
}