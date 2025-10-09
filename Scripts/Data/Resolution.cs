public struct Resolution
{
    public string label => ToString();
    public int width;
    public int height;
    public AspectRatio aspectRatio;

    public Resolution(int w, int h)
    {
        width = w;
        height = h;
        aspectRatio = CalculateAspectRatio(w, h);
    }

    private AspectRatio CalculateAspectRatio(int w, int h)
    {
        int a = w;
        int b = h;
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return new AspectRatio(w / a, h / a);
    }

    public override string ToString()
    {
        return $"{width} x {height}";
    }
}