namespace QRCode.Data
{
    public enum QRBreakPoint
    {
        CreatedMatrix = 0,
        OutputTypeBits = 1,
        OutputDataLength = 2,
        EncodedDataToBinary = 3,
        FinishedBinary = 4,
        BrokenBinary = 5,
        MissingBytes = 6,
        ErrorCode = 7,
        Filled = 8,
        Mask = 9,
        None = 10
    }
}