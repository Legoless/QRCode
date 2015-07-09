// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrPixel.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code pixel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    // We will use more pixels just to ease up debugging

    /// <summary>
    /// The <c>QR</c> pixel used for debugging.
    /// </summary>
    public enum QrPixel
    {
        /// <summary>
        /// The empty.
        /// </summary>
        Empty = 0, 

        /// <summary>
        /// The reserved.
        /// </summary>
        Reserved = 1, 

        /// <summary>
        /// The white.
        /// </summary>
        White = 2, 

        /// <summary>
        /// The black.
        /// </summary>
        Black = 3, 

        /// <summary>
        /// The format.
        /// </summary>
        Format = 4, 

        /// <summary>
        /// The version.
        /// </summary>
        Version = 5, 

        /// <summary>
        /// The mask.
        /// </summary>
        Mask = 6
    }
}