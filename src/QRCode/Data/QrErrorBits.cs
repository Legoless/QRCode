// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrErrorBits.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code error bits wrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    using System.Collections.Generic;

    using NLog;

    /// <summary>
    /// The <c>QR</c> code error bits wrapper.
    /// </summary>
    public class QrErrorBits
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get bits.
        /// </summary>
        /// <param name="err">
        /// The err.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetBits(QrError err)
        {
            switch (err)
            {
                case QrError.L:
                    return 0x01;
                case QrError.M:
                    return 0x00;
                case QrError.Q:
                    return 0x03;
                default:
                    return 0x02;
            }
        }

        /// <summary>
        /// The get error bits.
        /// </summary>
        /// <param name="err">
        /// The err.
        /// </param>
        /// <returns>
        /// The queue of <c>bools</c>.
        /// </returns>
        public static Queue<bool> GetErrorBits(QrError err)
        {
            Queue<bool> bits = new Queue<bool>();

            switch (err)
            {
                case QrError.L:
                    bits.Enqueue(false);
                    bits.Enqueue(true);

                    break;
                case QrError.M:
                    bits.Enqueue(false);
                    bits.Enqueue(false);

                    break;
                case QrError.Q:
                    bits.Enqueue(true);
                    bits.Enqueue(true);

                    break;
                default:
                    bits.Enqueue(true);
                    bits.Enqueue(false);

                    break;
            }

            return bits;
        }

        #endregion
    }
}