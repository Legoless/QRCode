using System.Collections.Generic;
using System.Drawing;
using System.Text;
using QRCode.Data;
using QRCode.ErrorCorrection;
using QRCode.Extensions;

namespace QRCode.Library
{
    internal class QRImage
    {
        //
        // Used for method interchange
        //

        private readonly QRBreakPoint _breakPoint;
        private readonly StringBuilder _debugData;
        private readonly QRMask _mask;
        private readonly QRVersion _version;
        private QRPixel[,] _picture;
        private bool[,] _staticData;

        public QRImage (QRVersion version, QRMask mask)
        {
            _version = version;
            _mask = mask;

            Initialize ();
        }

        public QRImage (QRVersion version, QRMask mask, QRBreakPoint bp)
        {
            _debugData = new StringBuilder ();

            _breakPoint = bp;

            _version = version;
            _mask = mask;

            Initialize ();
        }

        private void Initialize ()
        {
            //
            // Create picture data of correct size
            //

            if (_debugData != null)
            {
                _debugData.Append ("----------------- Preprocess -----------------\n- Creating QR Code structure ...\n");
            }

            _picture = new QRPixel[_version.MatrixSize,_version.MatrixSize];
            _staticData = new bool[_version.MatrixSize,_version.MatrixSize];

            for (int i = 0; i < _picture.GetLength (0); i++)
            {
                for (int j = 0; j < _picture.GetLength (1); j++)
                {
                    _picture[i, j] = QRPixel.Empty;
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Empty matrix of size " + _version.MatrixSize + " created.\n");
            }

            FillDefaults ();
        }

        public void CreateImage (Queue<bool> bits)
        {
            //
            // Step 8 - put data into matrix
            //

            FillData (bits);

            if ((_breakPoint == QRBreakPoint.Filled) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 9 -------");

                return;
            }

            //
            // Step 9 - execute mask
            //

            DoMask (_mask);

            if ((_breakPoint == QRBreakPoint.Mask) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 10 -------");

                return;
            }

            //
            // Step 10 - write format and version
            //

            WriteFormat (_version.ErrorLevel, _mask);

            WriteVersion (_version);
        }

        public string GetDebugData ()
        {
            string str = _debugData.ToString ();

            _debugData.Clear ();

            return str;
        }

        private void WriteVersion (QRVersion version)
        {
            if (_debugData != null)
            {
                _debugData.Append ("- Should we write version information bits?\n");
            }

            // Only writing version with versions higher than 6
            if (version.Version >= 7)
            {
                //
                // Calculate BCH (18, 6) for version information
                //

                int bchVersion = QRBoseChaudhuriHocquenghem.CalculateBch (version.Version, QRConst.GenPolyVersion);

                int versionInformation = (version.Version << 12) | bchVersion;

                List<bool> versionInfo = BinaryExtensions.DecToBin (versionInformation, 18);

                versionInfo.Reverse ();

                if (_debugData != null)
                {
                    _debugData.Append ("- Yes, writing information: " + BinaryExtensions.BinaryToString (versionInfo) + ".\n");
                }

                int iterator = 0;

                // Version bits at upper right corner
                for (int y = 0; y < 6; y++)
                {
                    for (int x = _picture.GetLength (0) - 8 - 3, j = 0; j < 3; x++, j++)
                    {
                        _picture[x, y] = versionInfo[iterator] ? QRPixel.Black : QRPixel.White;

                        iterator++;
                    }
                }

                iterator = 0;

                // Version bits at lower left corner
                for (int x = 0; x < 6; x++)
                {
                    for (int y = _picture.GetLength (0) - 8 - 3, j = 0; j < 3; y++, j++)
                    {
                        _picture[x, y] = versionInfo[iterator] ? QRPixel.Black : QRPixel.White;

                        iterator++;
                    }
                }

                if (_debugData != null)
                {
                    _debugData.Append ("- Version information bits successfully written.\n");
                }
            }

            if ((_debugData != null) && (version.Version < 7))
            {
                _debugData.Append ("- No, only required for versions higher than 6.\n");
            }
        }

        private void WriteFormat (QRError error, QRMask mask)
        {
            QRFormat format;

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 10 ------------------\n- Writing format ...\n");

                format = new QRFormat (error, mask, true);

                _debugData.Append (format.GetDebugData ());
            }
            else
            {
                format = new QRFormat (error, mask);
            }

            List<bool> bits = new List<bool> (format.GetFormatBits ().ToArray ());

            //
            // Write format vertically
            //

            int y = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (_picture[8, y] == QRPixel.Format)
                {
                    _picture[8, y] = (bits[i]) ? QRPixel.Black : QRPixel.White;
                }
                else
                {
                    y++;
                    i--;
                }
            }

            //
            // Write format horizontally
            //

            int x = _picture.GetLength (0) - 1;

            for (int i = 0; i < bits.Count; i++)
            {
                if (_picture[x, 8] == QRPixel.Format)
                {
                    _picture[x, 8] = (bits[i]) ? QRPixel.Black : QRPixel.White;
                }
                else
                {
                    x--;
                    i--;
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Format successfully written.\n\n");
            }
        }

