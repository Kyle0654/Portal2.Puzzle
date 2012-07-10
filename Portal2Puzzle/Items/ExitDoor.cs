using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    public class ExitDoor : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_EXIT_DOOR";

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this door on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

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
        /// Whether or not the exit door starts open or locked.
        /// </summary>
        public bool StartOpen { get; set; }

        /// <summary>
        /// Default value - if different, there may be something wrong.
        /// </summary>
        private double DoorInstanceID = 3.000000;

        #endregion

        #region Constructor

        public ExitDoor(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = false;
            StartOpen = true;

            ConnectionReceiver = new ConnectionLogicReceiver(this);
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
                case "ITEM_PROPERTY_CONNECTION_COUNT":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_START_OPEN":
                    StartOpen = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("DoorInstanceID", DoorInstanceID));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_OPEN", StartOpen));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            base.AddProperties(node);
        }

        #endregion
    }
}
