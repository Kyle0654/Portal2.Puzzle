using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// Paint type.
    /// </summary>
    public enum PaintType
    {
        Bounce = 0,
        Speed = 1,
        Conversion = 2,
        Cleansing = 3
    }

    /// <summary>
    /// Rate of paint flow.
    /// </summary>
    public enum PaintFlowType
    {
        Light = 0,
        Medium = 1,
        Heavy = 2,
        Drip = 3,
        Bomb = 4
    }

    /// <summary>
    /// Properties of paint/paint dropper.
    /// </summary>
    internal class PaintProperties
    {
        internal PaintType paintType = PaintType.Bounce;
        internal PaintFlowType flowType = PaintFlowType.Medium;
        internal bool dropperVisible = true;
        internal bool startEnabled = true;
        internal bool allowStreaks = true;
    }

    /// <summary>
    /// Paint that is dropped from a paint dropper.
    /// </summary>
    public class Paint : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_PAINT_SPLAT";

        #endregion

        #region Fields

        PaintProperties properties = new PaintProperties();

        internal SingleConnectionPointSender dropperConnectionSender;

        internal bool autoPosition;

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The wall the paint is sitting on.
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

        #region Position

        /// <summary>
        /// Whether or not this paint is placed directly below the dropper.
        /// </summary>
        public bool AutoPosition
        {
            get { return autoPosition; }
            set
            {
                autoPosition = value;

                PaintDropper dropper = Dropper;
                dropper.autoPosition = value;
                SetPositionFromDropper(dropper.VoxelPosition);
            }
        }

        /// <summary>
        /// Gets or sets the position of this paint.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                // If the dropper is right above the paint, move it automatically.
                Point3 offset = Dropper.VoxelPosition - VoxelPosition;

                base.VoxelPosition = value;

                if (!autoPosition || offset.X != 0 || offset.X != 0)
                    return;

                Dropper.SetPositionFromPaint(VoxelPosition);
            }
        }

        #endregion

        /// <summary>
        /// The paint type.
        /// </summary>
        public PaintType PaintType
        {
            get { return properties.paintType; }
            set { properties.paintType = value; }
        }

        /// <summary>
        /// The paint flow type.
        /// </summary>
        public PaintFlowType FlowType
        {
            get { return properties.flowType; }
            set { properties.flowType = value; }
        }

        /// <summary>
        /// Whether or not the paint will create streaks or just a splat.
        /// </summary>
        public bool AllowStreaks
        {
            get { return properties.allowStreaks; }
            set { properties.allowStreaks = value; }
        }

        /// <summary>
        /// Whether or not the dropper is enabled (visible).
        /// </summary>
        public bool DropperVisible
        {
            get { return properties.dropperVisible; }
            set { properties.dropperVisible = value; }
        }

        /// <summary>
        /// Whether or not the dropper will start enabled (dropping paint).
        /// </summary>
        public bool StartEnabled
        {
            get { return properties.startEnabled; }
            set { properties.startEnabled = value; }
        }

        /// <summary>
        /// The paint dropper for this paint.
        /// </summary>
        public PaintDropper Dropper
        {
            get
            {
                ConnectionPointReceiver receiver = dropperConnectionSender.Receiver;
                if (receiver == null)
                    return null;

                return receiver.Owner as PaintDropper;
            }
            internal set
            {
                // Remove properties link from previous dropper
                if (value != null)
                    value.properties = new PaintProperties();

                dropperConnectionSender.Disconnect();

                Connection.Connect(owner, dropperConnectionSender, value.paintConnectionReceiver);
            }
        }

        #endregion

        #region Constructors

        public Paint(Puzzle owner)
            : this(owner, true)
        {
        }

        internal Paint(Puzzle owner, bool createDropper)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
            angles = new Point3(0, 0, 0);

            dropperConnectionSender = new SingleConnectionPointSender(this);
            dropperConnectionSender.Connected += new EventHandler(dropperConnectionSender_Connected);

            if (createDropper)
            {
                PaintDropper dropper = new PaintDropper(owner);
                owner.Items.Add(dropper);
                Dropper = dropper;
                dropper.SetPositionFromPaint(VoxelPosition);
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// Moves the paint to the floor below the paint dropper.
        /// </summary>
        internal void SetPositionFromDropper(Point3 dropperPosition)
        {
            base.VoxelPosition = owner.Voxels.GetFloorVoxel(dropperPosition);
        }

        #endregion

        #region Connection

        void dropperConnectionSender_Connected(object sender, EventArgs e)
        {
            // Link paint properties to connected dropper
            Dropper.properties = properties;
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_PAINT_TYPE":
                    properties.paintType = (PaintType)property.GetInt32();
                    break;
                case "ITEM_PROPERTY_DROPPER_ENABLED":
                    properties.dropperVisible = property.GetBool();
                    break;
                case "ITEM_PROPERTY_FLOW_TYPE":
                    properties.flowType = (PaintFlowType)property.GetInt32();
                    break;
                case "ITEM_PROPERTY_ALLOW_STREAK_PAINT":
                    properties.allowStreaks = property.GetBool();
                    break;
                case "ITEM_PROPERTY_START_ENABLED":
                    properties.startEnabled = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_TYPE", (int)properties.paintType));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_DROPPER_ENABLED", properties.dropperVisible));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_ALLOW_STREAK_PAINT", properties.allowStreaks));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_FLOW_TYPE", (int)properties.flowType));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", properties.startEnabled));

            // TODO: determine what this property is for
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_EXPORT_TYPE", 0));

            base.AddProperties(node);
        }

        #endregion
    }
}
