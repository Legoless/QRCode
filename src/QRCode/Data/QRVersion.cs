using System;

namespace QRCode.Data
{
    public class QRVersion
    {
        //
        // How could you calculate this dynamically?
        // L, M, Q, H -> block 1 sizes
        //

        private static readonly byte[] CodeBlocks = new byte[]
                                                        {
                                                            1, 19, 0, 0,
                                                            1, 16, 0, 0,
                                                            1, 13, 0, 0,
                                                            1, 9, 0, 0,
                                                            1, 34, 0, 0,
                                                            1, 28, 0, 0,
                                                            1, 22, 0, 0,
                                                            1, 16, 0, 0,
                                                            1, 55, 0, 0,
                                                            1, 44, 0, 0,
                                                            2, 17, 0, 0,
                                                            2, 13, 0, 0,
                                                            1, 80, 0, 0,
                                                            2, 32, 0, 0,
                                                            2, 24, 0, 0,
                                                            4, 9, 0, 0,
                                                            1, 108, 0, 0,
                                                            2, 43, 0, 0,
                                                            2, 15, 2, 16,
                                                            2, 11, 2, 12,
                                                            2, 68, 0, 0,
                                                            4, 27, 0, 0,
                                                            4, 19, 0, 0,
                                                            4, 15, 0, 0,
                                                            2, 78, 0, 0,
                                                            4, 31, 0, 0,
                                                            2, 14, 4, 15,
                                                            4, 13, 1, 14,
                                                            2, 97, 0, 0,
                                                            2, 38, 2, 39,
                                                            4, 18, 2, 19,
                                                            4, 14, 2, 15,
                                                            2, 116, 0, 0,
                                                            3, 36, 2, 37,
                                                            4, 16, 4, 17,
                                                            4, 12, 4, 13,
                                                            2, 68, 2, 69,
                                                            4, 43, 1, 44,
                                                            6, 19, 2, 20,
                                                            6, 15, 2, 16,
                                                            4, 81, 0, 0,
                                                            1, 50, 4, 51,
                                                            4, 22, 4, 23,
                                                            3, 12, 8, 13,
                                                            2, 92, 2, 93,
                                                            6, 36, 2, 37,
                                                            4, 20, 6, 21,
                                                            7, 14, 4, 15,
                                                            4, 107, 0, 0,
                                                            8, 37, 1, 38,
                                                            8, 20, 4, 21,
                                                            12, 11, 4, 12,
                                                            3, 115, 1, 116,
                                                            4, 40, 5, 41,
                                                            11, 16, 5, 17,
                                                            11, 12, 5, 13,
                                                            5, 87, 1, 88,
                                                            5, 41, 5, 42,
                                                            5, 24, 7, 25,
                                                            11, 12, 7, 13,
                                                            5, 98, 1, 99,
                                                            7, 45, 3, 46,
                                                            15, 19, 2, 20,
                                                            3, 15, 13, 16,
                                                            1, 107, 5, 108,
                                                            10, 46, 1, 47,
                                                            1, 22, 15, 23,
                                                            2, 14, 17, 15,
                                                            5, 120, 1, 121,
                                                            9, 43, 4, 44,
                                                            17, 22, 1, 23,
                                                            2, 14, 19, 15,
                                                            3, 113, 4, 114,
                                                            3, 44, 11, 45,
                                                            17, 21, 4, 22,
                                                            9, 13, 16, 14,
                                                            3, 107, 5, 108,
                                                            3, 41, 13, 42,
                                                            15, 24, 5, 25,
                                                            15, 15, 10, 16,
                                                            4, 116, 4, 117,
                                                            17, 42, 0, 0,
                                                            17, 22, 6, 23,
                                                            19, 16, 6, 17,
                                                            2, 111, 7, 112,
                                                            17, 46, 0, 0,
                                                            7, 24, 16, 25,
                                                            34, 13, 0, 0,
                                                            4, 121, 5, 122,
                                                            4, 47, 14, 48,
                                                            11, 24, 14, 25,
                                                            16, 15, 14, 16,
                                                            6, 117, 4, 118,
                                                            6, 45, 14, 46,
                                                            11, 24, 16, 25,
                                                            30, 16, 2, 17,
                                                            8, 106, 4, 107,
                                                            8, 47, 13, 48,
                                                            7, 24, 22, 25,
                                                            22, 15, 13, 16,
                                                            10, 114, 2, 115,
                                                            19, 46, 4, 47,
                                                            28, 22, 6, 23,
                                                            33, 16, 4, 17,
                                                            8, 122, 4, 123,
                                                            22, 45, 3, 46,
                                                            8, 23, 26, 24,
                                                            12, 15, 28, 16,
                                                            3, 117, 10, 118,
                                                            3, 45, 23, 46,
                                                            4, 24, 31, 25,
                                                            11, 15, 31, 16,
                                                            7, 116, 7, 117,
                                                            21, 45, 7, 46,
                                                            1, 23, 37, 24,
                                                            19, 15, 26, 16,
                                                            5, 115, 10, 116,
                                                            19, 47, 10, 48,
                                                            15, 24, 25, 25,
                                                            23, 15, 25, 16,
                                                            13, 115, 3, 116,
                                                            2, 46, 29, 47,
                                                            42, 24, 1, 25,
                                                            23, 15, 28, 16,
                                                            17, 115, 0, 0,
                                                            10, 46, 23, 47,
                                                            10, 24, 35, 25,
                                                            19, 15, 35, 16,
                                                            17, 115, 1, 116,
                                                            14, 46, 21, 47,
                                                            29, 24, 19, 25,
                                                            11, 15, 46, 16,
                                                            13, 115, 6, 116,
                                                            14, 46, 23, 47,
                                                            44, 24, 7, 25,
                                                            59, 16, 1, 17,
                                                            12, 121, 7, 122,
                                                            12, 47, 26, 48,
                                                            39, 24, 14, 25,
                                                            22, 15, 41, 16,
                                                            6, 121, 14, 122,
                                                            6, 47, 34, 48,
                                                            46, 24, 10, 25,
                                                            2, 15, 64, 16,
                                                            17, 122, 4, 123,
                                                            29, 46, 14, 47,
                                                            49, 24, 10, 25,
                                                            24, 15, 46, 16,
                                                            4, 122, 18, 123,
                                                            13, 46, 32, 47,
                                                            48, 24, 14, 25,
                                                            42, 15, 32, 16,
                                                            20, 117, 4, 118,
                                                            40, 47, 7, 48,
                                                            43, 24, 22, 25,
                                                            10, 15, 67, 16,
                                                            19, 118, 6, 119,
                                                            18, 47, 31, 48,
                                                            34, 24, 34, 25,
                                                            20, 15, 61, 16
                                                        };

