using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QRCode.Data;
using QRCode.ErrorCorrection;
using QRCode.Extensions;

namespace QRCode.Library
{
    public class QRCodeCreator
    {
        //
        // QRCode variables
        //

        //
        // Debug
        //

        private readonly Queue<bool> _output = new Queue<bool> ();
        private readonly string _text;
        private QRBreakPoint _breakPoint;
        private StringBuilder _debugData;

        //
        // Class variables
        //

        private QRImage _img;
        private QRMask _mask;
        private QRType _type;
        private QRVersion _version;

        //
        // Constructor only makes
        //

        public QRCodeCreator (string t)
        {
            _text = t;
        }

        //
        // Initializers
        //

        public void SetDebugMode (QRBreakPoint bp)
        {
            _breakPoint = bp;
            _debugData = new StringBuilder ();
        }

        public void SetAdvancedMode (QRType t, QRVersion v, QRMask m)
        {
            _type = t;
            _version = v;
            _mask = m;
        }

        public string GetDebugData ()
        {
            if (_debugData != null)
            {
                return _debugData.ToString ();
            }
            return "";
        }

        private void ChooseVersions ()
        {
            // Automatic setting
            if ((_version == null) || (_mask == null))
            {
                _type = QRType.AlphaNumeric;
                _version = new QRVersion (1, QRError.Q);
                _mask = new QRMask (2);
            }
        }

        private void Process ()
        {
            //
            // Preprocess
            // -> one of the parameters was not set, simple mode
            // -> create structure of QR Image
            //

            ChooseVersions ();

            if (_debugData != null)
            {
                _img = new QRImage (_version, _mask, _breakPoint);

                _debugData.Append (_img.GetDebugData ());
            }
            else
            {
                _img = new QRImage (_version, _mask);
            }

            if ((_breakPoint == QRBreakPoint.CreatedMatrix) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 1 -------");

                return;
            }

            //
            // Step one - sending type to output
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 1 ------------------\n- Sending type bits (4) to output ...\n");
            }

            switch (_type)
            {
                case QRType.Numeric:
                    _output.Enqueue (false);
                    _output.Enqueue (false);
                    _output.Enqueue (false);
                    _output.Enqueue (true);

                    break;

                case QRType.AlphaNumeric:
                    _output.Enqueue (false);
                    _output.Enqueue (false);
                    _output.Enqueue (true);
                    _output.Enqueue (false);

                    break;

                case QRType.Binary:
                    _output.Enqueue (false);
                    _output.Enqueue (true);
                    _output.Enqueue (false);
                    _output.Enqueue (false);

                    break;
                case QRType.Japanese:
                    _output.Enqueue (true);
                    _output.Enqueue (false);
                    _output.Enqueue (false);
                    _output.Enqueue (false);

                    break;
                default:
                    throw new ArgumentException ("Wrong QRType selected.");
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Sent 4 bytes of output: " + BinaryExtensions.BinaryToString (_output) + "\n\n");
            }

            if ((_breakPoint == QRBreakPoint.OutputTypeBits) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 2 -------");

                return;
            }

            //
            // Step two - write length of string
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 2 ------------------\n- Calculating bit size for data length ...\n");
            }

            byte strLength;

