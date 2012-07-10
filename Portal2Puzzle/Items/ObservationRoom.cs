using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// An observation room - a level can only contain one of these.
    /// </summary>
    public class ObservationRoom : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_OBSERVATION_ROOM";

        #endregion

        #region Properties

        /// <summary>
        /// The wall the observation room is placed on.
        /// </summary>
        public Direction Wall
        {
            get { return ItemFacing.OppositeDirection(ItemFacing.Normal); }
            set
            {
                if ((Direction)((int)value % 3) == Direction.NegZ)
                    throw new ArgumentException("Invalid wall - observation room can not be placed on floor or ceiling.");

                Direction normal = GetNormalFromWall(value);

                ItemFacing = new ItemFacing(normal, ItemFacing.Cross(normal, Direction.Z));
            }
        }

        #endregion

        #region Constructor

        public ObservationRoom(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = false;
            angles = new Point3(-90, 0, 0);
        }

        #endregion
    }
}
