using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// An entry door.
    /// </summary>
    public class EntryDoor : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_ENTRY_DOOR";

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The wall the door is placed on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                if ((Direction)((int)value % 3) == Direction.NegZ)
                    throw new ArgumentException("Invalid wall - door can not be placed on floor or ceiling.");

                Direction normal = GetNormalFromWall(value);

                ItemFacing = new ItemFacing(normal, ItemFacing.Cross(normal, Direction.Z));
            }
        }

        #endregion

        /// <summary>
        /// Default value - if different, there may be something wrong.
        /// </summary>
        private double DoorInstanceID = 1.000000;

        #endregion

        #region Constructor

        public EntryDoor(Puzzle owner) : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = false;
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "DoorInstanceID":
                    DoorInstanceID = property.GetDouble();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("DoorInstanceID", DoorInstanceID));

            base.AddProperties(node);
        }

        #endregion
    }
}
