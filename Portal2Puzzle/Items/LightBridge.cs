using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A light bridge.
    /// </summary>
    public class LightBridge : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_LIGHT_BRIDGE";

        #endregion

        #region Fields

        private Direction normal;
        private Direction up;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this light bridge on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the light bridge is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)up % 3)
                    ItemFacing = new ItemFacing(normal, ItemFacing.Cross(normal, up));
            }
        }

        /// <summary>
        /// The topside of the bridge.
        /// </summary>
        public Direction BridgeTop
        {
            get { return ItemFacing.Cross(ItemFacing.Right, ItemFacing.Normal); }
            set
            {
                up = value;
                if ((int)normal % 3 != (int)up % 3)
                    ItemFacing = new ItemFacing(normal, ItemFacing.Cross(normal, up));
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this light bridge starts enabled.
        /// </summary>
        public bool StartEnabled { get; set; }

        #endregion

        #region Constructor

        public LightBridge(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            StartEnabled = true;

            ConnectionReceiver = new ConnectionLogicReceiver(this);
        }

        #endregion

        #region Input / Output

        internal override void FinishReadProperties()
        {
            normal = ItemFacing.Normal;
            up = ItemFacing.Right;

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
                    StartEnabled = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", StartEnabled));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            base.AddProperties(node);
        }

        #endregion
    }
}
