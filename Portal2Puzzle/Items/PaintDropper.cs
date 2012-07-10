using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// A dropper that drops paint.
    /// </summary>
    public class PaintDropper : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_DROPPER_PAINT";

        #endregion

        #region Fields

        internal PaintProperties properties = new PaintProperties();

        internal SingleConnectionPointReceiver paintConnectionReceiver;

        internal bool autoPosition = false;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this dropper on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the dropper is sitting on.
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
        /// Whether or not this dropper is placed directly above the paint.
        /// </summary>
        public bool AutoPosition
        {
            get { return autoPosition; }
            set
            {
                autoPosition = value;

                Paint paint = Paint;
                paint.autoPosition = value;
                SetPositionFromPaint(paint.VoxelPosition);
            }
        }

        /// <summary>
        /// Gets or sets the position of this paint dropper.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                // If the paint is right below the dropper, move it automatically.
                Point3 offset = Paint.VoxelPosition - VoxelPosition;

                base.VoxelPosition = value;

                if (!autoPosition || offset.X != 0 || offset.X != 0)
                    return;

                Paint.SetPositionFromDropper(VoxelPosition);
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
        /// The paint for this paint dropper.
        /// </summary>
        public Paint Paint
        {
            get
            {
                ConnectionPointSender sender = paintConnectionReceiver.Sender;
                if (sender == null)
                    return null;

                return sender.Owner as Paint;
            }
            internal set
            {
                // Disconnect properties link
                properties = new PaintProperties();

                paintConnectionReceiver.Disconnect();

                Connection.Connect(owner, value.dropperConnectionSender, paintConnectionReceiver);
            }
        }

        #endregion

        #region Constructor

        internal PaintDropper(Puzzle owner)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
            ItemFacing = new ItemFacing(Direction.NegZ, Direction.X);
            paintConnectionReceiver = new SingleConnectionPointReceiver(this, "CONNECTION_PAINT_DROPPER");
            ConnectionReceiver = new ConnectionLogicReceiver(this);
        }

        #endregion

        #region Position

        /// <summary>
        /// Moves the paint dropper to the ceiling above the paint.
        /// </summary>
        internal void SetPositionFromPaint(Point3 paintPosition)
        {
            base.VoxelPosition = owner.Voxels.GetCeilingVoxel(paintPosition);
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
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_TYPE", (int)properties.paintType));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_DROPPER_ENABLED", properties.dropperVisible));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_ALLOW_STREAK_PAINT", properties.allowStreaks));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_FLOW_TYPE", (int)properties.flowType));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_ENABLED", properties.startEnabled));

            // TODO: determine what this property is for
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PAINT_EXPORT_TYPE", 4));

            base.AddProperties(node);
        }

        #endregion
    }
}
