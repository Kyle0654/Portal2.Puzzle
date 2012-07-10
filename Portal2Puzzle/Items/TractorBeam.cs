using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A tractor beam that will move objects along its path.
    /// </summary>
    public class TractorBeam : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_TBEAM";

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this tractor beam on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        /// <summary>
        /// Connection receiver to reverse the polarity of this receiver.
        /// </summary>
        public ConnectionLogicReceiver PolarityConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the tractor beam is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                Direction normal = GetNormalFromWall(value);
                ItemFacing = new ItemFacing(normal, (Direction)(((int)normal + 1) % 6));
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this tractor beam starts enabled.
        /// </summary>
        public bool StartEnabled { get; set; }

        /// <summary>
        /// Whether or not this tractor beam starts reversed.
        /// </summary>
        public bool StartReversed { get; set; }

        #endregion

        #region Constructor

        public TractorBeam(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.NegX;

            Type = TypeName;
            deletable = true;

            StartEnabled = true;
            StartReversed = false;

            ConnectionReceiver = new ConnectionLogicReceiver(this);
            PolarityConnectionReceiver = new ConnectionLogicReceiver(this, "CONNECTION_TBEAM_POLARITY");
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_CONNECTION_COUNT":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_CONNECTION_COUNT_POLARITY":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_START_ENABLED":
                    StartEnabled = property.GetBool();
                    break;
                case "ITEM_PROPERTY_START_REVERSED":
                    StartReversed = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", StartEnabled));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_REVERSED", StartReversed));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount("ITEM_PROPERTY_CONNECTION_COUNT_POLARITY"));

            base.AddProperties(node);
        }

        #endregion
    }
}
