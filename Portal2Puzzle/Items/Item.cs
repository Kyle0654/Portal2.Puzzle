using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// Represents the forward direction of an item.
    /// </summary>
    public enum ItemAngleFacing : int
    {
        NegX = 0,
        Left = 0,
        PosX = 180,
        Right = 180,
        NegY = 90,
        Back = 90,
        PosY = -90,
        Forward = -90
    }

    /// <summary>
    /// Which face of a voxel an item is placed on.
    /// </summary>
    public enum VoxelFace : int
    {
        NegX = 0,
        NegY = 1,
        NegZ = 2,
        X = 3,
        Y = 4,
        Z = 5
    }

    /// <summary>
    /// Base class for all items.
    /// </summary>
    public class Item
    {
        #region Fields

        protected Puzzle owner;

        // Connections
        private ConnectionPointSender ConnectionSender = null;
        private Dictionary<string, ConnectionPointReceiver> ConnectionReceivers = new Dictionary<string, ConnectionPointReceiver>();

        // These properties may not be important to the specific item type - let the derived type expose if necessary.
        protected bool deletable;
        protected FPoint3 localPosition;

        /// <summary>
        /// Rotations around axis, ordered by Y, Z, X, performed in X, Y, Z order.
        /// </summary>
        protected Point3 angles;
        protected Point3 facing;

        // Default facing
        protected Direction defaultNormal = Direction.Z;
        protected Direction defaultRight = Direction.X;

        /// <summary>
        /// Note - we don't use item index anywhere except when saving.
        /// </summary>
        internal int index;

        /// <summary>
        /// The unrecognized properties of an item, used when the item type isn't recognized.
        /// </summary>
        List<KeyValuePair<string, string>> properties;

        #endregion

        #region Properties

        /// <summary>
        /// The type of the item.
        /// </summary>
        public string Type { get; protected set; }

        /// <summary>
        /// The location of the item.
        /// </summary>
        public virtual Point3 VoxelPosition { get; set; }

        /// <summary>
        /// The owner of this item.
        /// </summary>
        internal Puzzle Owner { get { return owner; } }

        /// <summary>
        /// Gets or sets the facing of this item.
        /// </summary>
        internal ItemFacing ItemFacing
        {
            get { return new ItemFacing(defaultNormal, defaultRight, angles); }
            set
            {
                ItemFacing facing = new ItemFacing(defaultNormal, defaultRight, value.Normal, value.Right);
                angles = facing.Angles;
            }
        }

        #endregion

        #region Constructor

        protected Item(Puzzle owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Gets the wall the item is currently on.
        /// </summary>
        protected Direction GetWall()
        {
            return ItemFacing.OppositeDirection(ItemFacing.Normal );
        }

        /// <summary>
        /// Gets a normal from a specified wall.
        /// </summary>
        protected static Direction GetNormalFromWall(Direction wall)
        {
            return ItemFacing.OppositeDirection(wall);
        }

        #endregion

        #region Connection

        internal void RegisterConnection(ConnectionPointSender sender)
        {
            ConnectionSender = sender;
        }

        internal void RegisterConnection(ConnectionPointReceiver receiver)
        {
            ConnectionReceivers[receiver.ConnectionType] = receiver;
        }

        internal virtual ConnectionPointSender GetConnectionSender()
        {
            return ConnectionSender;
        }

        internal virtual ConnectionPointReceiver GetConnectionReceiver(string connectionType)
        {
            return ConnectionReceivers.ContainsKey(connectionType) ? ConnectionReceivers[connectionType] : null;
        }

        #endregion

        #region Input / Output

        #region Read

        /// <summary>
        /// Creates an item of the specified item type (or a generic item if the type wasn't recognized).
        /// </summary>
        private static Item CreateInstance(Puzzle owner, string type)
        {
            switch (type)
            {
                case AngledPanel.TypeName:
                case AngledPanel.TypeNameGlass:
                    return new AngledPanel(owner) { IsGlass = type == AngledPanel.TypeNameGlass };
                    break;

                case Barrier.TypeName:
                    return new Barrier(owner, false);
                    break;

                case BarrierHazard.TypeName:
                    return new BarrierHazard(owner, false);
                    break;

                case Button.TypeName:
                    return new Button(owner);
                    break;

                case ButtonPedestal.TypeName:
                    return new ButtonPedestal(owner);
                    break;

                case Cube.TypeName:
                    return new Cube(owner, false);
                    break;

                case CubeDropper.TypeName:
                    return new CubeDropper(owner);
                    break;

                case EntryDoor.TypeName:
                    return new EntryDoor(owner);
                    break;

                case ExitDoor.TypeName:
                    return new ExitDoor(owner);
                    break;

                case Extent.BarrierHazardTypeName:
                case Extent.BarrierTypeName:
                case Extent.RailPlatformTypeName:
                    return new Extent(owner, type);
                    break;

                case Extent.PistonPlatformTypeName:
                    return new PistonPlatformExtent(owner);
                    break;

                case FaithPlate.TypeName:
                    return new FaithPlate(owner, false);
                    break;

                case FaithPlateTarget.TypeName:
                    return new FaithPlateTarget(owner);
                    break;

                case FlipPanel.TypeName:
                    return new FlipPanel(owner);
                    break;

                case Goo.TypeName:
                    return new Goo(owner);
                    break;

                case LaserCatcher.TypeName:
                case LaserCatcher.TypeNameOffset:
                    return new LaserCatcher(owner) { IsCentered = type == LaserCatcher.TypeName };
                    break;

                case LaserEmitter.TypeName:
                case LaserEmitter.TypeNameOffset:
                    return new LaserEmitter(owner) { IsCentered = type == LaserEmitter.TypeName };
                    break;

                case LaserRelay.TypeName:
                case LaserRelay.TypeNameOffset:
                    return new LaserRelay(owner) { IsCentered = type == LaserRelay.TypeName };
                    break;

                case LightBridge.TypeName:
                    return new LightBridge(owner);
                    break;

                case LightPanel.TypeName:
                    return new LightPanel(owner);
                    break;

                case ObservationRoom.TypeName:
                    return new ObservationRoom(owner);
                    break;

                case Paint.TypeName:
                    return new Paint(owner, false);
                    break;

                case PaintDropper.TypeName:
                    return new PaintDropper(owner);
                    break;

                case PistonPlatform.TypeName:
                    return new PistonPlatform(owner, false);
                    break;

                case SecondaryObservationRoom.TypeName:
                    return new SecondaryObservationRoom(owner);
                    break;

                case Stairs.TypeName:
                    return new Stairs(owner);
                    break;

                case TractorBeam.TypeName:
                    return new TractorBeam(owner);
                    break;

                case Turret.TypeName:
                    return new Turret(owner);
                    break;

                default:
                    Item item = new Item(owner);
                    item.properties = new List<KeyValuePair<string, string>>();
                    return item;
                    break;
            }
        }

        /// <summary>
        /// Reads the properties in the node into the item.
        /// </summary>
        private static void ReadProperties(P2CNode node, Item item)
        {
            foreach (IP2CNode child in node.Nodes)
            {
                P2CProperty property = child as P2CProperty;

                // TODO: handle child nodes?
                if (property == null)
                    continue;

                switch (property.Key)
                {
                    case "Index":
                        item.index = property.GetInt32();
                        break;
                    case "Type":
                        item.Type = property.Value;
                        break;
                    case "Deletable":
                        item.deletable = property.GetBool();
                        break;
                    case "VoxelPos":
                        item.VoxelPosition = property.GetPoint3();
                        break;
                    case "LocalPos":
                        item.localPosition = property.GetFPoint3();
                        break;
                    case "Angles":
                        item.angles = FromAngleProperty(property.GetPoint3());
                        break;
                    case "Facing": // TODO: this needs to be doubles instead of ints
                        item.facing = property.GetPoint3();
                        break;
                    default:
                        item.ReadProperty(property);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Reads a property that wasn't one of the default item properties.
        /// </summary>
        internal virtual void ReadProperty(P2CProperty property)
        {
            if (properties == null)
                return;

            properties.Add(new KeyValuePair<string, string>(property.Key, property.Value));
        }

        /// <summary>
        /// Called when all properties have been read, so any initialization can happen.
        /// </summary>
        internal virtual void FinishReadProperties()
        {
        }

        /// <summary>
        /// Reads a P2CNode for an item.
        /// </summary>
        internal static Item ReadItem(Puzzle owner, P2CNode node)
        {
            if (node.Key != "Item")
                throw new ArgumentException("Specified node is not a Item");

            Item item = CreateInstance(owner, node.GetProperty("Type").Value);
            ReadProperties(node, item);

            return item;
        }

        #endregion

        #region Write

        /// <summary>
        /// Derived classes can override this to prepare data before saving.
        /// </summary>
        internal virtual void PrepareGetNode()
        {
        }

        /// <summary>
        /// Derived classes should add all non-default item properties to the node here.
        /// </summary>
        internal virtual void AddProperties(P2CNode node)
        {
            if (properties == null)
                return;

            foreach (KeyValuePair<string, string> prop in properties)
            {
                node.Nodes.Add(new P2CProperty(prop.Key, prop.Value));
            }
        }

        /// <summary>
        /// Gets a P2CNode for saving.
        /// </summary>
        internal P2CNode GetNode()
        {
            PrepareGetNode();

            P2CNode node = new P2CNode("Item");

            node.Nodes.Add(new P2CProperty("Index", index));
            node.Nodes.Add(new P2CProperty("Type", Type));
            node.Nodes.Add(new P2CProperty("Deletable", deletable));
            node.Nodes.Add(new P2CProperty("VoxelPos", VoxelPosition));
            node.Nodes.Add(new P2CProperty("LocalPos", localPosition));
            node.Nodes.Add(new P2CProperty("Angles", ToAngleProperty(angles)));
            node.Nodes.Add(new P2CProperty("Facing", facing));

            AddProperties(node);

            return node;
        }

        #endregion

        #region Angle coordinate order conversion

        /// <summary>
        /// Output in YZX order.
        /// </summary>
        static Point3 ToAngleProperty(Point3 angles)
        {
            return new Point3(angles.Y, angles.Z, angles.X);
        }

        /// <summary>
        /// Read from YZX order to XYZ order.
        /// </summary>
        static Point3 FromAngleProperty(Point3 prop)
        {
            return new Point3(prop.Z, prop.X, prop.Y);
        }

        #endregion

        #endregion

        public override string ToString()
        {
            return string.Format("[{0}] {1}", index, Type);
        }
    }
}
