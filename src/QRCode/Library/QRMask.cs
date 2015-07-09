using System.Collections.Generic;

namespace QRCode.Library
{
    public class QRMask
    {
        private readonly int _type;

        public QRMask (int type)
        {
            _type = type;
        }

        public int GetMaskType ()
        {
            return _type % 8;
        }

        public Queue<bool> GetBits ()
        {
            Queue<bool> bits = new Queue<bool> ();

            switch (_type)
            {
                case 1:
                    bits.Enqueue (false);
                    bits.Enqueue (false);
                    bits.Enqueue (true);

                    break;
                case 2:
                    bits.Enqueue (false);
                    bits.Enqueue (true);
                    bits.Enqueue (false);

                    break;
                case 3:
                    bits.Enqueue (false);
                    bits.Enqueue (true);
                    bits.Enqueue (true);

                    break;
                case 4:
                    bits.Enqueue (true);
                    bits.Enqueue (false);
                    bits.Enqueue (false);

                    break;
                case 5:
                    bits.Enqueue (true);
                    bits.Enqueue (false);
                    bits.Enqueue (true);

                    break;
                case 6:
                    bits.Enqueue (true);
                    bits.Enqueue (true);
                    bits.Enqueue (false);

                    break;
                case 7:
                    bits.Enqueue (true);
                    bits.Enqueue (true);
                    bits.Enqueue (true);

                    break;
                default:
                    bits.Enqueue (false);
                    bits.Enqueue (false);
                    bits.Enqueue (false);

                    break;
            }

            return bits;
        }

        public bool Test (int y, int x)
        {
            switch (_type)
            {
                case 1:
                    return (y % 2) == 0;
                case 2:
                    return (x % 3) == 0;
                case 3:
                    return (y + x) % 3 == 0;
                case 4:
                    return ((y / 2) + (x / 3)) % 2 == 0;
                case 5:
                    return ((y * x) % 2) + ((y * x) % 3) == 0;
                case 6:
                    return ((y * x) % 2) + ((y * x) % 3) % 2 == 0;
                case 7:
                    return ((y + x) % 2) + ((y * x) % 3) % 2 == 0;
                default:
                    return (y + x) % 2 == 0;
            }
        }

        public override string ToString ()
        {
            switch (_type)
            {
                case 1:
                    return "(y % 2) == 0";
                case 2:
                    return "(x % 3) == 0";
                case 3:
                    return "(y + x) % 3 == 0";
                case 4:
                    return "((y / 2) + (x / 3)) % 2 == 0";
                case 5:
                    return "((y * x) % 2) + ((y * x) % 3) == 0";
                case 6:
                    return "((y * x) % 2) + ((y * x) % 3) % 2 == 0";
                case 7:
                    return "((y + x) % 2) + ((y * x) % 3) % 2 == 0";
                default:
                    return "(y + x) % 2 == 0";
            }
        }
    }
}