        public QRVersion (int ver, QRError error)
        {
            //
            // Optimization: Dynamically calculate QR Code version
            //

            Version = ver + 1;
            ErrorLevel = error;

            // Get maximum matrix size
            MatrixSize = Convert.ToByte (21 + (4 * ver));

            //
            // Calculate data and error sizes based on error level
            //

            // Position squares with separators are each 8x8 pixels, we have 4 of them
            int maxBytes = (MatrixSize * MatrixSize) - (8 * 8 * 3);
            // Format bits are 2 times of 15 bits
            maxBytes -= (2 * 15);
            // There is 1 dark pixel in bottom format information.
            maxBytes--;

            //
            // Through alignment patterns we cannot place bits
            //

            // Calculate number of alignment patters
            byte[] alignments = QRAlignment.GetAlignment (Version);

            int alignmentCount = 7;

            while (alignments[alignmentCount - 1] == 0)
            {
                alignmentCount--;
            }

            // Number of alignments, minus three, since three are static position patterns
            int alignmentNum = (alignmentCount * alignmentCount) - 3;

            // Each alignment takes 5x5 pixels
            if (alignmentNum > 0)
            {
                maxBytes -= (5 * 5) * alignmentNum;
            }

            // Versions higher than 7 have version information added
            if (Version >= 7)
            {
                maxBytes -= (2 * 18);
            }

            // Still need to remove the timing patterns...
            // - We already removed the bits in alignments, so not removing them twice

            if (alignmentCount == 1)
            {
                alignmentCount = 2;
            }

            maxBytes -= (MatrixSize - (2 * 8) - ((alignmentCount - 2) * 5)) * 2;

            // To get actual bytes
            maxBytes = maxBytes / 8;

            //
            // Block sizes and counts
            //

            BlockOneSize = CodeBlocks[(ver * 16) + (((int) error) * 4) + 1];
            BlockTwoSize = CodeBlocks[(ver * 16) + (((int) error) * 4) + 3];
            BlockOneCount = CodeBlocks[(ver * 16) + (((int) error) * 4) + 0];
            BlockTwoCount = CodeBlocks[(ver * 16) + (((int) error) * 4) + 2];

            // Amount of data size
            DataSize = ((BlockOneSize * BlockOneCount) + (BlockTwoSize * BlockTwoCount));
            // Error size
            ErrorSize = (maxBytes - DataSize);
        }

        public int Version
        {
            get;
            set;
        }

        public QRError ErrorLevel
        {
            get;
            set;
        }

        public byte MatrixSize
        {
            get;
            set;
        }

        public int DataSize
        {
            get;
            set;
        }

        public int ErrorSize
        {
            get;
            set;
        }

        public short BlockOneSize
        {
            get;
            set;
        }

        public short BlockTwoSize
        {
            get;
            set;
        }

        public byte BlockOneCount
        {
            get;
            set;
        }

        public byte BlockTwoCount
        {
            get;
            set;
        }

        public short GetBlockCount ()
        {
            return (short) (BlockOneCount + BlockTwoCount);
        }
    }
}