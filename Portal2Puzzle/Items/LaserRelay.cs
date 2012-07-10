using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A relay for lasers.
    /// </summary>
    public class LaserRelay : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_LASER_RELAY_CENTER";
        internal const string TypeNameOffset = "ITEM_LASER_RELAY_OFFSET";

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
        /// The wall the relay is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        /// <summary>
        /// Whether this relay is centered or offset.
        /// </summary>
        public bool IsCentered
        {
            get { return Type == TypeName; }
            set { Type = value ? TypeName : TypeNameOffset; }
        }

        /// <summary>
        /// The offset direction of this relay (setting this will set the relay to not centered).
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

        #endregion

        #region Constructor

        public LaserRelay(Puzzle owner)
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
