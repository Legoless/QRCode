namespace QRCode.ErrorCorrection
{
    public class QRBoseChaudhuriHocquenghem
    {
        public static int CalculateBch (int value, int poly)
        {
            //
            // Get poly power, we subtract one because finite field start with x0
            //

            int polyPower = CountDigits (poly);
            value = value << (polyPower - 1);

            //
            // Dividing until we reach same power
            //

            while (CountDigits (value) >= polyPower)
            {
                value = value ^ (poly << (CountDigits (value) - polyPower));
            }

            //
            // Return reminder
            //

            return value;
        }

        private static int CountDigits (int value)
        {
            int digits = 0;

            while (value != 0)
            {
                value = (int) ((uint) value >> 1);
                digits++;
            }

            return digits;
        }
    }
}