using System.Globalization;

namespace QRCode.Extensions
{
    public class Term
    {
        #region Constructors:

        /// <summary>
        /// Simple Constructor which Create a new Instanse of Term Class
        /// With 2 parameters
        ///  
        /// </summary>
        /// <param name="power"></param>
        /// <param name="coefficient"></param>
        public Term (int power, int coefficient)
        {
            Power = power;
            Coefficient = coefficient;
        }

        /// <summary>
        /// Constructor Overload which Create a new Instance of Term Class
        /// With a simple string and try to read the Power and Coefficient
        /// by identifing the input string
        /// </summary>
        /// <param name="termExpression"></param>
        public Term (string termExpression)
        {
            if (termExpression.Length > 0)
            {
                if (termExpression.IndexOf ("x^", System.StringComparison.Ordinal) > -1)
                {
                    string coefficientString = termExpression.Substring (0, termExpression.IndexOf ("x^", System.StringComparison.Ordinal));
                    int indexofX = termExpression.IndexOf ("x^", System.StringComparison.Ordinal);
                    string powerString = termExpression.Substring (indexofX + 2, (termExpression.Length - 1) - (indexofX + 1));
                    if (coefficientString == "-")
                    {
                        Coefficient = -1;
                    }
                    else if (coefficientString == "+" | coefficientString == "")
                    {
                        Coefficient = 1;
                    }
                    else
                    {
                        Coefficient = int.Parse (coefficientString);
                    }

                    Power = int.Parse (powerString);
                }
                else if (termExpression.IndexOf ("x", System.StringComparison.Ordinal) > -1)
                {
                    Power = 1;
                    string coefficientString = termExpression.Substring (0, termExpression.IndexOf ("x", System.StringComparison.Ordinal));
                    if (coefficientString == "-")
                    {
                        Coefficient = -1;
                    }
                    else if (coefficientString == "+" | coefficientString == "")
                    {
                        Coefficient = 1;
                    }
                    else
                    {
                        Coefficient = int.Parse (coefficientString);
                    }
                }
                else
                {
                    Power = 0;
                    Coefficient = int.Parse (termExpression);
                }
            }
            else
            {
                Power = 0;
                Coefficient = 0;
            }
        }

        #endregion

        #region Override Methods:

        /// <summary>
        /// This Override will push the Term in a String Form to the output.
        /// ToString() method will write the String Format of the Term Like: 3x^2
        /// Which means [Coefficient]x^[Power].
        /// This Method Also check if it's needed to have x^,x,- or + in the pattern.
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            string result = string.Empty;

            if (Coefficient >= 0)
            {
                result += " + ";
            }
            else
            {
                result += " - ";
            }

            if (Power == 0)
            {
                result += (Coefficient < 0 ? Coefficient * -1 : Coefficient).ToString (CultureInfo.InvariantCulture);
            }
            else if (Power == 1)
            {
                if (Coefficient > 1 | Coefficient < -1)
                {
                    result += string.Format ("{0} x ", (Coefficient < 0 ? Coefficient * -1 : Coefficient).ToString (CultureInfo.InvariantCulture));
                }
                else
                {
                    result += "x";
                }
            }
            else if (Coefficient > 1 | Coefficient < -1)
            {
                result += string.Format ("{0}x ^ {1}", (Coefficient < 0 ? Coefficient * -1 : Coefficient).ToString (CultureInfo.InvariantCulture), Power.ToString (CultureInfo.InvariantCulture));
            }
            else
            {
                result += string.Format ("x ^ {0}", Power.ToString (CultureInfo.InvariantCulture));
            }

            return result;
        }

        #endregion

        #region Fields & Properties:

        /// <summary>
        /// Private field to hold the Power Value.
        /// </summary>
        private int _power;

        /// <summary>
        /// Power Property
        /// Notice: Set Method Check if the value is Negetive and Make it Positive.
        /// </summary>
        public int Power
        {
            get
            {
                return _power;
            }
            set
            {
                if (value < 0)
                {
                    _power = value * -1;
                }
                else
                {
                    _power = value;
                }
            }
        }

        /// <summary>
        /// Coefficient Property
        /// </summary>
        public int Coefficient
        {
            get;
            set;
        }

        #endregion
    }
}