        public Bitmap Render (int resolution)
        {
            Bitmap img = new Bitmap (_picture.GetLength (0) * resolution, _picture.GetLength (1) * resolution);

            for (int i = 0; i < _picture.GetLength (0) * resolution; i++)
            {
                for (int j = 0; j < _picture.GetLength (1) * resolution; j++)
                {
                    Color pixelColor = Color.LightGray;

                    switch (_picture[i / resolution, j / resolution])
                    {
                        case QRPixel.Black:
                            pixelColor = Color.Black;
                            break;
                        case QRPixel.White:
                            pixelColor = Color.White;
                            break;
                        case QRPixel.Format:
                            pixelColor = Color.Blue;
                            break;
                        case QRPixel.Reserved:
                            pixelColor = Color.Chocolate;
                            break;
                        case QRPixel.Version:
                            pixelColor = Color.Green;
                            break;
                        case QRPixel.Mask:
                            pixelColor = Color.Red;
                            break;
                    }

                    img.SetPixel (i, j, pixelColor);
                }
            }

            return img;
        }

        public Bitmap GetImage ()
        {
            return Render (1);
        }

        private void DoMask (QRMask mask)
        {
            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 9 ------------------\n- Executing mask on data pixels: " + BinaryExtensions.BinaryToString (mask.GetBits ()) + "\n");
            }

            int counter = 0;

            for (int i = 0; i < _picture.GetLength (0); i++)
            {
                for (int j = 0; j < _picture.GetLength (1); j++)
                {
                    int x = _picture.GetLength (0) - i - 1;
                    int y = _picture.GetLength (1) - j - 1;

                    if (mask.Test (y, x) && (_staticData[x, y] == false) && (_picture[x, y] != QRPixel.Empty))
                    {
                        _picture[x, y] = (_picture[x, y] == QRPixel.White) ? QRPixel.Black : QRPixel.White;

                        counter++;

                        if ((_breakPoint == QRBreakPoint.Mask) && (_debugData != null))
                        {
                            _picture[x, y] = QRPixel.Mask;
                        }
                    }
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Mask (" + mask + ") written to: " + counter + " pixels.\n\n");
            }
        }

        private void FillData (Queue<bool> bits)
        {
            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 8 ------------------\n- Allocating " + (bits.Count / 8) + " blocks into matrix ...\n");
            }

            // Starting point
            int x = _picture.GetLength (0) - 1;
            int y = _picture.GetLength (0) - 1;

            // Filling matrix until we havent got any bits left
            bool rightCol = true;
            bool goingUp = true;

            while (bits.Count > 0)
            {
                bool bit = bits.Dequeue ();

                //
                // Find an empty place to write: algorithm -> up-up-up-top-down-down-down-down-up-up-up
                //

                while (_picture[x, y] != QRPixel.Empty)
                {
                    //
                    // If we are on right side, we go left.
                    //

                    if (rightCol)
                    {
                        //
                        // If in on current right column, there is static data we should skip this column...
                        //

                        rightCol = false;

                        if (x != 0)
                        {
                            x--;
                        }
                    }

                        //
                        // If we are on left side, we go either up or down, depending on position
                        //

                    else
                    {
                        // Return to right column
                        x++;

                        rightCol = true;

                        if (goingUp)
                        {
                            y--;

                            //
                            // We have reached the top of the matrix, jump down, we have to move y left by 2 pixels..
                            //

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

                            //
                            // We have reached the bottom of the matrix, reverse
                            //

                            if (y >= _picture.GetLength (0))
                            {
                                y = _picture.GetLength (0) - 1;

                                x = x - 2;

                                goingUp = true;
                            }
                        }

                        //
                        // We shall skip the 6th vertical line
                        //

                        if (x == 6)
                        {
                            x--;
                        }
                    }
                }

                // Write bit
                _picture[x, y] = (bit) ? QRPixel.Black : QRPixel.White;
            }

            //
            // Matrix must be entirely full after this point.. Remainder bits for all empty spaces..
            //

            for (int i = 0; i < _picture.GetLength (0); i++)
            {
                for (int j = 0; j < _picture.GetLength (1); j++)
                {
                    if (_picture[i, j] == QRPixel.Empty)
                    {
                        _picture[i, j] = QRPixel.White;
                    }
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Blocks successfully written.\n\n");
            }
        }

        private void FillDefaults ()
        {
            //
            // Draw 3 position squares
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Inserting position squares ...\n");
            }

            DrawSquare (0, 0);

            if (_debugData != null)
            {
                _debugData.Append ("- Position square: top-left: (0, 0).\n");
            }

            DrawSquare (0, _picture.GetLength (0) - 7);

            if (_debugData != null)
            {
                _debugData.Append ("- Position square: top-right: (0, " + (_picture.GetLength (0) - 7) + ").\n");
            }

            DrawSquare (_picture.GetLength (0) - 7, 0);

            if (_debugData != null)
            {
                _debugData.Append ("- Position square: bottom-left: (" + (_picture.GetLength (0) - 7) + ", 0).\n");
            }

            //
            // Draw reserved data
            //

            DrawReserved ();

