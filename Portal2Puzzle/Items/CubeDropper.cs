using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// A dropper that drops a cube.
    /// </summary>
    public class CubeDropper : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_DROPPER_CUBE";

        #endregion

        #region Fields

        internal CubeProperties properties = new CubeProperties();

        internal SingleConnectionPointReceiver cubeConnectionReceiver;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this dropper on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Position

        /// <summary>
        /// The position of this cube dropper.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                base.VoxelPosition = value;
                Cube cube = Cube;
                if (cube != null)
                    cube.SetPositionFromDropper(value);
            }
        }

        #endregion

        /// <summary>
        /// The type of this cube.
        /// </summary>
        public CubeType CubeType
        {
            get { return properties.cubeType; }
            set { properties.cubeType = value; }
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
        /// Whether or not the dropper will automatically spawn a cube.
        /// </summary>
        public bool AutoDrop
        {
            get { return properties.autoDrop; }
            set { properties.autoDrop = value; }
        }

        /// <summary>
        /// Whether or not the dropper will automatically respawn cubes.
        /// </summary>
        public bool AutoRespawn
        {
            get { return properties.autoRespawn; }
            set { properties.autoRespawn = value; }
        }

        /// <summary>
        /// The cube for this cube dropper.
        /// </summary>
        public Cube Cube
        {
            get
            {
                ConnectionPointSender sender = cubeConnectionReceiver.Sender;
                if (sender == null)
                    return null;

                return sender.Owner as Cube;
            }
            internal set
            {
                // Disconnect properties link
                properties = new CubeProperties();

                cubeConnectionReceiver.Disconnect();

                Connection.Connect(owner, value.dropperConnectionSender, cubeConnectionReceiver);
            }
        }

        #endregion

        #region Constructor

        internal CubeDropper(Puzzle owner)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
            ItemFacing = new ItemFacing(Direction.NegZ, Direction.X);
            cubeConnectionReceiver = new SingleConnectionPointReceiver(this, "CONNECTION_BOX_DROPPER");
            ConnectionReceiver = new ConnectionLogicReceiver(this);
        }

        #endregion

        #region Position

        /// <summary>
        /// Moves the dropper to the ceiling above the cube.
        /// </summary>
        internal void SetPositionFromCube(Point3 cubePosition)
        {
            base.VoxelPosition = owner.Voxels.GetCeilingVoxel(cubePosition);
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_CUBE_TYPE":
                    properties.cubeType = (CubeType)property.GetInt32();
                    break;
                case "ITEM_PROPERTY_DROPPER_ENABLED":
                    properties.dropperVisible = property.GetBool();
                    break;
                case "ITEM_PROPERTY_AUTO_DROP_CUBE":
                    properties.autoDrop = property.GetBool();
                    break;
                case "ITEM_PROPERTY_AUTO_RESPAWN_CUBE":
                    properties.autoRespawn = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_CUBE_TYPE", (int)properties.cubeType));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_DROPPER_ENABLED", properties.dropperVisible));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_DROPPER_FALL_STRAIGHT_DOWN", true));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_AUTO_DROP_CUBE", properties.autoDrop));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_AUTO_RESPAWN_CUBE", properties.autoRespawn));
            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());

            base.AddProperties(node);
        }

        #endregion
    }
}
