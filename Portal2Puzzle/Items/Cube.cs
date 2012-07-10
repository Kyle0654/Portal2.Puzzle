using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// Cube type.
    /// </summary>
    public enum CubeType
    {
        Standard = 0,
        Companion = 1,
        Reflective = 2,
        Sphere = 3,
        Franken = 4
    }

    /// <summary>
    /// Properties of a cube/cube dropper.
    /// </summary>
    internal class CubeProperties
    {
        internal CubeType cubeType = CubeType.Standard;
        internal bool dropperVisible = true;
        internal bool autoDrop = true;
        internal bool autoRespawn = true;
    }

    /// <summary>
    /// A cube.
    /// </summary>
    public class Cube : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_CUBE";

        #endregion

        #region Fields

        CubeProperties properties = new CubeProperties();

        internal SingleConnectionPointSender dropperConnectionSender;

        #endregion

        #region Properties

        #region Position

        /// <summary>
        /// The position of this cube.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                base.VoxelPosition = value;
                CubeDropper dropper = Dropper;
                if (dropper != null)
                    dropper.SetPositionFromCube(value);
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
        /// The cube dropper for this cube.
        /// </summary>
        public CubeDropper Dropper
        {
            get
            {
                ConnectionPointReceiver receiver = dropperConnectionSender.Receiver;
                if (receiver == null)
                    return null;

                return receiver.Owner as CubeDropper;
            }
            internal set
            {
                // Remove properties link from previous dropper
                if (value != null)
                    value.properties = new CubeProperties();

                dropperConnectionSender.Disconnect();

                Connection.Connect(owner, dropperConnectionSender, value.cubeConnectionReceiver);
            }
        }

        #endregion

        #region Constructors

        public Cube(Puzzle owner)
            : this(owner, true)
        {
        }

        internal Cube(Puzzle owner, bool createDropper)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
            angles = new Point3(0, 0, 0);

            dropperConnectionSender = new SingleConnectionPointSender(this);
            dropperConnectionSender.Connected += new EventHandler(dropperConnectionSender_Connected);

            if (createDropper)
            {
                CubeDropper dropper = new CubeDropper(owner);
                owner.Items.Add(dropper);
                Dropper = dropper;
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// Moves the cube to the floor below the cube dropper.
        /// </summary>
        internal void SetPositionFromDropper(Point3 dropperPosition)
        {
            base.VoxelPosition = owner.Voxels.GetFloorVoxel(dropperPosition);
        }

        #endregion

        #region Connection

        void dropperConnectionSender_Connected(object sender, EventArgs e)
        {
            // Link cube properties to connected dropper
            Dropper.properties = properties;
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
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_AUTO_DROP_CUBE", properties.autoDrop));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_AUTO_RESPAWN_CUBE", properties.autoRespawn));

            base.AddProperties(node);
        }

        #endregion
    }
}