            //
            // Draw alignment positions, based on version
            //

            if (_version.Version > 1)
            {
                if (_debugData != null)
                {
                    _debugData.Append ("- Version is higher than 1, placing alignment patterns ...\n");
                }

                // Drawing patterns
                byte[] pattern = QRAlignment.GetAlignment (_version.Version);

                int row = 0;
                int col = 0;

                while (pattern[row] > 0)
                {
                    while (pattern[col] > 0)
                    {
                        if (_debugData != null)
                        {
                            _debugData.Append ("- Placing alignment square at: (" + pattern[row] + ", " + pattern[col] + ").\n");
                        }

                        DrawAlignment (pattern[row], pattern[col]);

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

                if (_debugData != null)
                {
                    _debugData.Append ("- Alignment placing complete.\n");
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("\n");
            }
        }

        private void DrawReserved ()
        {
            //
            // Drawing black & white dotted lines to right and down corner
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Drawing timing patterns ...\n");
            }

            int i = 8;

            while (true)
            {
                if (_picture[i, 6] != QRPixel.Empty)
                {
                    break;
                }
                _picture[i, 6] = (i % 2 == 0) ? QRPixel.Black : QRPixel.White;
                _picture[6, i] = (i % 2 == 0) ? QRPixel.Black : QRPixel.White;

                _staticData[i, 6] = true;
                _staticData[6, i] = true;

                i++;
            }

            //
            // There is a black pixel at bottom left square
            //

            _picture[8, _picture.GetLength (0) - 8] = QRPixel.Black;
            _staticData[8, _picture.GetLength (0) - 8] = true;

            //
            // Format information is in blue pixels and is stored around squares
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Securing format information bits.\n");
            }

            // Top-Left
            for (int x = 0; x < 9; x++)
            {
                if (_picture[8, x] == QRPixel.Empty)
                {
                    _picture[8, x] = QRPixel.Format;
                    _staticData[8, x] = true;
                }
            }

            for (int x = 0; x < 9; x++)
            {
                if (_picture[x, 8] == QRPixel.Empty)
                {
                    _picture[x, 8] = QRPixel.Format;
                    _staticData[x, 8] = true;
                }
            }

            // Top-Right
            for (int x = 0; x < 8; x++)
            {
                if (_picture[_picture.GetLength (0) - x - 1, 8] == QRPixel.Empty)
                {
                    _picture[_picture.GetLength (0) - x - 1, 8] = QRPixel.Format;
                    _staticData[_picture.GetLength (0) - x - 1, 8] = true;
                }
            }

            // Bottom-Left
            for (int x = 0; x < 8; x++)
            {
                if (_picture[8, _picture.GetLength (0) - x - 1] == QRPixel.Empty)
                {
                    _picture[8, _picture.GetLength (0) - x - 1] = QRPixel.Format;
                    _staticData[8, _picture.GetLength (0) - x - 1] = true;
                }
            }

            //
            // Version information is in green pixels, 2 blocks of 18 pixels (3 x 6)
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Securing version information bits.\n");
            }

            if (_version.Version >= 7)
            {
                // Version bits at upper right corner
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        _picture[_picture.GetLength (0) - 9 - x, y] = QRPixel.Version;
                        _staticData[_picture.GetLength (0) - 9 - x, y] = true;
                    }
                }

                // Version bits at lower left corner
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 6; y++)
                    {
                        _picture[y, _picture.GetLength (0) - 9 - x] = QRPixel.Version;
                        _staticData[y, _picture.GetLength (0) - 9 - x] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draw smaller alignment square at desired position
        /// </summary>
        /// <param name="x">x position of the point</param>
        /// <param name="y">y position of the point</param>
        private void DrawAlignment (int x, int y)
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
                    if (_picture[x + i, y + j] != QRPixel.Empty)
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
                        _picture[x + i, y + j] = QRPixel.Black;
                        _staticData[x + i, y + j] = true;
                    }
                    else
                    {
                        _picture[x + i, y + j] = QRPixel.White;
                        _staticData[x + i, y + j] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Draws QR square at selected position
        /// </summary>
        /// <param name="x">x coordinate of desired square position</param>
        /// <param name="y">y coordinate of desired square position</param>
        private void DrawSquare (int x, int y)
        {
            // Draw square
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if ((i == 0) || (j == 0) || (i == 6) || (j == 6) || ((i > 1 && i < 5) && (j > 1 && j < 5)))
                    {
                        _picture[x + i, y + j] = QRPixel.Black;
                        _staticData[x + i, y + j] = true;
                    }
                    else
                    {
                        _picture[x + i, y + j] = QRPixel.White;
                        _staticData[x + i, y + j] = true;
                    }
                }
            }

            //
            // Draw white edge around square
            //

            x--;
            y--;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if ((x + i >= 0) && (y + j >= 0) && (x + i < _picture.GetLength (0)) && (y + j < _picture.GetLength (1)))
                    {
                        if (_picture[x + i, y + j] == QRPixel.Empty)
                        {
                            _picture[x + i, y + j] = QRPixel.White;
                            _staticData[x + i, y + j] = true;
                        }
                    }
                }
            }
        }
    }
}