// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrBreakPoint.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The break point for debugging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    /// <summary>
    /// The break point for debugging.
    /// </summary>
    public enum QrBreakPoint
    {
        /// <summary>
        /// The created matrix.
        /// </summary>
        CreatedMatrix = 0, 

        /// <summary>
        /// The output type bits.
        /// </summary>
        OutputTypeBits = 1, 

        /// <summary>
        /// The output data length.
        /// </summary>
        OutputDataLength = 2, 

        /// <summary>
        /// The encoded data to binary.
        /// </summary>
        EncodedDataToBinary = 3, 

        /// <summary>
        /// The finished binary.
        /// </summary>
        FinishedBinary = 4, 

        /// <summary>
        /// The broken binary.
        /// </summary>
        BrokenBinary = 5, 

        /// <summary>
        /// The missing bytes.
        /// </summary>
        MissingBytes = 6, 

        /// <summary>
        /// The error code.
        /// </summary>
        ErrorCode = 7, 

        /// <summary>
        /// The filled.
        /// </summary>
        Filled = 8, 

        /// <summary>
        /// The mask.
        /// </summary>
        Mask = 9, 

        /// <summary>
        /// The none.
        /// </summary>
        None = 10
    }
}