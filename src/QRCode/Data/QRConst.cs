namespace QRCode.Data
{
    public class QRConst
    {
        //
        // QRCode uses GF (2 pow 8) Galois Field
        //

        // x^8 + x^4 + x^3 + x^2 + 1 -> 100011101
        public const int QRCodeField = 0x011D;
        public const int GfFieldCount = 256;

        //
        // Format information generator polynomial BCH (15,5) -> Annex C -> Format information
        //

        // x10 + x8 + x5 + x4 + x2 + x + 1 -> 101 0011 0111
        public const int GenPolyFormat = 0x537;

        // XOR constant for format
        // 101010000010010
        public const int ConstFormatXor = 0x5412;

        //
        // Version information generator polynomial BCH (18,6) -> Annex D -> Version information
        //

        // x12 + x11 + x9 + x8 + x5 + x2 + 1 -> 1 1111 0010 0101
        public const int GenPolyVersion = 0x1f25;
    }
}