using QRCode.Extensions;

namespace QRCode.Library
{
    internal class QRBlock
    {
        public Poly MessagePoly
        {
            get;
            set;
        }

        public byte[] EccCoefficients
        {
            get;
            set;
        }
    }
}