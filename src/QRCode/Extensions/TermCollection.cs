using System.Collections;

namespace QRCode.Extensions
{
    public class TermCollection : CollectionBase
    {
        #region Custom Enum Definition:

        /// <summary>
        /// Array Sort Type
        /// Values: ASC , DESC
        /// </summary>
        public enum SortType
        {
            Asc = 0,
            Des = 1
        }

        #endregion

        #region Custom Methods:

        /// <summary>
        /// Sor Method: Sort Items of Collection in ASC or DESC Order.
        /// </summary>
        /// <param name="order">SortOrder values : ASC or DESC</param>
        public void Sort (SortType order)
        {
            TermCollection result = new TermCollection ();
            if (order == SortType.Asc)
            {
                while (Length > 0)
                {
                    Term minTerm = this[0];
                    foreach (Term t in List)
                    {
                        if (t.Power < minTerm.Power)
                        {
                            minTerm = t;
                        }
                    }
                    result.Add (minTerm);
                    Remove (minTerm);
                }
            }
            else
            {
                while (Length > 0)
                {
                    Term maxTerm = this[0];
                    foreach (Term t in List)
                    {
                        if (t.Power > maxTerm.Power)
                        {
                            maxTerm = t;
                        }
                    }
                    result.Add (maxTerm);
                    Remove (maxTerm);
                }
            }

            Clear ();
            foreach (Term t in result)
            {
                Add (t);
            }
        }

        /// <summary>
        /// This will Add the Term to the Same Power Term if it's already exists in the Collection.
        /// This mean we Plus the New Terms to the Currnet Polynomial
        /// </summary>
        /// <param name="value"></param>
        public void AddToEqualPower (Term value)
        {
            foreach (Term t in List)
            {
                if (t.Power == value.Power)
                {
                    t.Coefficient += value.Coefficient;
                }
            }
        }

        /// <summary>
        /// Check if there is any Term by the given Power.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool HasTermByPower (int p)
        {
            foreach (Term t in List)
            {
                if (t.Power == p)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Index Collections Definition:
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Term this [int index]
        {
            get
            {
                return ((Term) List[index]);
            }
            set
            {
                List[index] = value;
            }
        }


        public int Length
        {
            get
            {
                return List.Count;
            }
        }


        /// <summary>
        /// Modified Add Method: 
        /// First check the Coefficient Value. 
        /// this Method checks if there is any Term by the Same Input Power.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add (Term value)
        {
            if (HasTermByPower (value.Power))
            {
                AddToEqualPower (value);
                return -1;
            }

            return (List.Add (value));
        }


        public int IndexOf (Term value)
        {
            return (List.IndexOf (value));
        }

        public void Insert (int index, Term value)
        {
            List.Insert (index, value);
        }

        public void Remove (Term value)
        {
            List.Remove (value);
        }

        public bool Contatins (Term value)
        {
            return (List.Contains (value));
        }
    }
}