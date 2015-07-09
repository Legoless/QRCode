// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrConst.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    /// <summary>
    /// The <c>QR</c> code constants.
    /// </summary>
    public static class QrConst
    {
        // XOR constant for format
        // 101010000010010
        #region Constants

        /// <summary>
        /// The constant format <c>xor</c>.
        /// </summary>
        public const int ConstFormatXor = 0x5412;

        /// <summary>
        /// The gen poly format.
        /// </summary>
        public const int GenPolyFormat = 0x537;

        /// <summary>
        /// The gen poly version.
        /// </summary>
        public const int GenPolyVersion = 0x1f25;

        /// <summary>
        /// The galois field count.
        /// </summary>
        public const int GfFieldCount = 256;

        /// <summary>
        /// The <c>QR</c> code field.
        /// </summary>
        public const int QrCodeField = 0x011D;

        #endregion
    }
}