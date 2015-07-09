// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrAlignment.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code alignment data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    using System.Collections.Generic;

    using NLog;

    /// <summary>
    /// The <c>QR</c> code alignment data.
    /// </summary>
    public static class QrAlignment
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The alignment positions.
        /// </summary>
        private static byte[,] alignmentPos = new byte[,]
                                                   {
                                                       {
                                                          6, 0, 0, 0, 0, 0, 0 
                                                       }, {
                                                             6, 18, 0, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 22, 0, 0, 0, 0, 0 
                                                       }, {
                                                             6, 26, 0, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 30, 0, 0, 0, 0, 0 
                                                       }, {
                                                             6, 34, 0, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 22, 38, 0, 0, 0, 0 
                                                       }, {
                                                             6, 24, 42, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 26, 46, 0, 0, 0, 0 
                                                       }, {
                                                             6, 28, 50, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 30, 54, 0, 0, 0, 0 
                                                       }, {
                                                             6, 32, 58, 0, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 34, 62, 0, 0, 0, 0 
                                                       }, {
                                                             6, 26, 46, 66, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 26, 48, 70, 0, 0, 0 
                                                       }, {
                                                             6, 26, 50, 74, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 30, 54, 78, 0, 0, 0 
                                                       }, {
                                                             6, 30, 56, 82, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 30, 68, 86, 0, 0, 0 
                                                       }, {
                                                             6, 34, 62, 90, 0, 0, 0 
                                                          }, 
                                                       {
                                                          6, 28, 50, 72, 94, 0, 0 
                                                       }, {
                                                             6, 26, 50, 74, 98, 0, 0 
                                                          }, 
                                                       {
                                                          6, 30, 54, 78, 102, 0, 0 
                                                       }, {
                                                             6, 28, 54, 80, 106, 0, 0 
                                                          }, 
                                                       {
                                                          6, 32, 58, 84, 110, 0, 0 
                                                       }, {
                                                             6, 30, 58, 86, 114, 0, 0 
                                                          }, 
                                                       {
                                                          6, 34, 62, 90, 118, 0, 0 
                                                       }, {
                                                             6, 26, 50, 74, 98, 122, 0 
                                                          }, 
                                                       {
                                                          6, 30, 54, 78, 102, 126, 0 
                                                       }, {
                                                             6, 26, 52, 78, 104, 130, 0 
                                                          }, 
                                                       {
                                                          6, 30, 56, 82, 108, 134, 0 
                                                       }, {
                                                             6, 34, 60, 86, 112, 138, 0 
                                                          }, 
                                                       {
                                                          6, 30, 58, 86, 114, 142, 0 
                                                       }, {
                                                             6, 34, 62, 90, 118, 146, 0 
                                                          }, 
                                                       {
                                                          6, 30, 54, 78, 102, 126, 150 
                                                       }, {
                                                             6, 24, 50, 76, 102, 128, 154 
                                                          }, 
                                                       {
                                                          6, 28, 54, 80, 106, 132, 158 
                                                       }, {
                                                             6, 32, 58, 84, 110, 136, 162 
                                                          }, 
                                                       {
                                                          6, 26, 54, 82, 110, 138, 166 
                                                       }, {
                                                             6, 30, 58, 86, 114, 142, 170 
                                                          }
                                                   };

        #endregion

        // Experimental, not working!!!!
        #region Public Methods and Operators

        /// <summary>
        /// The calculate alignments.
        /// </summary>
        public static void CalculateAlignments()
        {
            alignmentPos = new byte[40, 7];

            int startSize = 45;

            int alignCount = 2;
            int counter = 0;

            for (int i = 6; i < 40; i++)
            {
                counter++;

                // First and second alignments are usually on same position
                alignmentPos[i, 0] = 6;
                alignmentPos[i, alignCount] = (byte)(startSize - 7);

                // Calculate rows and columns of middle alignments
                int divider = startSize - 6 - 7;
                divider = divider / alignCount;

                for (int x = 1; x < alignCount; x++)
                {
                    alignmentPos[i, x] = (byte)(6 + (divider * x));

                    if (alignmentPos[i, x] % 2 == 1)
                    {
                        alignmentPos[i, x]--;
                    }
                }

                if (counter == 7)
                {
                    alignCount++;
                    counter = 0;
                }

                startSize += 4;
            }
        }

        /// <summary>
        /// The get alignment.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] GetAlignment(int version)
        {
            List<byte> positions = new List<byte>();

            for (int i = 0; i < 7; i++)
            {
                positions.Add(alignmentPos[version - 1, i]);
            }

            return positions.ToArray();
        }

        #endregion

        /*
        1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15
        21, 25, 29, 33, 37, 41, 45, 49, 53, 57, 61, 65, 69, 73, 77
        0,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,  3,  3
        8,  12, 16, 20, 24, 28, 32, 36, 40, 44, 48, 52, 56, 60, 64
         */
    }
}