            if ((_version.Version >= 1) && (_version.Version <= 9))
            {
                switch (_type)
                {
                    case QRType.Numeric:
                        strLength = 10;
                        break;
                    case QRType.AlphaNumeric:
                        strLength = 9;
                        break;
                    case QRType.Binary:
                        strLength = 8;
                        break;
                    case QRType.Japanese:
                        strLength = 8;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else if ((_version.Version >= 10) && (_version.Version <= 26))
            {
                switch (_type)
                {
                    case QRType.Numeric:
                        strLength = 12;
                        break;
                    case QRType.AlphaNumeric:
                        strLength = 11;
                        break;
                    case QRType.Binary:
                        strLength = 16;
                        break;
                    case QRType.Japanese:
                        strLength = 10;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else if ((_version.Version >= 27) && (_version.Version <= 40))
            {
                switch (_type)
                {
                    case QRType.Numeric:
                        strLength = 14;
                        break;
                    case QRType.AlphaNumeric:
                        strLength = 13;
                        break;
                    case QRType.Binary:
                        strLength = 16;
                        break;
                    case QRType.Japanese:
                        strLength = 12;
                        break;
                    default:
                        strLength = 0;
                        break;
                }
            }
            else
            {
                throw new ArgumentException ("Wrong QRVersion selected.");
            }

            if (_debugData != null)
            {
                _debugData.Append ("- QR Type: " + _type + ", QR Version: " + _version.Version + ", Bit size: " + strLength + " bits.\n");
            }

            // Write length to output

            int length = _text.Length;

            ToOutput (BinaryExtensions.DecToBin (length, strLength));

            if (_debugData != null)
            {
                _debugData.Append ("- Stored data length to output: " + BinaryExtensions.BinaryToString (BinaryExtensions.DecToBin (length, strLength)) + "\n");
                _debugData.Append ("- Data stored in output: " + BinaryExtensions.BinaryToString (_output) + "\n\n");
            }

            if ((_breakPoint == QRBreakPoint.OutputDataLength) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 3 -------");

                return;
            }

            //
            // Step 3 - encoding entry text
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 3 ------------------\n- Creating code table for input ...\n");
            }

            QRCodeTable table = new QRCodeTable (_type);

            if (_debugData != null)
            {
                _debugData.Append ("- Checking for odd input length.\n");
            }

            // Odd text length handling
            if (length % 2 == 1)
            {
                if (_debugData != null)
                {
                    _debugData.Append (" - Text length is odd, reducing size for one character.\n");
                }

                length = length - 1;
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Writing characters ...\n");
            }

            for (int i = 0; i < length; i += 2)
            {
                // Get number
                int calc = (table.GetCharCount () * table.GetCodeByChar (_text[i])) + table.GetCodeByChar (_text[i + 1]);

                if (_debugData != null)
                {
                    _debugData.Append (" - Characters: " + _text[i] + ", " + _text[i + 1] + ": (" + table.GetCharCount () + " * " + table.GetCodeByChar (_text[i]) + ") + " + table.GetCodeByChar (_text[i + 1]) + " = " + calc + "\n");
                }

                // Write number, why 11 bits for each sign?????
                // Experimental: strLength as calculated for size.
                ToOutput (BinaryExtensions.DecToBin (calc, 11));
            }

            // Handling last 6 bits for odd length
            if (_text.Length % 2 == 1)
            {
                if (_debugData != null)
                {
                    _debugData.Append ("- Writing last (odd) character " + _text[_text.Length - 1] + " with " + 6 + " bits: " + table.GetCodeByChar (_text[_text.Length - 1]) + "\n");
                }

                ToOutput (BinaryExtensions.DecToBin (table.GetCodeByChar (_text[_text.Length - 1]), 6));
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Input successfully encoded to binary.\n\n");
            }

            if ((_breakPoint == QRBreakPoint.EncodedDataToBinary) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 4 -------");

                return;
            }

            //
            // Step 4 - finishing binary string
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 4 ------------------\n- Finishing binary string ...\n");
            }

            // Calculate maximum bit length

            int maxBits = _version.DataSize * 8;

            if (_debugData != null)
            {
                _debugData.Append ("- Maximum bits for current version: " + maxBits + " current bits: " + _output.Count + "\n");
            }

            // Add zeros to end of bit string, if we are missing them

            int bitOverflow = 0;

            while (_output.Count < maxBits)
            {
                _output.Enqueue (false);

                bitOverflow++;

                if (bitOverflow >= 4)
                {
                    break;
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Added " + bitOverflow + " bits to binary output. Total: " + _output.Count + " bits.\n\n");
            }

            if ((_breakPoint == QRBreakPoint.FinishedBinary) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 5 -------");

                return;
            }

            //
            // Step 5 - breaking the bit string into bytes
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 5 ------------------\n- Dividing bits into bytes (8 bits per byte) ...\n");
                _debugData.Append ("- Current output size: " + _output.Count + " bits\n");
            }

            // For now we will just make sure it is divided by 8, we will not convert to bytes unless required.

            while (_output.Count % 8 != 0)
            {
                _output.Enqueue (false);
            }

            if (_debugData != null)
            {
                _debugData.Append ("- Added zeros to output, current size: " + _output.Count + " bits, " + (_output.Count / 8) + " bytes.\n\n");
            }

            if ((_breakPoint == QRBreakPoint.BrokenBinary) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 6 -------");

                return;
            }

            //
            // Step 6 - inserting characters
            //

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 6 ------------------\n- Inserting characters to fill missing data string ...\n");
            }

            // If we have too many bits
            if ((_output.Count / 8) > _version.DataSize)
            {
                throw new Exception ("Text is too long for current matrix.");
            }

            // If we have less or exact number of bits

            List<bool> defaultBlock1 = BinaryExtensions.DecToBin (236, 8);
            List<bool> defaultBlock2 = BinaryExtensions.DecToBin (17, 8);

            // Fill matrix until it is full as specified in version
            int fillCount = 0;

            if (_debugData != null)
            {
                _debugData.Append ("- Current output size: " + (_output.Count / 8) + ", missing: " + (_version.DataSize - (_output.Count / 8)) + " bytes.\n");
                _debugData.Append ("- Added bytes: ");
            }

            while ((_output.Count / 8) < _version.DataSize)
            {
                if ((fillCount % 2) == 0)
                {
                    if (_debugData != null)
                    {
                        _debugData.Append ("236, ");
                    }

                    ToOutput (defaultBlock1);
                }
                else
                {
                    if (_debugData != null)
                    {
                        _debugData.Append ("17, ");
                    }

                    ToOutput (defaultBlock2);
                }

                fillCount++;
            }

            if (_debugData != null)
            {
                _debugData.Append ("\n- Output size filled to match data size: " + (_output.Count / 8) + "/" + _version.DataSize + " bytes\n\n");
            }

            if ((_breakPoint == QRBreakPoint.MissingBytes) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 7 -------");

                return;
            }

            //
            // Step 7 - generating error code correction (ECC) by Reed-Solomon procedure
            //

            // Convert bits from output to byte array
            byte[] bytes = BinaryExtensions.BitArrayToByteArray (new BitArray (_output.ToArray ()));

            if (_debugData != null)
            {
                _debugData.Append ("------------------ Step 7 ------------------\n- Calculating error code correction ...\n");
                _debugData.Append ("- Dividing data words into blocks: " + _version.GetBlockCount () + " total.\n");
                _debugData.Append ("- Creating message polynomial for each block ...\n");
            }

            //
            // Divide bytes into blocks
            //

            QRBlock[] blocks = new QRBlock[_version.GetBlockCount ()];

            for (int x = 0; x < _version.GetBlockCount (); x++)
            {
                //
                // Creating message polynominal system (F(x)) for current block
                //

                //
                // Calculate block length and starting position
                //

                int blockLength;
                int blockStart;

                if (x >= _version.BlockOneCount)
                {
                    blockLength = _version.BlockTwoSize;

                    blockStart = (_version.BlockOneSize * _version.BlockOneCount) + ((x - _version.BlockOneCount) * _version.BlockTwoSize);
                }
                else
                {
                    blockLength = _version.BlockOneSize;

                    blockStart = (_version.BlockOneSize * x);
                }

                if (_debugData != null)
                {
                    _debugData.Append ("----------------------------------\n- Block " + (x + 1) + ", data from: " + blockStart + ", length: " + blockLength + "\n- Message polynomial:\n\n");
                }

                Poly message = new Poly ();

                int messageLevel = _version.BlockOneSize + (_version.ErrorSize / _version.GetBlockCount ()) - 1;

                for (int i = 0; i < blockLength; i++)
                {
                    if (_debugData != null)
                    {
                        _debugData.Append ("(" + bytes[i + blockStart] + "x ^ " + messageLevel + ")");

                        if (i != blockLength - 1)
                        {
                            _debugData.Append (" + ");
                        }
                    }

                    Term ter = new Term (messageLevel, bytes[i + blockStart]);
                    message.Terms.Add (ter);

                    messageLevel--;
                }

                if (_debugData != null)
                {
                    _debugData.Append ("\n\n- Calculating " + ((int) Math.Floor (_version.ErrorSize / (double) _version.GetBlockCount ())) + " error codes using Reed-Solomon algorithm.\n");
                }

                //
                // Calculate Error codes of desired block
                //

                blocks[x] = new QRBlock
                                {
                                    MessagePoly = new Poly (message)
                                };

                //
                // Create Reed Solomon algorithm class
                //

                QRReedSolomon ecc = new QRReedSolomon ();

                // Experimental bugfix: rounding the result, so we can choose correct ECC
                blocks[x].EccCoefficients = ecc.Calculate (message, (int) Math.Floor (_version.ErrorSize / (double) _version.GetBlockCount ()));

                if (_debugData != null)
                {
                    _debugData.Append ("- Calculated error codes: " + BinaryExtensions.ArrayToString (blocks[x].EccCoefficients) + "\n\n");
                }
            }

            //
            // Write error codes to bit stream
            //

            // Clear current output
            _output.Clear ();

            //
            // Writing data bits
            //

            if (_debugData != null)
            {
                _debugData.Append ("- Writing data bits into output:\n- ");
            }

            // Blocks might not be the same size...
            for (int i = 0; i < Math.Max (_version.BlockOneSize, _version.BlockTwoSize); i++)
            {
                foreach (QRBlock t in blocks)
                {
                    if (i < t.MessagePoly.Terms.Length)
                    {
                        ToOutput (BinaryExtensions.DecToBin (t.MessagePoly.Terms[i].Coefficient, 8));

                        if (_debugData != null)
                        {
                            _debugData.Append (t.MessagePoly.Terms[i].Coefficient + ", ");
                        }
                    }
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("\n\n- Writing ECC bits to output:\n- ");
            }

            //
            // Writting ECC bits
            //

            for (int i = 0; i < blocks[0].EccCoefficients.Length; i++)
            {
                if (i == 17)
                {
                    if (_debugData != null)
                    {
                        _debugData.Append (",");
                    }
                }

                foreach (QRBlock t in blocks)
                {
                    ToOutput (BinaryExtensions.DecToBin (t.EccCoefficients[i], 8));

                    if (_debugData != null)
                    {
                        _debugData.Append (t.EccCoefficients[i] + ", ");
                    }
                }
            }

            if (_debugData != null)
            {
                _debugData.Append ("\n\n");
            }

            //
            // Step 8, 9, 10 - Handled by QRImage class
            //

            if ((_breakPoint == QRBreakPoint.ErrorCode) && (_debugData != null))
            {
                _debugData.Append ("------- Breakpoint reached: Step 8 -------");

                return;
            }

            _img.CreateImage (_output);

            if (_debugData != null)
            {
                _debugData.Append (_img.GetDebugData ());
            }
        }

        public System.Drawing.Image Render (int resolution)
        {
            if (_img == null)
            {
                Process ();
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

            if (_img != null)
            {
                return _img.Render (res);
            }

            return null;
        }

        private void ToOutput (IEnumerable<bool> list)
        {
            foreach (bool b in list)
            {
                _output.Enqueue (b);
            }
        }
    }
}