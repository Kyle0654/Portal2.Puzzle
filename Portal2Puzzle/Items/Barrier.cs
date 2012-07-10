using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// Barrier type.
    /// </summary>
    public enum BarrierType : int
    {
        Glass = 0,
        Grating = 1
    }

    /// <summary>
    /// A barrier that blocks an area.
    /// </summary>
    public class Barrier : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_BARRIER";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        private int leftExtentOffset = 0;
        private int rightExtentOffset = 0;
        private int upExtentOffset = 0;
        
        private Extent leftExtent;
        private Extent rightExtent;
        private Extent upExtent;

        private MultiConnectionPointSender barrierExtentSender;

        #endregion

        #region Properties

        /// <summary>
        /// The type of barrier.
        /// </summary>
        public BarrierType BarrierType { get; set; }

        #region Orientation

        /// <summary>
        /// The direction the barrier is facing.
        /// </summary>
        public Direction Facing
        {
            get { return normal; }
            set
            {
                normal = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateExtents();
            }
        }

        /// <summary>
        /// The right of the barrier.
        /// </summary>
        public Direction Right
        {
            get { return ItemFacing.Right; }
            set
            {
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateExtents();
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// The position of this barrier.
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
        /// Gets or sets the left extent.
        /// </summary>
        public int LeftExtent
        {
            get { return leftExtentOffset; }
            set
            {
                leftExtentOffset = value;
                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the upward extent.
        /// </summary>
        public int UpExtent
        {
            get { return upExtentOffset; }
            set
            {
                upExtentOffset = value;
                UpdateExtents();
            }
        }

        /// <summary>
        /// Gets or sets the right extent.
        /// </summary>
        public int RightExtent
        {
            get { return rightExtentOffset; }
            set
            {
                rightExtentOffset = value;
                UpdateExtents();
            }
        }

        #endregion

        #endregion

        #region Constructors

        public Barrier(Puzzle owner)
            : this(owner, true)
        {
        }

        internal Barrier(Puzzle owner, bool createExtents)
            : base(owner)
        {
            defaultNormal = Direction.NegX;
            defaultRight = Direction.Y;

            Type = TypeName;
            deletable = true;

            barrierExtentSender = new MultiConnectionPointSender(this);
            barrierExtentSender.Connected += new EventHandler(barrierExtentSender_Connected);

            if (createExtents)
            {
                Extent leftext = new Extent(owner) { ExtentType = ExtentType.Barrier };
                Extent upext = new Extent(owner) { ExtentType = ExtentType.Barrier };
                Extent rightext = new Extent(owner) { ExtentType = ExtentType.Barrier };
                owner.Items.Add(leftext);
                owner.Items.Add(upext);
                owner.Items.Add(rightext);
                barrierExtentSender.ConnectTo(leftext.extentConnectionReceiver);
                barrierExtentSender.ConnectTo(upext.extentConnectionReceiver);
                barrierExtentSender.ConnectTo(rightext.extentConnectionReceiver);
            }
        }

        #endregion

        #region Extents

        private void UpdateExtents()
        {
            // Shouldn't udpate extents if normal and right are parallel
            if ((int)normal % 3 == (int)right % 3)
                return;

            // Can't set extents if they don't exist
            if (leftExtent == null || upExtent == null || rightExtent == null)
                return;

            Direction upDir = ItemFacing.Cross(right, normal);
            Point3 rightPoint = ItemFacing.DirectionToPoint(right);
            Point3 leftPoint = -rightPoint;
            Point3 upPoint = ItemFacing.DirectionToPoint(upDir);

            leftExtent.ItemFacing = new ItemFacing(ItemFacing.OppositeDirection(right), ItemFacing.OppositeDirection(normal));
            upExtent.ItemFacing = new ItemFacing(upDir, ItemFacing.OppositeDirection(normal));
            rightExtent.ItemFacing = new ItemFacing(right, ItemFacing.OppositeDirection(normal));

            leftExtent.VoxelPosition = VoxelPosition + leftPoint * leftExtentOffset;
            upExtent.VoxelPosition = VoxelPosition + upPoint * upExtentOffset;
            rightExtent.VoxelPosition = VoxelPosition + rightPoint * rightExtentOffset;
        }

        #endregion

        #region Connection

        void barrierExtentSender_Connected(object sender, EventArgs e)
        {
            // Determine min and max
            Extent leftext = null;
            Extent rightext = null;
            Extent upext = null;

            foreach (Connection connection in barrierExtentSender.Connections)
            {
                if (connection.Receiver == null)
                    continue;

                Extent extent = connection.Receiver.Owner as Extent;
                if (extent == null)
                    continue;

                if (leftext == null)
                {
                    leftext = extent;
                    continue;
                }

                if (upext == null)
                {
                    upext = extent;
                    continue;
                }

                if (rightext == null)
                {
                    rightext = extent;
                    break;
                }
            }

            // Need to have all extents
            if (leftext == null || rightext == null || upext == null)
            {
                leftExtent = null;
                rightExtent = null;
                upExtent = null;
                return;
            }

            leftExtent = leftext;
            rightExtent = rightext;
            upExtent = upext;

            // Update distances
            Point3 leftOffset = VoxelPosition - leftExtent.VoxelPosition;
            Point3 rightOffset = rightExtent.VoxelPosition - VoxelPosition;
            Point3 upOffset = upExtent.VoxelPosition - VoxelPosition;

            leftExtentOffset = leftOffset.MaxDimension();
            rightExtentOffset = rightOffset.MaxDimension();
            upExtentOffset = upOffset.MaxDimension();
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
                case "ITEM_PROPERTY_BARRIER_TYPE":
                    BarrierType = (BarrierType)property.GetInt32();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_BARRIER_TYPE", (int)BarrierType));

            base.AddProperties(node);
        }

        #endregion
    }
}
