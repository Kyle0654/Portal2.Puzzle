using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// An emitter for lasers.
    /// </summary>
    public class LaserEmitter : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_LASER_EMITTER_CENTER";
        internal const string TypeNameOffset = "ITEM_LASER_EMITTER_OFFSET";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this emitter on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the emitter is sitting on.
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
        /// Whether this emitter is centered or offset.
        /// </summary>
        public bool IsCentered
        {
            get { return Type == TypeName; }
            set { Type = value ? TypeName : TypeNameOffset; }
        }

        /// <summary>
        /// The offset direction of this emitter (setting this will set the emitter to not centered).
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
        /// Whether or not this emitter starts enabled.
        /// </summary>
        public bool StartEnabled { get; set; }

        #endregion

        #region Constructor

        public LaserEmitter(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.NegX;

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
