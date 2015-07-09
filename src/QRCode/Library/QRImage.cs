// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrImage.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> image object wrapper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Library
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    using NLog;

    using QRCode.Data;
    using QRCode.ErrorCorrection;
    using QRCode.Extensions;

    /// <summary>
    /// The <c>QR</c> image object wrapper
    /// </summary>
    internal class QrImage
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        // Used for method interchange
        #region Fields

        /// <summary>
        /// The break point.
        /// </summary>
        private readonly QrBreakPoint breakPoint;

        /// <summary>
        /// The debug data.
        /// </summary>
        private readonly StringBuilder debugData;

        /// <summary>
        /// The mask.
        /// </summary>
        private readonly QrMask mask;

        /// <summary>
        /// The version.
        /// </summary>
        private readonly QrVersion version;

        /// <summary>
        /// The picture.
        /// </summary>
        private QrPixel[,] picture;

        /// <summary>
        /// The static data.
        /// </summary>
        private bool[,] staticData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrImage"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="mask">
        /// The mask.
        /// </param>
        public QrImage(QrVersion version, QrMask mask)
        {
            this.version = version;
            this.mask = mask;

            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QrImage"/> class.
        /// </summary>
        /// <param name="codeVersion">
        /// The version.
        /// </param>
        /// <param name="codeMask">
        /// The mask.
        /// </param>
        /// <param name="codeBreakPoint">
        /// The breakpoint to stop image generation.
        /// </param>
        public QrImage(QrVersion codeVersion, QrMask codeMask, QrBreakPoint codeBreakPoint)
        {
            this.debugData = new StringBuilder();

            this.breakPoint = codeBreakPoint;

            this.version = codeVersion;
            this.mask = codeMask;

            this.Initialize();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create image.
        /// </summary>
        /// <param name="bits">
        /// The bits.
        /// </param>
        public void CreateImage(Queue<bool> bits)
        {
            // Step 8 - put data into matrix
            this.FillData(bits);

            if ((this.breakPoint == QrBreakPoint.Filled) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 9 -------");

                return;
            }

            // Step 9 - execute mask
            this.DoMask(this.mask);

            if ((this.breakPoint == QrBreakPoint.Mask) && (this.debugData != null))
            {
                this.debugData.Append("------- Breakpoint reached: Step 10 -------");

                return;
            }

            // Step 10 - write format and version
            this.WriteFormat(this.version.ErrorLevel, this.mask);

            this.WriteVersion(this.version);
        }

        /// <summary>
        /// The get debug data.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDebugData()
        {
            string str = this.debugData.ToString();

            this.debugData.Clear();

            return str;
        }

        /// <summary>
        /// The get image.
        /// </summary>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap GetImage()
        {
            return this.Render(1);
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public Bitmap Render(int resolution)
        {
            Bitmap img = new Bitmap(this.picture.GetLength(0) * resolution, this.picture.GetLength(1) * resolution);

            for (int i = 0; i < this.picture.GetLength(0) * resolution; i++)
            {
                for (int j = 0; j < this.picture.GetLength(1) * resolution; j++)
                {
                    Color pixelColor = Color.LightGray;

                    switch (this.picture[i / resolution, j / resolution])
                    {
                        case QrPixel.Black:
                            pixelColor = Color.Black;
                            break;
                        case QrPixel.White:
                            pixelColor = Color.White;
                            break;
                        case QrPixel.Format:
                            pixelColor = Color.Blue;
                            break;
                        case QrPixel.Reserved:
                            pixelColor = Color.Chocolate;
                            break;
                        case QrPixel.Version:
                            pixelColor = Color.Green;
                            break;
                        case QrPixel.Mask:
                            pixelColor = Color.Red;
                            break;
                    }

                    img.SetPixel(i, j, pixelColor);
                }
            }

            return img;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The do mask.
        /// </summary>
        /// <param name="codeMask">
        /// The mask.
        /// </param>
        private void DoMask(QrMask codeMask)
        {
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 9 ------------------\n- Executing mask on data pixels: "
                    + BinaryExtensions.BinaryToString(codeMask.GetBits()) + "\n");
            }

            int counter = 0;

            for (int i = 0; i < this.picture.GetLength(0); i++)
            {
                for (int j = 0; j < this.picture.GetLength(1); j++)
                {
                    int x = this.picture.GetLength(0) - i - 1;
                    int y = this.picture.GetLength(1) - j - 1;

                    if (codeMask.Test(y, x) && (this.staticData[x, y] == false) && (this.picture[x, y] != QrPixel.Empty))
                    {
                        this.picture[x, y] = (this.picture[x, y] == QrPixel.White) ? QrPixel.Black : QrPixel.White;

                        counter++;

                        if ((this.breakPoint == QrBreakPoint.Mask) && (this.debugData != null))
                        {
                            this.picture[x, y] = QrPixel.Mask;
                        }
                    }
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Mask (" + codeMask + ") written to: " + counter + " pixels.\n\n");
            }
        }

        /// <summary>
        /// Draw smaller alignment square at desired position
        /// </summary>
        /// <param name="x">
        /// x position of the point
        /// </param>
        /// <param name="y">
        /// y position of the point
        /// </param>
        private void DrawAlignment(int x, int y)
        {
            // Check if all pixels on desired position are empty

            // Draw from top left corner
            x = x - 2;
            y = y - 2;

            int hitCounter = 0;

            // If there is one pixel already filled, we do not draw anything
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (this.picture[x + i, y + j] != QrPixel.Empty)
                    {
                        hitCounter++;
                    }
                }
            }

            // There are 5 pixels at timing pattern which intersect pattern, should never be more!
            if (hitCounter > 5)
            {
                return;
            }

            // Draw square
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if ((i == 0) || (j == 0) || (i == 4) || (j == 4) || ((i > 1 && i < 3) && (j > 1 && j < 3)))
                    {
                        this.picture[x + i, y + j] = QrPixel.Black;
                        this.staticData[x + i, y + j] = true;
                    }
                    else
                    {
                        this.picture[x + i, y + j] = QrPixel.White;
                        this.staticData[x + i, y + j] = true;
                    }
                }
            }
        }

        /// <summary>
        /// The draw reserved.
        /// </summary>
        private void DrawReserved()
        {
            // Drawing black & white dotted lines to right and down corner
            if (this.debugData != null)
            {
                this.debugData.Append("- Drawing timing patterns ...\n");
            }

            int i = 8;

            while (true)
            {
                if (this.picture[i, 6] != QrPixel.Empty)
                {
                    break;
                }

                this.picture[i, 6] = (i % 2 == 0) ? QrPixel.Black : QrPixel.White;
                this.picture[6, i] = (i % 2 == 0) ? QrPixel.Black : QrPixel.White;

                this.staticData[i, 6] = true;
                this.staticData[6, i] = true;

                i++;
            }

            // There is a black pixel at bottom left square
            this.picture[8, this.picture.GetLength(0) - 8] = QrPixel.Black;
            this.staticData[8, this.picture.GetLength(0) - 8] = true;

            // Format information is in blue pixels and is stored around squares
            if (this.debugData != null)
            {
                this.debugData.Append("- Securing format information bits.\n");
            }

            // Top-Left
            for (int x = 0; x < 9; x++)
            {
                if (this.picture[8, x] == QrPixel.Empty)
                {
                    this.picture[8, x] = QrPixel.Format;
                    this.staticData[8, x] = true;
                }
            }

            for (int x = 0; x < 9; x++)
            {
                if (this.picture[x, 8] == QrPixel.Empty)
                {
                    this.picture[x, 8] = QrPixel.Format;
                    this.staticData[x, 8] = true;
                }
            }

            // Top-Right
            for (int x = 0; x < 8; x++)
            {
                if (this.picture[this.picture.GetLength(0) - x - 1, 8] == QrPixel.Empty)
                {
                    this.picture[this.picture.GetLength(0) - x - 1, 8] = QrPixel.Format;
                    this.staticData[this.picture.GetLength(0) - x - 1, 8] = true;
                }
            }

            // Bottom-Left
            for (int x = 0; x < 8; x++)
            {
                if (this.picture[8, this.picture.GetLength(0) - x - 1] == QrPixel.Empty)
                {
                    this.picture[8, this.picture.GetLength(0) - x - 1] = QrPixel.Format;
                    this.staticData[8, this.picture.GetLength(0) - x - 1] = true;
                }
            }

            // Version information is in green pixels, 2 blocks of 18 pixels (3 x 6)
            if (this.debugData != null)
            {
                this.debugData.Append("- Securing version information bits.\n");
            }

            if (this.version.Version >= 7)
            {
                // Version bits at upper right corner
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        this.picture[this.picture.GetLength(0) - 9 - x, y] = QrPixel.Version;
                        this.staticData[this.picture.GetLength(0) - 9 - x, y] = true;
                    }
                }

                // Version bits at lower left corner
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        this.picture[y, this.picture.GetLength(0) - 9 - x] = QrPixel.Version;
                        this.staticData[y, this.picture.GetLength(0) - 9 - x] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draws QR square at selected position
        /// </summary>
        /// <param name="x">
        /// x coordinate of desired square position
        /// </param>
        /// <param name="y">
        /// y coordinate of desired square position
        /// </param>
        private void DrawSquare(int x, int y)
        {
            // Draw square
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if ((i == 0) || (j == 0) || (i == 6) || (j == 6) || ((i > 1 && i < 5) && (j > 1 && j < 5)))
                    {
                        this.picture[x + i, y + j] = QrPixel.Black;
                        this.staticData[x + i, y + j] = true;
                    }
                    else
                    {
                        this.picture[x + i, y + j] = QrPixel.White;
                        this.staticData[x + i, y + j] = true;
                    }
                }
            }

            // Draw white edge around square
            x--;
            y--;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if ((x + i >= 0) && (y + j >= 0) && (x + i < this.picture.GetLength(0))
                        && (y + j < this.picture.GetLength(1)))
                    {
                        if (this.picture[x + i, y + j] == QrPixel.Empty)
                        {
                            this.picture[x + i, y + j] = QrPixel.White;
                            this.staticData[x + i, y + j] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The fill data.
        /// </summary>
        /// <param name="bits">
        /// The bits.
        /// </param>
        private void FillData(Queue<bool> bits)
        {
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "------------------ Step 8 ------------------\n- Allocating " + (bits.Count / 8)
                    + " blocks into matrix ...\n");
            }

            // Starting point
            int x = this.picture.GetLength(0) - 1;
            int y = this.picture.GetLength(0) - 1;

            // Filling matrix until we havent got any bits left
            bool rightCol = true;
            bool goingUp = true;

            while (bits.Count > 0)
            {
                bool bit = bits.Dequeue();

                // Find an empty place to write: algorithm -> up-up-up-top-down-down-down-down-up-up-up
                while (this.picture[x, y] != QrPixel.Empty)
                {
                    // If we are on right side, we go left.
                    if (rightCol)
                    {
                        // If in on current right column, there is static data we should skip this column...
                        rightCol = false;

                        if (x != 0)
                        {
                            x--;
                        }
                    }
                    else
                    {
                        // If we are on left side, we go either up or down, depending on position
                        // Return to right column
                        x++;

                        rightCol = true;

                        if (goingUp)
                        {
                            y--;

                            // We have reached the top of the matrix, jump down, we have to move y left by 2 pixels..
                            if (y < 0)
                            {
                                y = 0;

                                x = x - 2;

                                goingUp = false;
                            }
                        }
                        else
                        {
                            y++;

                            // We have reached the bottom of the matrix, reverse
                            if (y >= this.picture.GetLength(0))
                            {
                                y = this.picture.GetLength(0) - 1;

                                x = x - 2;

                                goingUp = true;
                            }
                        }

                        // We shall skip the 6th vertical line
                        if (x == 6)
                        {
                            x--;
                        }
                    }
                }

                // Write bit
                this.picture[x, y] = bit ? QrPixel.Black : QrPixel.White;
            }

            // Matrix must be entirely full after this point.. Remainder bits for all empty spaces..
            for (int i = 0; i < this.picture.GetLength(0); i++)
            {
                for (int j = 0; j < this.picture.GetLength(1); j++)
                {
                    if (this.picture[i, j] == QrPixel.Empty)
                    {
                        this.picture[i, j] = QrPixel.White;
                    }
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Blocks successfully written.\n\n");
            }
        }

        /// <summary>
        /// The fill defaults.
        /// </summary>
        private void FillDefaults()
        {
            // Draw 3 position squares
            if (this.debugData != null)
            {
                this.debugData.Append("- Inserting position squares ...\n");
            }

            this.DrawSquare(0, 0);

            if (this.debugData != null)
            {
                this.debugData.Append("- Position square: top-left: (0, 0).\n");
            }

            this.DrawSquare(0, this.picture.GetLength(0) - 7);

            if (this.debugData != null)
            {
                this.debugData.Append("- Position square: top-right: (0, " + (this.picture.GetLength(0) - 7) + ").\n");
            }

            this.DrawSquare(this.picture.GetLength(0) - 7, 0);

            if (this.debugData != null)
            {
                this.debugData.Append(
                    "- Position square: bottom-left: (" + (this.picture.GetLength(0) - 7) + ", 0).\n");
            }

            // Draw reserved data
            this.DrawReserved();

            // Draw alignment positions, based on version
            if (this.version.Version > 1)
            {
                if (this.debugData != null)
                {
                    this.debugData.Append("- Version is higher than 1, placing alignment patterns ...\n");
                }

                // Drawing patterns
                byte[] pattern = QrAlignment.GetAlignment(this.version.Version);

                int row = 0;
                int col = 0;

                while (pattern[row] > 0)
                {
                    while (pattern[col] > 0)
                    {
                        if (this.debugData != null)
                        {
                            this.debugData.Append(
                                "- Placing alignment square at: (" + pattern[row] + ", " + pattern[col] + ").\n");
                        }

                        this.DrawAlignment(pattern[row], pattern[col]);

                        col++;

                        if (col >= pattern.Length)
                        {
                            break;
                        }
                    }

                    row++;
                    col = 0;

                    if (row >= pattern.Length)
                    {
                        break;
                    }
                }

                if (this.debugData != null)
                {
                    this.debugData.Append("- Alignment placing complete.\n");
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("\n");
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            // Create picture data of correct size
            if (this.debugData != null)
            {
                this.debugData.Append(
                    "----------------- Preprocess -----------------\n- Creating QR Code structure ...\n");
            }

            this.picture = new QrPixel[this.version.MatrixSize, this.version.MatrixSize];
            this.staticData = new bool[this.version.MatrixSize, this.version.MatrixSize];

            for (int i = 0; i < this.picture.GetLength(0); i++)
            {
                for (int j = 0; j < this.picture.GetLength(1); j++)
                {
                    this.picture[i, j] = QrPixel.Empty;
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Empty matrix of size " + this.version.MatrixSize + " created.\n");
            }

            this.FillDefaults();
        }

        /// <summary>
        /// Method writes format to picture.
        /// </summary>
        /// <param name="error">
        /// The error code information.
        /// </param>
        /// <param name="codeMask">
        /// The mask mask information.
        /// </param>
        private void WriteFormat(QrError error, QrMask codeMask)
        {
            QrFormat format;

            if (this.debugData != null)
            {
                this.debugData.Append("------------------ Step 10 ------------------\n- Writing format ...\n");

                format = new QrFormat(error, codeMask, true);

                this.debugData.Append(format.GetDebugData());
            }
            else
            {
                format = new QrFormat(error, codeMask);
            }

            List<bool> bits = new List<bool>(format.GetFormatBits().ToArray());

            // Write format vertically
            int y = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (this.picture[8, y] == QrPixel.Format)
                {
                    this.picture[8, y] = bits[i] ? QrPixel.Black : QrPixel.White;
                }
                else
                {
                    y++;
                    i--;
                }
            }

            // Write format horizontally
            int x = this.picture.GetLength(0) - 1;

            for (int i = 0; i < bits.Count; i++)
            {
                if (this.picture[x, 8] == QrPixel.Format)
                {
                    this.picture[x, 8] = bits[i] ? QrPixel.Black : QrPixel.White;
                }
                else
                {
                    x--;
                    i--;
                }
            }

            if (this.debugData != null)
            {
                this.debugData.Append("- Format successfully written.\n\n");
            }
        }

        /// <summary>
        /// The write version.
        /// </summary>
        /// <param name="codeVersion">
        /// The version.
        /// </param>
        private void WriteVersion(QrVersion codeVersion)
        {
            if (this.debugData != null)
            {
                this.debugData.Append("- Should we write version information bits?\n");
            }

            // Only writing version with versions higher than 6
            if (codeVersion.Version >= 7)
            {
                // Calculate BCH (18, 6) for version information
                int bchVersion = QrBoseChaudhuriHocquenghem.CalculateBch(codeVersion.Version, QrConst.GenPolyVersion);

                int versionInformation = (codeVersion.Version << 12) | bchVersion;

                List<bool> versionInfo = BinaryExtensions.DecToBin(versionInformation, 18);

                versionInfo.Reverse();

                if (this.debugData != null)
                {
                    this.debugData.Append(
                        "- Yes, writing information: " + BinaryExtensions.BinaryToString(versionInfo) + ".\n");
                }

                int iterator = 0;

                // Version bits at upper right corner
                for (int y = 0; y < 6; y++)
                {
                    for (int x = this.picture.GetLength(0) - 8 - 3, j = 0; j < 3; x++, j++)
                    {
                        this.picture[x, y] = versionInfo[iterator] ? QrPixel.Black : QrPixel.White;

                        iterator++;
                    }
                }

                iterator = 0;

                // Version bits at lower left corner
                for (int x = 0; x < 6; x++)
                {
                    for (int y = this.picture.GetLength(0) - 8 - 3, j = 0; j < 3; y++, j++)
                    {
                        this.picture[x, y] = versionInfo[iterator] ? QrPixel.Black : QrPixel.White;

                        iterator++;
                    }
                }

                if (this.debugData != null)
                {
                    this.debugData.Append("- Version information bits successfully written.\n");
                }
            }

            if ((this.debugData != null) && (codeVersion.Version < 7))
            {
                this.debugData.Append("- No, only required for versions higher than 6.\n");
            }
        }

        #endregion
    }
}