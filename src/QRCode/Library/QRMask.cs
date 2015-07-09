// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrMask.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> mask object wrapper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Library
{
    using System.Collections.Generic;

    using NLog;

    /// <summary>
    /// The <c>QR</c> mask object wrapper
    /// </summary>
    public class QrMask
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The _type.
        /// </summary>
        private readonly int type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrMask"/> class.
        /// </summary>
        /// <param name="codeType">
        /// The type.
        /// </param>
        public QrMask(int codeType)
        {
            this.type = codeType;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get bits.
        /// </summary>
        /// <returns>
        /// Bits in queue.
        /// </returns>
        public Queue<bool> GetBits()
        {
            Queue<bool> bits = new Queue<bool>();

            switch (this.type)
            {
                case 1:
                    bits.Enqueue(false);
                    bits.Enqueue(false);
                    bits.Enqueue(true);

                    break;
                case 2:
                    bits.Enqueue(false);
                    bits.Enqueue(true);
                    bits.Enqueue(false);

                    break;
                case 3:
                    bits.Enqueue(false);
                    bits.Enqueue(true);
                    bits.Enqueue(true);

                    break;
                case 4:
                    bits.Enqueue(true);
                    bits.Enqueue(false);
                    bits.Enqueue(false);

                    break;
                case 5:
                    bits.Enqueue(true);
                    bits.Enqueue(false);
                    bits.Enqueue(true);

                    break;
                case 6:
                    bits.Enqueue(true);
                    bits.Enqueue(true);
                    bits.Enqueue(false);

                    break;
                case 7:
                    bits.Enqueue(true);
                    bits.Enqueue(true);
                    bits.Enqueue(true);

                    break;
                default:
                    bits.Enqueue(false);
                    bits.Enqueue(false);
                    bits.Enqueue(false);

                    break;
            }

            return bits;
        }

        /// <summary>
        /// The get mask type.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaskType()
        {
            return this.type % 8;
        }

        /// <summary>
        /// Method tests if the pixel belongs to the mask.
        /// </summary>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Test(int y, int x)
        {
            switch (this.type)
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
                    return (((y * x) % 2) + ((y * x) % 3)) % 2 == 0;
                case 7:
                    return (((y + x) % 2) + ((y * x) % 3)) % 2 == 0;
                default:
                    return (y + x) % 2 == 0;
            }
        }

        /// <summary>
        /// Convert object to string representation.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            switch (this.type)
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
                    return "(((y * x) % 2) + ((y * x) % 3)) % 2 == 0";
                case 7:
                    return "(((y + x) % 2) + ((y * x) % 3)) % 2 == 0";
                default:
                    return "(y + x) % 2 == 0";
            }
        }

        #endregion
    }
}