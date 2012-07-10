using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// Angle that an AngledPanel extends to.
    /// </summary>
    public enum AngledPanelExtendAngle : int
    {
        Angle30 = 3,
        Angle45 = 2,
        Angle60 = 1,
        Angle90 = 0
    }

    /// <summary>
    /// A panel that extends at an angle.
    /// </summary>
    public class AngledPanel : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_PANEL_ANGLED";
        internal const string TypeNameGlass = "ITEM_PANEL_CLEAR";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        #endregion

        #region Properties

        /// <summary>
        /// Connection receiver to switch this panel on/off.
        /// </summary>
        public ConnectionLogicReceiver ConnectionReceiver { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the angled panel is on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        /// <summary>
        /// The direction the panel will extend towards when deployed.
        /// </summary>
        public Direction ExtendDirection
        {
            get { return ItemFacing.Right; }
            set
            {
                right = ItemFacing.OppositeDirection(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        #endregion

        /// <summary>
        /// The angle this panel will extend to when deployed.
        /// </summary>
        public AngledPanelExtendAngle ExtendAngle { get; set; }

        /// <summary>
        /// Whether or not this panel starts deployed.
        /// </summary>
        public bool StartDeployed { get; set; }

        /// <summary>
        /// Whether or not this panel is portalable (will change the voxel).
        /// </summary>
        public bool IsPortalable
        {
            get { return owner.Voxels.GetVoxel(VoxelPosition).IsPortalable(Wall); }
            set { owner.Voxels.SetPortalable(VoxelPosition, Wall, value); }
        }

        /// <summary>
        /// Whether this panel is glass or solid.
        /// </summary>
        public bool IsGlass
        {
            get { return Type == TypeNameGlass; }
            set { Type = value ? TypeNameGlass : TypeName; }
        }

        #endregion

        #region Constructor

        public AngledPanel(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.X;

            Type = TypeName;
            deletable = true;

            ExtendAngle = AngledPanelExtendAngle.Angle45;
            StartDeployed = false;

            ConnectionReceiver = new ConnectionLogicReceiver(this);
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
                case "ITEM_PROPERTY_CONNECTION_COUNT":
                    // Probably nothing to do here, except maybe verify the connection count matches connections.
                    break;
                case "ITEM_PROPERTY_ANGLED_PANEL_TYPE":
                    ExtendAngle = (AngledPanelExtendAngle)property.GetInt32();
                    break;
                case "ITEM_PROPERTY_START_DEPLOYED":
                    StartDeployed = property.GetBool();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            string animation = "";
            switch (ExtendAngle)
            {
                case AngledPanelExtendAngle.Angle30:
                    animation = "ramp_30_deg_open";
                    break;
                case AngledPanelExtendAngle.Angle45:
                    animation = "ramp_45_deg_open";
                    break;
                case AngledPanelExtendAngle.Angle60:
                    animation = "ramp_60_deg_open";
                    break;
                case AngledPanelExtendAngle.Angle90:
                    animation = "ramp_90_deg_open";
                    break;
            }

            node.Nodes.Add(ConnectionReceiver.GetConnectionCount());
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_ANGLED_PANEL_TYPE", (int)ExtendAngle));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_START_DEPLOYED", StartDeployed));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_PORTALABLE", IsPortalable));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_ANGLED_PANEL_ANIMATION", animation));
            
            base.AddProperties(node);
        }

        #endregion
    }
}
