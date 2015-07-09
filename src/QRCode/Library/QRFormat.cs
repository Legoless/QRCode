// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrFormat.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code format object wrapper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Library
{
    using System.Collections.Generic;
    using System.Text;

    using NLog;

    using QRCode.Data;
    using QRCode.ErrorCorrection;
    using QRCode.Extensions;

    /// <summary>
    /// The <c>QR</c> code format object wrapper
    /// </summary>
    public class QrFormat
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The debug data.
        /// </summary>
        private readonly StringBuilder debugData;

        /// <summary>
        /// The output.
        /// </summary>
        private readonly Queue<bool> output = new Queue<bool>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrFormat"/> class.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="mask">
        /// The mask.
        /// </param>
        public QrFormat(QrError error, QrMask mask)
        {
            this.CalculateFormat(error, mask);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrFormat"/> class.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="mask">
        /// The mask.
        /// </param>
        /// <param name="debug">
        /// The debug.
        /// </param>
        public QrFormat(QrError error, QrMask mask, bool debug)
        {
            if (debug)
            {
                this.debugData = new StringBuilder();
            }

            this.CalculateFormat(error, mask);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns debug data as a string
        /// </summary>
        /// <returns>
        /// Debug data as a <see cref="string"/>.
        /// </returns>
        public string GetDebugData()
        {
            if (this.debugData != null)
            {
                return this.debugData.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns format bits.
        /// </summary>
        /// <returns>
        /// The list of <c>bools</c> in queue.
        /// </returns>
        public Queue<bool> GetFormatBits()
        {
            return this.output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate <c>QR</c> code format
        /// </summary>
        /// <param name="error">
        /// The error code information.
        /// </param>
        /// <param name="mask">
        /// The mask.
        /// </param>
        private void CalculateFormat(QrError error, QrMask mask)
        {
            // Get error bits
            if (this.debugData != null)
            {
                this.debugData.Append("- Calculating format bits...\n");
            }

            // Get 5 bits of format information
            int formatInfo = (QrErrorBits.GetBits(error) << 3) | BinaryExtensions.BinToDec(mask.GetBits());

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Format information error and mask bits: "
                    + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(formatInfo)) + "\n");
                this.debugData.Append("- Calculating BCH (15,5) code for information bits...\n");
            }

            int bchCode = QrBoseChaudhuriHocquenghem.CalculateBch(formatInfo, QrConst.GenPolyFormat);

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- BCH (15,5) error code: " + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(bchCode))
                    + "\n");
            }

            // Append BCH code to format info
            formatInfo = (formatInfo << 10) | bchCode;

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Format information with appended BCH: "
                    + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(formatInfo)) + "\n");
                this.debugData.Append(
                    "- Calculating XOR with format constant: "
                    + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(QrConst.ConstFormatXor)) + "\n");
            }

            formatInfo = formatInfo ^ QrConst.ConstFormatXor;

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Final format: " + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(formatInfo, 15))
                    + "\n");
            }

            List<bool> formatBits = BinaryExtensions.DecToBin(formatInfo, 15);

            // Reverse, because our QRCode is inserting backwards
            formatBits.Reverse();

            foreach (bool t in formatBits)
            {
                this.output.Enqueue(t);
            }
        }

        #endregion
    }
}