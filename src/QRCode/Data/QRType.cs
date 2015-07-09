// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrType.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    /// <summary>
    /// The <c>QR</c> code type.
    /// </summary>
    public enum QrType
    {
        /// <summary>
        /// The numeric.
        /// </summary>
        Numeric = 0, 

        /// <summary>
        /// The alpha numeric.
        /// </summary>
        AlphaNumeric = 1, 

        /// <summary>
        /// The binary.
        /// </summary>
        Binary = 2, 

        /// <summary>
        /// The japanese.
        /// </summary>
        Japanese = 3
    }
}