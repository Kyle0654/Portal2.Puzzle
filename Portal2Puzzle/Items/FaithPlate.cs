using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// A faith plate launcher.
    /// </summary>
    public class FaithPlate : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_CATAPULT";

        #endregion

        #region Fields

        private double arcHeight = 3.0;

        internal SingleConnectionPointSender targetConnectionSender;

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The wall the faith plate is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                Direction normal = GetNormalFromWall(value);
                ItemFacing = new ItemFacing(normal, (Direction)(((int)normal + 1) % 6));
                UpdateFacing();
            }
        }

        #endregion

        #region Position

        /// <summary>
        /// Gets or sets the location of this faith plate.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                base.VoxelPosition = value;
                UpdateFacing();
            }
        }

        #endregion

        /// <summary>
        /// The height of the top of the launch arc (must be at least the height difference between the launcher and target).
        /// </summary>
        public double ArcHeight
        {
            get { return arcHeight; }
            set
            {
                arcHeight = value;
                UpdateFacing();
            }
        }

        /// <summary>
        /// Gets the target
        /// </summary>
        public FaithPlateTarget Target
        {
            get
            {
                ConnectionPointReceiver receiver = targetConnectionSender.Receiver;
                if (receiver == null)
                    return null;

                return receiver.Owner as FaithPlateTarget;
            }
            internal set
            {
                targetConnectionSender.Disconnect();
                Connection.Connect(owner, targetConnectionSender, value.launcherConnectionReceiver);
            }
        }

        /// <summary>
        /// Returns whether or not the target is positioned directly in line with the faith plate's normal.
        /// </summary>
        internal bool IsVertical
        {
            get
            {
                FaithPlateTarget target = Target;

                if (target == null)
                    return false;

                Point3 pos = target.VoxelPosition - VoxelPosition;
                Direction normalDir = (Direction)((int)ItemFacing.Normal % 3);

                // NOTE: While horizontal launches will *look* like straight lines, they behave like normal launches.
                return (normalDir == Direction.NegZ && pos.X == 0 && pos.Y == 0);
            }
        }

        #endregion

        #region Constructors

        public FaithPlate(Puzzle owner)
            : this(owner, true)
        {
        }

        internal FaithPlate(Puzzle owner, bool createTarget)
            : base(owner)
        {
            defaultNormal = Direction.Z;
            defaultRight = Direction.NegX;

            Type = TypeName;
            deletable = true;

            targetConnectionSender = new SingleConnectionPointSender(this);

            if (createTarget)
            {
                FaithPlateTarget target = new FaithPlateTarget(owner);
                owner.Items.Add(target);
                Target = target;
            }

            Wall = Direction.NegZ;
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Updates the facing toward the target direction.
        /// </summary>
        internal void UpdateFacing()
        {
            // Get direciton to target
            FaithPlateTarget target = Target;
            Point3 dirVector = target.VoxelPosition - VoxelPosition;
            int x = dirVector.X;
            int y = dirVector.Y;
            int z = dirVector.Z;

            // Determine best facing direction
            Direction n = (Direction)((int)ItemFacing.Normal % 3);
            Direction right = Direction.NegY;
            if (n == Direction.NegX || n == Direction.NegY)
            {
                right = (arcHeight >= 0 || z >= 0) ? Direction.Z : Direction.NegZ;
            }
            else
            {
                if (Math.Abs(x) >= Math.Abs(y))
                {
                    right = (x > 0) ? Direction.X : Direction.NegX;
                }
                else
                {
                    right = (y > 0) ? Direction.Y : Direction.NegY;
                }
            }

            ItemFacing = new ItemFacing(ItemFacing.Normal, right);
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_CATAPULT_SPEED":
                    // TODO: Read this and do something with it?
                    break;
                case "ITEM_PROPERTY_TARGET_NAME":
                    // TODO: Read this and do something with it?
                    break;
                case "ITEM_PROPERTY_VERTICAL_ALIGNMENT":
                    // This is determined by position of launcher and target - don't need to read it
                    break;
                case "CatapultHeight":
                    arcHeight = property.GetDouble();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            // TODO: Catapult speed controls the initial display of the arc in the editor, but is not used
            //       when compiling a puzzle. It will look wrong in the editor until touched, but will work
            //       fine in a compiled level.
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_CATAPULT_SPEED", 900.000000));

            // TODO: This always seems to be 0. Find any situations where it's not?
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_TARGET_NAME", 0));

            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_VERTICAL_ALIGNMENT", IsVertical));
            node.Nodes.Add(new P2CProperty("CatapultHeight", arcHeight));

            base.AddProperties(node);
        }

        #endregion
    }
}
