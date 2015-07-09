// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrBlock.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   A single block of data in <c>qr</c> code, used to generate error correction codes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Library
{
    using QRCode.Extensions;

    /// <summary>
    /// A single block of data in <c>qr</c> code, used to generate error correction codes
    /// </summary>
    internal class QrBlock
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <c>ecc</c> coefficients.
        /// </summary>
        public byte[] EccCoefficients { get; set; }

        /// <summary>
        /// Gets or sets the message poly.
        /// </summary>
        public Poly MessagePoly { get; set; }

        #endregion
    }
}