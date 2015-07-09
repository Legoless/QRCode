using System.Collections.Generic;

namespace QRCode.Data
{
    public enum QRError
    {
        L = 0,
        M = 1,
        Q = 2,
        H = 3
    }

    public class QRErrorBits
    {
        public static Queue<bool> GetErrorBits (QRError err)
        {
            Queue<bool> bits = new Queue<bool> ();

            switch (err)
            {
                case QRError.L:
                    bits.Enqueue (false);
                    bits.Enqueue (true);

                    break;
                case QRError.M:
                    bits.Enqueue (false);
                    bits.Enqueue (false);

                    break;
                case QRError.Q:
                    bits.Enqueue (true);
                    bits.Enqueue (true);

                    break;
                default:
                    bits.Enqueue (true);
                    bits.Enqueue (false);

                    break;
            }

            return bits;
        }

        public static int GetBits (QRError err)
        {
            switch (err)
            {
                case QRError.L:
                    return 0x01;
                case QRError.M:
                    return 0x00;
                case QRError.Q:
                    return 0x03;
                default:
                    return 0x02;
            }
        }
    }
}