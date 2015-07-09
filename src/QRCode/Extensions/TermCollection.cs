// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TermCollection.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The term collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Extensions
{
    using System.Collections;

    /// <summary>
    /// The term collection.
    /// </summary>
    public class TermCollection : CollectionBase
    {
        #region Enums

        /// <summary>
        /// Array Sort Type
        /// Values: ASC , DESC
        /// </summary>
        public enum SortType
        {
            /// <summary>
            /// The <c>asc</c>.
            /// </summary>
            Asc = 0, 

            /// <summary>
            /// The des.
            /// </summary>
            Des = 1
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.List.Count;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Index Collections Definition:
        /// </summary>
        /// <param name="index">
        /// Index to return.
        /// </param>
        /// <returns>
        /// The <see cref="Term"/>.
        /// </returns>
        public Term this[int index]
        {
            get
            {
                return (Term)this.List[index];
            }

            set
            {
                this.List[index] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Modified Add Method: 
        /// First check the Coefficient Value. 
        /// this Method checks if there is any Term by the Same Input Power.
        /// </summary>
        /// <param name="value">
        /// Term value to add.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Add(Term value)
        {
            if (this.HasTermByPower(value.Power))
            {
                this.AddToEqualPower(value);
                return -1;
            }

            return this.List.Add(value);
        }

        /// <summary>
        /// This will Add the Term to the Same Power Term if it's already exists in the Collection.
        /// This mean we Plus the New Terms to the Current Polynomial
        /// </summary>
        /// <param name="value">
        /// Value to add.
        /// </param>
        public void AddToEqualPower(Term value)
        {
            foreach (Term t in this.List)
            {
                if (t.Power == value.Power)
                {
                    t.Coefficient += value.Coefficient;
                }
            }
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(Term value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Check if there is any Term by the given Power.
        /// </summary>
        /// <param name="p">
        /// Power to check
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasTermByPower(int p)
        {
            foreach (Term t in this.List)
            {
                if (t.Power == p)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The index of.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int IndexOf(Term value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Insert(int index, Term value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Remove(Term value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Sort Method: Sort Items of Collection in ASC or DESC Order.
        /// </summary>
        /// <param name="order">
        /// SortOrder values : ASC or DESC
        /// </param>
        public void Sort(SortType order)
        {
            TermCollection result = new TermCollection();
            if (order == SortType.Asc)
            {
                while (this.Length > 0)
                {
                    Term minTerm = this[0];
                    foreach (Term t in this.List)
                    {
                        if (t.Power < minTerm.Power)
                        {
                            minTerm = t;
                        }
                    }

                    result.Add(minTerm);
                    this.Remove(minTerm);
                }
            }
            else
            {
                while (this.Length > 0)
                {
                    Term maxTerm = this[0];
                    foreach (Term t in this.List)
                    {
                        if (t.Power > maxTerm.Power)
                        {
                            maxTerm = t;
                        }
                    }

                    result.Add(maxTerm);
                    this.Remove(maxTerm);
                }
            }

            this.Clear();
            foreach (Term t in result)
            {
                this.Add(t);
            }
        }

        #endregion
    }
}