namespace Convert_Color_Picture_to_Grayscale
{
    public class Pixel
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}