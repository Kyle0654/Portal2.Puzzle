using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A smaller observation room.
    /// </summary>
    public class SecondaryObservationRoom : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_SECONDARY_OBSERVATION_ROOM";

        #endregion

        #region Properties

        #region Orientation

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

        #endregion

        #region Constructor

        public SecondaryObservationRoom(Puzzle owner) : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;
            angles = new Point3(-90, 0, 0);
        }

        #endregion
    }
}
