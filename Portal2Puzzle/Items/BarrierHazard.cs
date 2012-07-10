using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// Barrier hazard type.
    /// </summary>
    public enum BarrierHazardType : int
    {
        Fizzler = 0,
        Laserfield = 1
    }

    /// <summary>
    /// A barrier that either fizzles or kills.
    /// </summary>
    public class BarrierHazard : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_BARRIER_HAZARD";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;
        private bool isOffset = false;
        private int posExtentOffset = 0;
        private int negExtentOffset = 0;

        private Extent negExtent;
        private Extent posExtent;

        private MultiConnectionPointSender hazardExtentSender;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this barrier hazard on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the barrier hazard is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateOffset();
                UpdateExtents();
            }
        }

        /// <summary>
        /// The right of the barrier hazard.
        /// </summary>
        public Direction HazardRight
        {
            get { return ItemFacing.Right; }
            set
            {
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateOffset();
                UpdateExtents();
            }
        }

        /// <summary>
        /// Whether this barrier hazard is offset or centered.
        /// </summary>
        public bool IsOffset
        {
            get { return localPosition == FPoint3.Zero; }
            set
            {
                isOffset = value;
                UpdateOffset();
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// The position of this barrier hazard.
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
        /// Gets or sets the negative extent.
        /// </summary>
        public int NegExtent
        {
            get { return negExtentOffset; }
            set
            {
                negExtentOffset = value;
                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the positive extent.
        /// </summary>
        public int PosExtent
        {
            get { return posExtentOffset; }
            set
            {
                posExtentOffset = value;
                UpdateExtents();
            }
        }

        #endregion

        /// <summary>
        /// Whether or not this hazard starts enabled.
        /// </summary>
        public bool StartEnabled { get; set; }

        /// <summary>
        /// The type of barrier hazard.
        /// </summary>
        public BarrierHazardType HazardType { get; set; }

        #endregion

        #region Constructors

        public BarrierHazard(Puzzle owner)
            : this(owner, true)
        {
        }

        internal BarrierHazard(Puzzle owner, bool createExtents)
            : base(owner)
        {
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            StartEnabled = true;

            hazardExtentSender = new MultiConnectionPointSender(this);
            hazardExtentSender.Connected += new EventHandler(hazardExtentSender_Connected);

            ConnectionReceiver = new ConnectionLogicReceiver(this);

            if (createExtents)
            {
                Extent min = new Extent(owner) { ExtentType = ExtentType.BarrierHazard };
                Extent max = new Extent(owner) { ExtentType = ExtentType.BarrierHazard };
                owner.Items.Add(min);
                owner.Items.Add(max);
                hazardExtentSender.ConnectTo(min.extentConnectionReceiver);
                hazardExtentSender.ConnectTo(max.extentConnectionReceiver);
            }
        }

        #endregion

        #region Extents

        private void UpdateOffset()
        {
            if (!isOffset)
            {
                localPosition = FPoint3.Zero;
                return;
            }

            // Offset
            Direction forward = ItemFacing.Cross(normal, right);
            if (forward == right)
                return;

            FPoint3 offsetDir = ItemFacing.DirectionToPoint(forward);

            localPosition = offsetDir * 0.375f;
        }

        private void UpdateExtents()
        {
            // Shouldn't udpate extents if normal and right are parallel
            if ((int)normal % 3 == (int)right % 3)
                return;

            // Can't set extents if they don't exist
            if (negExtent == null || posExtent == null)
                return;

            Direction negativeDir = (Direction)((int)right % 3);
            Direction upDir = ItemFacing.Cross(normal, negativeDir);

            negExtent.ItemFacing = new ItemFacing(negativeDir, upDir);
            posExtent.ItemFacing = new ItemFacing(ItemFacing.OppositeDirection(negativeDir), upDir);

            Point3 negative = ItemFacing.DirectionToPoint(negativeDir);
            Point3 positive = -negative;

            negExtent.VoxelPosition = VoxelPosition + negative * negExtentOffset;
            posExtent.VoxelPosition = VoxelPosition + positive * posExtentOffset;
        }

        #endregion

        #region Connection

        void hazardExtentSender_Connected(object sender, EventArgs e)
        {
            // Determine min and max
            Extent min = null;
            Extent max = null;

            foreach (Connection connection in hazardExtentSender.Connections)
            {
                if (connection.Receiver == null)
                    continue;

                Extent extent = connection.Receiver.Owner as Extent;
                if (extent == null)
                    continue;

                // First extent becomes min and max
                if (min == null && max == null)
                {
                    min = extent;
                    max = extent;
                    continue;
                }

                // Check if we have a new min (less than current min)
                if (extent.VoxelPosition.X < min.VoxelPosition.X ||
                    extent.VoxelPosition.Y < min.VoxelPosition.Y ||
                    extent.VoxelPosition.Z < min.VoxelPosition.Z)
                {
                    min = extent;
                    continue;
                }

                // Check if we have a new max (less than or equal to current max)
                if (extent.VoxelPosition.X >= max.VoxelPosition.X ||
                    extent.VoxelPosition.Y >= max.VoxelPosition.Y ||
                    extent.VoxelPosition.Z >= max.VoxelPosition.Z)
                {
                    max = extent;
                    continue;
                }
            }

            // Need to have both extents
            if (min == max)
            {
                negExtent = null;
                posExtent = null;
                return;
            }

            negExtent = min;
            posExtent = max;

            // Update distances
            Point3 negOffset = VoxelPosition - negExtent.VoxelPosition;
            Point3 posOffset = posExtent.VoxelPosition - VoxelPosition;

            negExtentOffset = negOffset.MaxDimension();
            posExtentOffset = posOffset.MaxDimension();
        }

        #endregion

        #region Input / Output

        internal override void FinishReadProperties()
        {
            normal = ItemFacing.Normal;
            right = ItemFacing.Right;
            isOffset = localPosition != FPoint3.Zero;

            base.FinishReadProperties();
        }

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_CONNECTION_COUNT":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_BARRIER_HAZARD_TYPE":
                    HazardType = (BarrierHazardType)property.GetInt32();
                    break;
                case "ITEM_PROPERTY_START_ENABLED":
                    StartEnabled = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", StartEnabled));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_BARRIER_HAZARD_TYPE", (int)HazardType));

            base.AddProperties(node);
        }

        #endregion
    }
}
