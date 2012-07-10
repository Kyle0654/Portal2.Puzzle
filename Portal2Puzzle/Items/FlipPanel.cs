using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A wall panel that flips to reveal the opposite portalability.
    /// </summary>
    public class FlipPanel : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_PANEL_FLIP";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this panel on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the flip panel is on.
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
        /// The axis around which to flip the panel
        /// </summary>
        public Direction FlipAxis
        {
            get { return ItemFacing.Right; }
            set
            {
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this panel starts flipped.
        /// </summary>
        public bool StartFlipped { get; set; }

        /// <summary>
        /// Whether or not this panel is portalable (will change the voxel).
        /// </summary>
        public bool IsPortalable
        {
            get { return owner.Voxels.GetVoxel(VoxelPosition).IsPortalable(Wall); }
            set { owner.Voxels.SetPortalable(VoxelPosition, Wall, value); }
        }

        #endregion

        #region Constructor

        public FlipPanel(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            StartFlipped = true;

            ConnectionReceiver = new ConnectionLogicReceiver(this);
        }

        #endregion

        #region Input / Output

        internal override void FinishReadProperties()
        {
            normal = ItemFacing.Normal;
            right = ItemFacing.Right;

            base.FinishReadProperties();
        }

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_CONNECTION_COUNT":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_START_ENABLED":
                    StartFlipped = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", StartFlipped));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            base.AddProperties(node);
        }

        #endregion
    }
}
