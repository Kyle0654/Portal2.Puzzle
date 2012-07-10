using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A 2x1 raising stair platform.
    /// </summary>
    public class Stairs : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_PANEL_STAIRS";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch these stairs on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the stairs are on.
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
        /// The direction to head up the stairs (they will extend one voxel in this direction).
        /// </summary>
        public Direction StairDirection
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
        /// Whether or not these stairs start deployed.
        /// </summary>
        public bool StartDeployed { get; set; }

        #endregion

        #region Constructor

        public Stairs(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.X;

            normal = defaultNormal;
            right = defaultRight;

            Type = TypeName;
            deletable = true;

            StartDeployed = true;

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
                case "ITEM_PROPERTY_START_DEPLOYED":
                    StartDeployed = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_DEPLOYED", StartDeployed));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            base.AddProperties(node);
        }

        #endregion
    }
}
