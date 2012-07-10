using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A faith plate target.
    /// </summary>
    public class FaithPlateTarget : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_CATAPULT_TARGET";

        #endregion

        #region Fields

        internal SingleConnectionPointReceiver launcherConnectionReceiver;

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The wall the target is sitting on.
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
        /// Gets or sets the location of this target.
        /// </summary>
        public override Point3 VoxelPosition
        {
            get { return base.VoxelPosition; }
            set
            {
                base.VoxelPosition = value;
                FaithPlate launcher = Launcher;
                if (launcher != null)
                    launcher.UpdateFacing();
            }
        }

        #endregion

        /// <summary>
        /// Gets the launcher
        /// </summary>
        public FaithPlate Launcher
        {
            get
            {
                ConnectionPointSender sender = launcherConnectionReceiver.Sender;
                if (sender == null)
                    return null;

                return sender.Owner as FaithPlate;
            }
        }

        #endregion

        #region Constructor

        internal FaithPlateTarget(Puzzle owner)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
            ItemFacing = new ItemFacing(Direction.NegZ, Direction.X);

            launcherConnectionReceiver = new SingleConnectionPointReceiver(this, "CONNECTION_CATAPULT_TARGET");
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_VERTICAL_ALIGNMENT":
                    // This is determined by position of launcher and target - don't need to read it
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            FaithPlate launcher = Launcher;
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_VERTICAL_ALIGNMENT", launcher != null && launcher.IsVertical));

            base.AddProperties(node);
        }

        #endregion
    }
}
