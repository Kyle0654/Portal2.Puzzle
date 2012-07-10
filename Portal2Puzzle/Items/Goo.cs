using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// Water that kills upon contact.
    /// </summary>
    public class Goo : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_GOO";

        #endregion

        #region Constructor

        public Goo(Puzzle owner) : base(owner)
        {
            Type = TypeName;
            deletable = true;
        }

        #endregion
    }
}
