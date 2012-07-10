using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using System.IO;
using Portal2.Items;

namespace Portal2
{
    /// <summary>
    /// A Portal 2 puzzle.
    /// </summary>
    public class Puzzle
    {
        #region Fields

        P2CFile file;

        int AppID = 644;
        int Version = 12;
        string FileID = "0x0000000000000000";

        DateTime Created;
        DateTime Modified;
        double CompileTime;

        int PreviewDirty = 0;
        Point3 chamberSize = Point3.Zero;

        // Voxels
        VoxelField voxels;

        // Items
        List<Item> items = new List<Item>();

        // Connections
        List<Connection> connections = new List<Connection>();

        #endregion

        #region Properties

        /// <summary>
        /// The title of the puzzle.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the puzzle.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The size of the chamber.
        /// </summary>
        public Point3 ChamberSize
        {
            get
            {
                return (voxels == null) ? chamberSize :
                    new Point3(voxels.Width - 1, voxels.Depth - 1, voxels.Height - 1);
            }
            set
            {
                chamberSize = value;
                if (voxels != null)
                {
                    voxels.Resize(value.X + 1, value.Y + 1, value.Z + 1);
                }
            }
        }

        /// <summary>
        /// Gets the voxel field for this puzzle.
        /// </summary>
        public VoxelField Voxels
        {
            get { return voxels; }
        }

        /// <summary>
        /// Gets the items list for this puzzle.
        /// </summary>
        public List<Item> Items
        {
            get { return items; }
        }

        /// <summary>
        /// Gets the connections list for this puzzle.
        /// </summary>
        public List<Connection> Connections
        {
            get { return connections; }
        }

        #endregion

        #region Constructors

        public Puzzle()
        {
            InitializeNew();
        }

        /// <summary>
        /// Create a puzzle from a P2CFile.
        /// </summary>
        internal Puzzle(P2CFile file)
        {
            this.file = file;
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Missing expected property in puzzle file", e);
            }
        }

        #endregion

        #region Static API

        /// <summary>
        /// Opens a puzzle file.
        /// </summary>
        public static Puzzle Open(string filename)
        {
            P2CFile p2c = P2CFile.Open(filename);
            return new Puzzle(p2c);
        }

        #endregion

        #region Intialization

        /// <summary>
        /// Initializes a new puzzle with default values.
        /// </summary>
        private void InitializeNew()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
            CompileTime = 15.000;

            Title = "My awesome puzzle";
            Description = "An awesome puzzle I made";

            chamberSize = new Point3(1, 1, 1);
            voxels = new VoxelField(this);
        }

        /// <summary>
        /// Initializes the puzzle when read from a file.
        /// </summary>
        /// <exception cref="NullReferenceException">One of the properties was not found.</exception>
        private void Initialize()
        {
            P2CNode root = file.Root;

            // Read AppID and Version, make sure they match default values
            // (Note: if not, it means the format has potentially changed, and this app needs to be updated)
            int appID = root.GetProperty("AppID").GetInt32();
            if (AppID != appID)
            {
                throw new InvalidDataException(string.Format("The AppID \"{0}\" does not match the expected AppID \"{1}\".", appID, appID));
            }

            int version = root.GetProperty("Version").GetInt32();
            if (AppID != appID)
            {
                throw new InvalidDataException(string.Format("The Version \"{0}\" does not match the expected Version \"{1}\". Lab Rat may need to be updated.", version, Version));
            }

            // Puzzle properties
            FileID = root.GetProperty("FileID").Value;
            Created = root.GetProperty("Timestamp_Created").GetDateTime();
            Modified = root.GetProperty("Timestamp_Modified").GetDateTime();
            CompileTime = root.GetProperty("CompileTime").GetDouble();
            Title = root.GetProperty("Title").Value;
            Description = root.GetProperty("Description").Value;
            PreviewDirty = root.GetProperty("PreviewDirty").GetInt32();
            ChamberSize = root.GetProperty("ChamberSize").GetPoint3();

            // Voxels
            P2CNode voxelsNode = root.GetNode("Voxels");
            if (voxelsNode != null)
            {
                voxels = new VoxelField(this, voxelsNode);
            }

            // Items
            P2CNode itemsNode = root.GetNode("Items");
            if (itemsNode != null)
            {
                items.AddRange(itemsNode.GetNodes("Item").ConvertAll<Item>(n => Item.ReadItem(this, n)));
            }

            // Connections
            P2CNode connectionsNode = root.GetNode("Connections");
            if (connectionsNode != null)
            {
                connections.AddRange(connectionsNode.GetNodes("Connection").ConvertAll<Connection>(n => Connection.ReadConnection(this, n)));
            }
        }

        #endregion

        #region Input / Output

        /// <summary>
        /// Save the puzzle to the specified filename.
        /// </summary>
        public void Save(string filename)
        {
            Modified = DateTime.Now;
            P2CFile file = new P2CFile(GetNode());
            file.Save(filename);
        }

        /// <summary>
        /// Get a P2CNode representing the puzzle.
        /// </summary>
        private P2CNode GetNode()
        {
            P2CNode node = new P2CNode("portal2_puzzle");

            // Puzzle properties
            node.Nodes.Add(new P2CProperty("AppID", AppID));
            node.Nodes.Add(new P2CProperty("Version", Version));
            node.Nodes.Add(new P2CProperty("FileID", FileID));
            node.Nodes.Add(new P2CProperty("Timestamp_Created", Created));
            node.Nodes.Add(new P2CProperty("Timestamp_Modified", Modified));
            node.Nodes.Add(new P2CProperty("CompileTime", CompileTime));
            node.Nodes.Add(new P2CProperty("Title", Title));
            node.Nodes.Add(new P2CProperty("Description", Description));
            node.Nodes.Add(new P2CProperty("PreviewDirty", PreviewDirty));
            node.Nodes.Add(new P2CProperty("ChamberSize", ChamberSize));

            // Voxels
            if (voxels != null)
            {
                node.Nodes.Add(voxels.GetNode());
            }

            // Items
            if (items.Count != 0)
            {
                // Set index on all items
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].index = i;
                }

                P2CNode itemsNode = new P2CNode("Items");
                itemsNode.Nodes.AddRange(items.ConvertAll<IP2CNode>(i => i.GetNode()));
                node.Nodes.Add(itemsNode);
            }

            // Connections
            if (connections.Count != 0)
            {
                P2CNode connectionsNode = new P2CNode("Connections");
                connectionsNode.Nodes.AddRange(connections.ConvertAll<IP2CNode>(c => c.GetNode()));
                node.Nodes.Add(connectionsNode);
            }

            return node;
        }

        #endregion
    }
}
