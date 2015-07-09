// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrBoseChaudhuriHocquenghem.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>Bose Chaudhuri Hocquenghem</c> algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.ErrorCorrection
{
    using NLog;

    /// <summary>
    /// The <c>Bose Chaudhuri Hocquenghem</c> algorithm.
    /// </summary>
    public class QrBoseChaudhuriHocquenghem
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The calculate bch.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="poly">
        /// The poly.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CalculateBch(int value, int poly)
        {
            // Get poly power, we subtract one because finite field start with x0
            int polyPower = CountDigits(poly);
            value = value << (polyPower - 1);

            // Dividing until we reach same power
            while (CountDigits(value) >= polyPower)
            {
                value = value ^ (poly << (CountDigits(value) - polyPower));
            }

            // Return reminder
            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The count digits.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CountDigits(int value)
        {
            int digits = 0;

            while (value != 0)
            {
                value = (int)((uint)value >> 1);
                digits++;
            }

            return digits;
        }

        #endregion
    }
}