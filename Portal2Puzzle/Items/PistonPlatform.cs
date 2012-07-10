using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A platform that rises and falls.
    /// </summary>
    public class PistonPlatform : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_PISTON_PLATFORM";

        #endregion

        #region Fields

        private int nearExtentOffset = 0;
        private int farExtentOffset = 0;

        private PistonPlatformExtent nearExtent;
        private PistonPlatformExtent farExtent;

        private MultiConnectionPointSender platformExtentSender;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this platform on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the platform is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                Direction normal = GetNormalFromWall(value);
                ItemFacing = new ItemFacing(normal, (Direction)(((int)normal + 1) % 6));

                UpdateExtents();
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// The position of this platform.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                base.VoxelPosition = value;
                UpdateExtents();
            }
        }

        #endregion

        #region Extents

        /// <summary>
        /// Gets or sets the near extent.
        /// </summary>
        public int NearExtent
        {
            get { return nearExtentOffset; }
            set
            {
                if (value < 0)
                    return;

                nearExtentOffset = value;

                // Far extent must be at least the near extent
                farExtentOffset = Math.Max(nearExtentOffset, farExtentOffset);

                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the far extent.
        /// </summary>
        public int FarExtent
        {
            get { return farExtentOffset; }
            set
            {
                farExtentOffset = value;

                // Near extent must be at most the far extent
                nearExtentOffset = Math.Min(nearExtentOffset, farExtentOffset);

                UpdateExtents();
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this platform start in the up position.
        /// </summary>
        public bool StartUp { get; set; }

        #endregion

        #region Constructors

        public PistonPlatform(Puzzle owner)
            : this(owner, true)
        {
        }

        internal PistonPlatform(Puzzle owner, bool createExtents)
            : base(owner)
        {
            defaultNormal = Direction.Z;
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            platformExtentSender = new MultiConnectionPointSender(this);
            platformExtentSender.Connected += new EventHandler(platformExtentSender_Connected);

            ConnectionReceiver = new ConnectionLogicReceiver(this);

            if (createExtents)
            {
                PistonPlatformExtent min = new PistonPlatformExtent(owner) { EndHandle = 0 };
                PistonPlatformExtent max = new PistonPlatformExtent(owner) { EndHandle = 1 };
                owner.Items.Add(min);
                owner.Items.Add(max);
                platformExtentSender.ConnectTo(min.extentConnectionReceiver);
                platformExtentSender.ConnectTo(max.extentConnectionReceiver);
            }
        }

        #endregion

        #region Extents

        private void UpdateExtents()
        {
            // Can't set extents if they don't exist
            if (nearExtent == null || farExtent == null)
                return;

            Point3 dir = ItemFacing.DirectionToPoint(GetNormalFromWall(Wall));

            nearExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.Right);
            farExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.Right);

            nearExtent.VoxelPosition = VoxelPosition + dir * nearExtentOffset;
            farExtent.VoxelPosition = VoxelPosition + dir * farExtentOffset;
        }

        #endregion

        #region Connection

        void platformExtentSender_Connected(object sender, EventArgs e)
        {
            // Determine min and max
            PistonPlatformExtent near = null;
            PistonPlatformExtent far = null;

            int nearDist = 0;
            int farDist = 0;

            foreach (Connection connection in platformExtentSender.Connections)
            {
                if (connection.Receiver == null)
                    continue;

                PistonPlatformExtent extent = connection.Receiver.Owner as PistonPlatformExtent;
                if (extent == null)
                    continue;

                int dist = (VoxelPosition - extent.VoxelPosition).MaxDimension();

                // Check which extent is which
                if (extent.EndHandle == 0)
                {
                    near = extent;
                    nearDist = dist;
                    continue;
                }
                else if (extent.EndHandle == 1)
                {
                    far = extent;
                    farDist = dist;
                    continue;
                }
            }

            // Need to have both extents
            if (near == null || far == null)
            {
                nearExtent = null;
                farExtent = null;
                return;
            }

            nearExtent = near;
            farExtent = far;

            // Update distances
            nearExtentOffset = nearDist;
            farExtentOffset = farDist;

            // Update extents
            UpdateExtents();
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
                case "ITEM_PROPERTY_PISTON_LIFT_BOTTOM_LEVEL":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_PISTON_LIFT_TOP_LEVEL":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_PISTON_LIFT_START_UP":
                    StartUp = property.GetBool();
                    break;
                case "ITEM_PROPERTY_PISTON_ALLOW_AUTO_TRIGGER":
                    // Don't really need to read this
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            P2CProperty connectionCount = ConnectionReceiver.GetConnectionCount();
            node.Nodes.Add(connectionCount);

            // Bottom and top level refer to the end handles. We'll assume they're always 0 and 1 for now.
            // (they could technically be different, but no tests have shown so)
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PISTON_LIFT_BOTTOM_LEVEL", 0));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PISTON_LIFT_TOP_LEVEL", 1));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PISTON_LIFT_START_UP", StartUp));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PISTON_ALLOW_AUTO_TRIGGER", (connectionCount.GetInt32() == 0)));

            base.AddProperties(node);
        }

        #endregion
    }
}
