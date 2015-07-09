namespace QRCode.Data
{
    //
    // We will use more pixels just to ease up debugging
    //

    public enum QRPixel
    {
        Empty = 0,
        Reserved = 1,
        White = 2,
        Black = 3,
        Format = 4,
        Version = 5,
        Mask = 6
    }
}