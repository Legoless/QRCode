// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrCodeCreator.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   Main <c>QR</c> code class, should be used to generate codes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Library
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    using NLog;

    using QRCode.Data;
    using QRCode.ErrorCorrection;
    using QRCode.Extensions;

    /// <summary>
    /// Main <c>QR</c> code class, should be used to generate codes
    /// </summary>
    public class QrCodeCreator
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        // Debug
        #region Fields

        /// <summary>
        /// The output.
        /// </summary>
        private readonly Queue<bool> output = new Queue<bool>();

        /// <summary>
        /// The text.
        /// </summary>
        private readonly string text;

        /// <summary>
        /// The break point.
        /// </summary>
        private QrBreakPoint breakPoint;

        /// <summary>
        /// The debug data.
        /// </summary>
        private StringBuilder debugData;

        /// <summary>
        /// The <c>QrImage</c> element.
        /// </summary>
        private QrImage img;

        /// <summary>
        /// The mask.
        /// </summary>
        private QrMask mask;

        /// <summary>
        /// The type.
        /// </summary>
        private QrType type;

        /// <summary>
        /// The version.
        /// </summary>
        private QrVersion version;

        #endregion

        // Constructor only makes
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrCodeCreator"/> class.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        public QrCodeCreator(string t)
        {
            this.text = t;
        }

        #endregion

        // Initializers
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
        /// Renders <c>QR</c> code to an image object
        /// </summary>
        /// <param name="resolution">
        /// Integer 1-5 defines how many pixels are per bit
        /// </param>
        /// <returns>
        /// Rendered image object <see cref="Image"/>.
        /// </returns>
        public Image Render(int resolution)
        {
            if (this.img == null)
            {
                this.Process();
            }

            int res = 1;

            switch (resolution)
            {
                case 1:
                    res = 2;

                    break;
                case 2:
                    res = 4;

                    break;
                case 3:
                    res = 8;

                    break;
                case 4:
                    res = 16;

                    break;
                case 5:
                    res = 32;

                    break;
            }

            if (this.img != null)
            {
                return this.img.Render(res);
            }

            return null;
        }

        /// <summary>
        /// Advanced mode wrapper
        /// </summary>
        /// <param name="t">
        /// <c>QR</c> code type
        /// </param>
        /// <param name="v">
        /// <c>QR</c> code version
        /// </param>
        /// <param name="m">
        /// <c>QR</c> code mask
        /// </param>
        public void SetAdvancedMode(QrType t, QrVersion v, QrMask m)
        {
            this.type = t;
            this.version = v;
            this.mask = m;
        }

        /// <summary>
        /// Set debug mode breakpoint.
        /// </summary>
        /// <param name="breakPointSet">
        /// The break point.
        /// </param>
        public void SetDebugMode(QrBreakPoint breakPointSet)
        {
            this.breakPoint = breakPointSet;
            this.debugData = new StringBuilder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The choose versions.
        /// </summary>
        private void ChooseVersions()
        {
            // Automatic setting
            if ((this.version == null) || (this.mask == null))
            {
                this.type = QrType.AlphaNumeric;
                this.version = new QrVersion(1, QrError.Q);
                this.mask = new QrMask(2);
            }
        }

        /// <summary>
        /// The process.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Throws argument exception if wrong type or version are selected
        /// </exception>
        /// <exception cref="Exception">
        /// Throws exception if text is too long for the matrix
        /// </exception>
        private void Process()
        {
            // Preprocess
            // -> one of the parameters was not set, simple mode
            // -> create structure of QR Image
            this.ChooseVersions();

            if (this.debugData != null)
            {
                this.img = new QrImage(this.version, this.mask, this.breakPoint);

                this.debugData.Append(this.img.GetDebugData());
            }
            else
            {
                this.img = new QrImage(this.version, this.mask);
            }

            if ((this.breakPoint == QrBreakPoint.CreatedMatrix) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 1 -------");

                return;
            }

            // Step one - sending type to output
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 1 ------------------\n- Sending type bits (4) to output ...\n");
            }

            switch (this.type)
            {
                case QrType.Numeric:
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);
                    this.output.Enqueue(true);

                    break;

                case QrType.AlphaNumeric:
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);
                    this.output.Enqueue(true);
                    this.output.Enqueue(false);

                    break;

                case QrType.Binary:
                    this.output.Enqueue(false);
                    this.output.Enqueue(true);
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);

                    break;
                case QrType.Japanese:
                    this.output.Enqueue(true);
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);
                    this.output.Enqueue(false);

                    break;
                default:
                    throw new ArgumentException("Wrong QRType selected.");
            }

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Sent 4 bytes of output: " + BinaryExtensions.BinaryToString(this.output) + "\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.OutputTypeBits) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 2 -------");

                return;
            }

            // Step two - write length of string
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 2 ------------------\n- Calculating bit size for data length ...\n");
            }

            byte strLength;

            if ((this.version.Version >= 1) && (this.version.Version <= 9))
            {
                switch (this.type)
                {
                    case QrType.Numeric:
                        strLength = 10;
                        break;
                    case QrType.AlphaNumeric:
                        strLength = 9;
                        break;
                    case QrType.Binary:
                        strLength = 8;
                        break;
                    case QrType.Japanese:
                        strLength = 8;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else if ((this.version.Version >= 10) && (this.version.Version <= 26))
            {
                switch (this.type)
                {
                    case QrType.Numeric:
                        strLength = 12;
                        break;
                    case QrType.AlphaNumeric:
                        strLength = 11;
                        break;
                    case QrType.Binary:
                        strLength = 16;
                        break;
                    case QrType.Japanese:
                        strLength = 10;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else if ((this.version.Version >= 27) && (this.version.Version <= 40))
            {
                switch (this.type)
                {
                    case QrType.Numeric:
                        strLength = 14;
                        break;
                    case QrType.AlphaNumeric:
                        strLength = 13;
                        break;
                    case QrType.Binary:
                        strLength = 16;
                        break;
                    case QrType.Japanese:
                        strLength = 12;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else
            {
                throw new ArgumentException("Wrong QRVersion selected.");
            }

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- QR Type: " + this.type + ", QR Version: " + this.version.Version + ", Bit size: " + strLength
                    + " bits.\n");
            }

            // Write length to output
            int length = this.text.Length;

            this.ToOutput(BinaryExtensions.DecToBin(length, strLength));

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Stored data length to output: "
                    + BinaryExtensions.BinaryToString(BinaryExtensions.DecToBin(length, strLength)) + "\n");
                this.debugData.Append(
                    "- Data stored in output: " + BinaryExtensions.BinaryToString(this.output) + "\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.OutputDataLength) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 3 -------");

                return;
            }

            // Step 3 - encoding entry text
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 3 ------------------\n- Creating code table for input ...\n");
            }

            QrCodeTable table = new QrCodeTable(this.type);

            if (this.debugData != null)
            {
                this.debugData.Append("- Checking for odd input length.\n");
            }

            // Odd text length handling
            if (length % 2 == 1)
            {
                if (this.debugData != null)
                {
                    this.debugData.Append(" - Text length is odd, reducing size for one character.\n");
                }

                length = length - 1;
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Writing characters ...\n");
            }

            for (int i = 0; i < length; i += 2)
            {
                // Get number
                int calc = (table.GetCharCount() * table.GetCodeByChar(this.text[i]))
                           + table.GetCodeByChar(this.text[i + 1]);

                if (this.debugData != null)
                {
                    this.debugData.Append(
                        " - Characters: " + this.text[i] + ", " + this.text[i + 1] + ": (" + table.GetCharCount()
                        + " * " + table.GetCodeByChar(this.text[i]) + ") + " + table.GetCodeByChar(this.text[i + 1])
                        + " = " + calc + "\n");
                }

                // Write number, why 11 bits for each sign?????
                // Experimental: strLength as calculated for size.
                this.ToOutput(BinaryExtensions.DecToBin(calc, 11));
            }

            // Handling last 6 bits for odd length
            if (this.text.Length % 2 == 1)
            {
                if (this.debugData != null)
                {
                    this.debugData.Append(
                        "- Writing last (odd) character " + this.text[this.text.Length - 1] + " with " + 6 + " bits: "
                        + table.GetCodeByChar(this.text[this.text.Length - 1]) + "\n");
                }

                this.ToOutput(BinaryExtensions.DecToBin(table.GetCodeByChar(this.text[this.text.Length - 1]), 6));
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Input successfully encoded to binary.\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.EncodedDataToBinary) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 4 -------");

                return;
            }

            // Step 4 - finishing binary string
            if (this.debugData != null)
            {
                this.debugData.Append("------------------ Step 4 ------------------\n- Finishing binary string ...\n");
            }

            // Calculate maximum bit length
            int maxBits = this.version.DataSize * 8;

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Maximum bits for current version: " + maxBits + " current bits: " + this.output.Count + "\n");
            }

            // Add zeros to end of bit string, if we are missing them
            int bitOverflow = 0;

            while (this.output.Count < maxBits)
            {
                this.output.Enqueue(false);

                bitOverflow++;

                if (bitOverflow >= 4)
                {
                    break;
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Added " + bitOverflow + " bits to binary output. Total: " + this.output.Count + " bits.\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.FinishedBinary) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 5 -------");

                return;
            }

            // Step 5 - breaking the bit string into bytes
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 5 ------------------\n- Dividing bits into bytes (8 bits per byte) ...\n");
                this.debugData.Append("- Current output size: " + this.output.Count + " bits\n");
            }

            // For now we will just make sure it is divided by 8, we will not convert to bytes unless required.
            while (this.output.Count % 8 != 0)
            {
                this.output.Enqueue(false);
            }

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Added zeros to output, current size: " + this.output.Count + " bits, "
                    + (this.output.Count / 8) + " bytes.\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.BrokenBinary) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 6 -------");

                return;
            }

            // Step 6 - inserting characters
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 6 ------------------\n- Inserting characters to fill missing data string ...\n");
            }

            // If we have too many bits
            if ((this.output.Count / 8) > this.version.DataSize)
            {
                throw new Exception("Text is too long for current matrix.");
            }

            // If we have less or exact number of bits
            List<bool> defaultBlock1 = BinaryExtensions.DecToBin(236, 8);
            List<bool> defaultBlock2 = BinaryExtensions.DecToBin(17, 8);

            // Fill matrix until it is full as specified in version
            int fillCount = 0;

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Current output size: " + (this.output.Count / 8) + ", missing: "
                    + (this.version.DataSize - (this.output.Count / 8)) + " bytes.\n");
                this.debugData.Append("- Added bytes: ");
            }

            while ((this.output.Count / 8) < this.version.DataSize)
            {
                if ((fillCount % 2) == 0)
                {
                    if (this.debugData != null)
                    {
                        this.debugData.Append("236, ");
                    }

                    this.ToOutput(defaultBlock1);
                }
                else
                {
                    if (this.debugData != null)
                    {
                        this.debugData.Append("17, ");
                    }

                    this.ToOutput(defaultBlock2);
                }

                fillCount++;
            }

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "\n- Output size filled to match data size: " + (this.output.Count / 8) + "/"
                    + this.version.DataSize + " bytes\n\n");
            }

            if ((this.breakPoint == QrBreakPoint.MissingBytes) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 7 -------");

                return;
            }

            // Step 7 - generating error code correction (ECC) by Reed-Solomon procedure

            // Convert bits from output to byte array
            byte[] bytes = BinaryExtensions.BitArrayToByteArray(new BitArray(this.output.ToArray()));

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 7 ------------------\n- Calculating error code correction ...\n");
                this.debugData.Append(
                    "- Dividing data words into blocks: " + this.version.GetBlockCount() + " total.\n");
                this.debugData.Append("- Creating message polynomial for each block ...\n");
            }

            // Divide bytes into blocks
            QrBlock[] blocks = new QrBlock[this.version.GetBlockCount()];

            for (int x = 0; x < this.version.GetBlockCount(); x++)
            {
                // Creating message polynominal system (F(x)) for current block

                // Calculate block length and starting position
                int blockLength;
                int blockStart;

                if (x >= this.version.BlockOneCount)
                {
                    blockLength = this.version.BlockTwoSize;

                    blockStart = (this.version.BlockOneSize * this.version.BlockOneCount)
                                 + ((x - this.version.BlockOneCount) * this.version.BlockTwoSize);
                }
                else
                {
                    blockLength = this.version.BlockOneSize;

                    blockStart = this.version.BlockOneSize * x;
                }

                if (this.debugData != null)
                {
                    this.debugData.Append(
                        "----------------------------------\n- Block " + (x + 1) + ", data from: " + blockStart
                        + ", length: " + blockLength + "\n- Message polynomial:\n\n");
                }

                Poly message = new Poly();

                int messageLevel = this.version.BlockOneSize
                                   + (this.version.ErrorSize / this.version.GetBlockCount()) - 1;

                for (int i = 0; i < blockLength; i++)
                {
                    if (this.debugData != null)
                    {
                        this.debugData.Append("(" + bytes[i + blockStart] + "x ^ " + messageLevel + ")");

                        if (i != blockLength - 1)
                        {
                            this.debugData.Append(" + ");
                        }
                    }

                    Term ter = new Term(messageLevel, bytes[i + blockStart]);
                    message.Terms.Add(ter);

                    messageLevel--;
                }

                if (this.debugData != null)
                {
                    this.debugData.Append(
                        "\n\n- Calculating "
                        + ((int)Math.Floor(this.version.ErrorSize / (double)this.version.GetBlockCount()))
                        + " error codes using Reed-Solomon algorithm.\n");
                }

                // Calculate Error codes of desired block
                blocks[x] = new QrBlock { MessagePoly = new Poly(message) };

                // Create Reed Solomon algorithm class
                QrReedSolomon ecc = new QrReedSolomon();

                // Experimental bugfix: rounding the result, so we can choose correct ECC
                blocks[x].EccCoefficients = ecc.Calculate(
                    message, (int)Math.Floor(this.version.ErrorSize / (double)this.version.GetBlockCount()));

                if (this.debugData != null)
                {
                    this.debugData.Append(
                        "- Calculated error codes: " + BinaryExtensions.ArrayToString(blocks[x].EccCoefficients)
                        + "\n\n");
                }
            }

            // Write error codes to bit stream

            // Clear current output
            this.output.Clear();

            // Writing data bits
            if (this.debugData != null)
            {
                this.debugData.Append("- Writing data bits into output:\n- ");
            }

            // Blocks might not be the same size...
            for (int i = 0; i < Math.Max(this.version.BlockOneSize, this.version.BlockTwoSize); i++)
            {
                foreach (QrBlock t in blocks)
                {
                    if (i < t.MessagePoly.Terms.Length)
                    {
                        this.ToOutput(BinaryExtensions.DecToBin(t.MessagePoly.Terms[i].Coefficient, 8));

                        if (this.debugData != null)
                        {
                            this.debugData.Append(t.MessagePoly.Terms[i].Coefficient + ", ");
                        }
                    }
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("\n\n- Writing ECC bits to output:\n- ");
            }

            // Writting ECC bits
            for (int i = 0; i < blocks[0].EccCoefficients.Length; i++)
            {
                if (i == 17)
                {
                    if (this.debugData != null)
                    {
                        this.debugData.Append(",");
                    }
                }

                foreach (QrBlock t in blocks)
                {
                    this.ToOutput(BinaryExtensions.DecToBin(t.EccCoefficients[i], 8));

                    if (this.debugData != null)
                    {
                        this.debugData.Append(t.EccCoefficients[i] + ", ");
                    }
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("\n\n");
            }

            // Step 8, 9, 10 - Handled by QRImage class
            if ((this.breakPoint == QrBreakPoint.ErrorCode) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 8 -------");

                return;
            }

            this.img.CreateImage(this.output);

            if (this.debugData != null)
            {
                this.debugData.Append(this.img.GetDebugData());
            }
        }

        /// <summary>
        /// The to output.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        private void ToOutput(IEnumerable<bool> list)
        {
            foreach (bool b in list)
            {
                this.output.Enqueue(b);
            }
        }

        #endregion
    }
}