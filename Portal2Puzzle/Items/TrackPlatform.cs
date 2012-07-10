using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A platform that moves up and down or side to side along a wall.
    /// </summary>
    public class TrackPlatform : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_RAIL_PLATFORM";

        #endregion

        #region Fields

        private Direction normal;
        private Direction forward;

        private int backExtentOffset = 0;
        private int leftExtentOffset = 0;
        private int forwardExtentOffset = 0;
        private int rightExtentOffset = 0;

        private Extent backExtent;
        private Extent leftExtent;
        private Extent forwardExtent;
        private Extent rightExtent;

        private MultiConnectionPointSender platformExtentSender;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this platform on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the panel is on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)forward % 3)
                {
                    Direction right = ItemFacing.Cross(forward, normal);
                    ItemFacing = new ItemFacing(normal, right);
                    UpdateExtents();
                }
            }
        }

        /// <summary>
        /// The direction the panel will face.
        /// </summary>
        public Direction FacingDirection
        {
            get
            {
                return ItemFacing.Cross(ItemFacing.Normal, ItemFacing.Right);
            }
            set
            {
                forward = value;
                if ((int)normal % 3 != (int)forward % 3)
                {
                    Direction right = ItemFacing.Cross(forward, normal);
                    ItemFacing = new ItemFacing(normal, right);
                    UpdateExtents();
                }
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
        /// Gets or sets the backward extent (sets left and right extent to 0 if not 0).
        /// </summary>
        public int BackExtent
        {
            get { return backExtentOffset; }
            set
            {
                if (value < 0)
                    return;

                backExtentOffset = value;

                if (value != 0)
                {
                    leftExtentOffset = 0;
                    rightExtentOffset = 0;
                }

                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the left extent (sets forward and back extent to 0 if not 0).
        /// </summary>
        public int LeftExtent
        {
            get { return leftExtentOffset; }
            set
            {
                if (value < 0)
                    return;

                leftExtentOffset = value;

                if (value != 0)
                {
                    forwardExtentOffset = 0;
                    backExtentOffset = 0;
                }

                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the forward extent (sets left and right extent to 0 if not 0).
        /// </summary>
        public int ForwardExtent
        {
            get { return forwardExtentOffset; }
            set
            {
                if (value < 0)
                    return;

                forwardExtentOffset = value;

                if (value != 0)
                {
                    leftExtentOffset = 0;
                    rightExtentOffset = 0;
                }

                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the right extent (sets forward and back extent to 0 if not 0).
        /// </summary>
        public int RightExtent
        {
            get { return rightExtentOffset; }
            set
            {
                if (value < 0)
                    return;

                rightExtentOffset = value;

                if (value != 0)
                {
                    forwardExtentOffset = 0;
                    backExtentOffset = 0;
                }

                UpdateExtents();
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this platform starts active.
        /// </summary>
        public bool StartActive { get; set; }

        /// <summary>
        /// Whether or not this platform oscillates on the rail (should have a button hooked up if not).
        /// </summary>
        public bool RailOscillate { get; set; }

        #endregion

        #region Constructors

        public TrackPlatform(Puzzle owner)
            : this(owner, true)
        {
        }

        internal TrackPlatform(Puzzle owner, bool createExtents)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            StartActive = true;
            RailOscillate = true;

            platformExtentSender = new MultiConnectionPointSender(this);
            platformExtentSender.Connected += new EventHandler(platformExtentSender_Connected);

            ConnectionReceiver = new ConnectionLogicReceiver(this);

            if (createExtents)
            {
                Extent back = new Extent(owner) { ExtentType = ExtentType.RailPlatform };
                Extent left = new Extent(owner) { ExtentType = ExtentType.RailPlatform };
                Extent forward = new Extent(owner) { ExtentType = ExtentType.RailPlatform };
                Extent right = new Extent(owner) { ExtentType = ExtentType.RailPlatform };
                owner.Items.Add(back);
                owner.Items.Add(left);
                owner.Items.Add(forward);
                owner.Items.Add(right);
                platformExtentSender.ConnectTo(back.extentConnectionReceiver);
                platformExtentSender.ConnectTo(left.extentConnectionReceiver);
                platformExtentSender.ConnectTo(forward.extentConnectionReceiver);
                platformExtentSender.ConnectTo(right.extentConnectionReceiver);
            }
        }

        #endregion

        #region Extents

        private void UpdateExtents()
        {
            // Can't set extents if they don't exist
            if (backExtent == null || leftExtent == null || forwardExtent == null || rightExtent == null)
                return;

            Point3 rightDir = ItemFacing.DirectionToPoint(ItemFacing.Right);
            Direction forward = ItemFacing.Cross(ItemFacing.Normal, ItemFacing.Right);
            Point3 forwardDir = ItemFacing.DirectionToPoint(forward);

            backExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.Right);
            leftExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.Cross(ItemFacing.Right, ItemFacing.Normal));
            forwardExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.OppositeDirection(ItemFacing.Right));
            rightExtent.ItemFacing = new ItemFacing(ItemFacing.Normal, ItemFacing.Cross(ItemFacing.Normal, ItemFacing.Right));

            backExtent.VoxelPosition = VoxelPosition - forwardDir * backExtentOffset;
            leftExtent.VoxelPosition = VoxelPosition - rightDir * leftExtentOffset;
            forwardExtent.VoxelPosition = VoxelPosition + forwardDir * forwardExtentOffset;
            rightExtent.VoxelPosition = VoxelPosition + rightDir * rightExtentOffset;
        }

        #endregion

        #region Connection

        void platformExtentSender_Connected(object sender, EventArgs e)
        {
            Extent backExt = null;
            Extent leftExt = null;
            Extent forwardExt = null;
            Extent rightExt = null;

            foreach (Connection connection in platformExtentSender.Connections)
            {
                if (connection.Receiver == null)
                    continue;

                Extent extent = connection.Receiver.Owner as Extent;
                if (extent == null)
                    continue;

                // Always in the order back, left, forward, right
                if (backExt == null)
                {
                    backExt = extent;
                    continue;
                }

                if (leftExt == null)
                {
                    leftExt = extent;
                    continue;
                }

                if (forwardExt == null)
                {
                    forwardExt = extent;
                    continue;
                }

                else if (rightExt == null)
                {
                    rightExt = extent;
                    break;
                }
            }

            // Need to have all extents
            if (backExt == null || leftExt == null || forwardExt == null || rightExt == null)
            {
                backExtent = null;
                leftExtent = null;
                forwardExtent = null;
                rightExtent = null;
                return;
            }

            backExtent = backExt;
            leftExtent = leftExt;
            forwardExtent = forwardExt;
            rightExtent = rightExt;

            // Update distances
            backExtentOffset = (backExtent.VoxelPosition - VoxelPosition).MaxDimension();
            leftExtentOffset = (leftExtent.VoxelPosition - VoxelPosition).MaxDimension();
            forwardExtentOffset = (forwardExtent.VoxelPosition - VoxelPosition).MaxDimension();
            rightExtentOffset = (rightExtent.VoxelPosition - VoxelPosition).MaxDimension();
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
                case "ITEM_PROPERTY_RAIL_OSCILLATE":
                    RailOscillate = property.GetBool();
                    break;
                case "ITEM_PROPERTY_RAIL_STARTING_POSITION":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_RAIL_TRAVEL_DISTANCE":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_RAIL_SPEED":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_RAIL_TRAVEL_DIRECTION":
                    // Don't really need to read this
                    break;
                case "ITEM_PROPERTY_RAIL_START_ACTIVE":
                    StartActive = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            // TODO: Many of these seem to have unchanging defaults - figure out if they ever change?
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_OSCILLATE", RailOscillate));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_STARTING_POSITION", 0));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_TRAVEL_DISTANCE", 0));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_SPEED", 100));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_TRAVEL_DIRECTION", (forwardExtentOffset == 0 && backExtentOffset == 0) ? 1 : 0));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_RAIL_START_ACTIVE", StartActive));

            base.AddProperties(node);
        }

        #endregion
    }
}
