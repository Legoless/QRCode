using System.Collections.Generic;
using System.Text;
using QRCode.Data;
using QRCode.ErrorCorrection;
using QRCode.Extensions;

namespace QRCode.Library
{
    public class QRFormat
    {
        private readonly StringBuilder _debugData;
        private readonly Queue<bool> _output = new Queue<bool> ();

        public QRFormat (QRError error, QRMask mask)
        {
            CalculateFormat (error, mask);
        }

        public QRFormat (QRError error, QRMask mask, bool debug)
        {
            if (debug)
            {
                _debugData = new StringBuilder ();
            }

            CalculateFormat (error, mask);
        }

        public string GetDebugData ()
        {
            if (_debugData != null)
            {
                return _debugData.ToString ();
            }

            return "";
        }

        private void CalculateFormat (QRError error, QRMask mask)
        {
            //
            // Get error bits
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Calculating format bits...\n");
            }

            //
            // Get 5 bits of format information
            //

            int formatInfo = (QRErrorBits.GetBits (error) << 3) | BinaryExtensions.BinToDec (mask.GetBits ());

            if (_debugData != null)
            {
                _debugData.Append ("- Format information error and mask bits: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (formatInfo)) + "\n");
                _debugData.Append ("- Calculating BCH (15,5) code for information bits...\n");
            }

            int bchCode = QRBoseChaudhuriHocquenghem.CalculateBch (formatInfo, QRConst.GenPolyFormat);

            if (_debugData != null)
            {
                _debugData.Append ("- BCH (15,5) error code: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (bchCode)) + "\n");
            }

            // Append BCH code to format info

            formatInfo = (formatInfo << 10) | bchCode;

            if (_debugData != null)
            {
                _debugData.Append ("- Format information with appended BCH: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (formatInfo)) + "\n");
                _debugData.Append ("- Calculating XOR with format constant: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (QRConst.ConstFormatXor)) + "\n");
            }

            formatInfo = formatInfo ^ QRConst.ConstFormatXor;

            if (_debugData != null)
            {
                _debugData.Append ("- Final format: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (formatInfo, 15)) + "\n");
            }

            List<bool> formatBits = BinaryExtensions.DecToBin (formatInfo, 15);

            // Reverse, because our QRCode is inserting backwards
            formatBits.Reverse ();

            foreach (bool t in formatBits)
            {
                _output.Enqueue (t);
            }
        }

        public Queue<bool> GetFormatBits ()
        {
            return _output;
        }
    }
}