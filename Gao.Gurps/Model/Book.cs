using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Represents a GURPS book, and some of its metadata.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Whether it is GURPS, 4e, Traveller, etc.
        /// </summary>
        public string ProductLine { get; set; }
        /// <summary>
        /// Whether it is 4e or older.
        /// </summary>
        public int Edition { get; set; }

        /// <summary>
        /// Tells if a value is equal to another value.
        /// </summary>
        /// <param name="obj">value to compare</param>
        /// <returns>true if they are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Book)) return false;
            var other = obj as Book;
            return Edition == other.Edition &&
                ProductLine == other.ProductLine &&
                Title == other.Title;
        }

        /// <summary>
        /// Gets hash code.
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                // Maybe nullity checks, if these are objects not primitives!
                hash = hash * 23 + Title.GetHashCode();
                hash = hash * 23 + ProductLine.GetHashCode();
                hash = hash * 23 + Edition.GetHashCode();
                return hash;
            }
        }
    }
}
