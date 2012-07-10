using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A target for lasers, acts as a switch.
    /// </summary>
    public class LaserCatcher : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_LASER_CATCHER_CENTER";
        internal const string TypeNameOffset = "ITEM_LASER_CATCHER_OFFSET";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        #endregion

        #region Properties

        /// <summary>
        /// The logic sending connection point.
        /// </summary>
        public ConnectionLogicSender ConnectionSender { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the catcher is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);

                if (IsCentered)
                    right = (Direction)(((int)normal + 1) % 6);

                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        /// <summary>
        /// The offset direction of this catcher (setting this will set the catcher to not centered).
        /// </summary>
        public Direction OffsetDirection
        {
            get { return ItemFacing.Right; }
            set
            {
                IsCentered = false;
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        #endregion

        /// <summary>
        /// Whether this catcher is centered or offset.
        /// </summary>
        public bool IsCentered
        {
            get { return Type == TypeName; }
            set { Type = value ? TypeName : TypeNameOffset; }
        }

        #endregion

        #region Constructor

        public LaserCatcher(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.NegX;

            Type = TypeName;
            deletable = true;

            ConnectionSender = new ConnectionLogicSender(this);
        }

        #endregion

        #region Input / Output

        internal override void FinishReadProperties()
        {
            normal = ItemFacing.Normal;
            right = ItemFacing.Right;

            base.FinishReadProperties();
        }

        #endregion
    }
}
