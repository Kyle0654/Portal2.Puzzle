using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// An extent for a piston platform.
    /// </summary>
    public class PistonPlatformExtent : Extent
    {
        #region Properties

        /// <summary>
        /// Which end this extent is.
        /// </summary>
        internal int EndHandle { get; set; }

        #endregion

        #region Constructor

        public PistonPlatformExtent(Puzzle owner)
            : base(owner)
        {
            defaultNormal = Direction.Z;
            defaultRight = Direction.Y;
            Type = Extent.PistonPlatformTypeName;
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            if (property.Key == "EndHandle")
                EndHandle = property.GetInt32();

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            base.AddProperties(node);

            node.Nodes.Add(new P2CProperty("EndHandle", EndHandle));
        }

        #endregion
    }
}
