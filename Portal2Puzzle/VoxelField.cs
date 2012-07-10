using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Items;

namespace Portal2
{
    /// <summary>
    /// A field of voxels that makes up a level.
    /// </summary>
    public class VoxelField
    {
        #region Fields

        Puzzle owner;

        int width = 0;
        int depth = 0;
        int height = 0;

        /// <summary>
        /// The voxels, stored in x y z order
        /// </summary>
        private Voxel[] voxels;

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the field will automatically grow to fit any queried or set values
        /// </summary>
        public bool AutoGrow { get; set; }

        /// <summary>
        /// The width of the voxel field (in the X/Right direction)
        /// </summary>
        public int Width
        {
            get { return width; }
            set { Resize(value, depth, height); }
        }

        /// <summary>
        /// The depth of the voxel field (in the Y/Forward direction)
        /// </summary>
        public int Depth
        {
            get { return depth; }
            set { Resize(width, value, height); }
        }

        /// <summary>
        /// The height of the voxel field (in the Z/Up direction)
        /// </summary>
        public int Height
        {
            get { return height; }
            set { Resize(width, depth, value); }
        }

        #endregion

        #region Construction

        public VoxelField(Puzzle owner)
        {
            this.owner = owner;
            AutoGrow = false;
            Point3 size = owner.ChamberSize;
            Resize(size.X, size.Y, size.Z);
        }

        internal VoxelField(Puzzle owner, P2CNode voxelsNode)
        {
            this.owner = owner;
            AutoGrow = false;
            Point3 size = owner.ChamberSize;
            Resize(size.X + 1, size.Y + 1, size.Z + 1);

            if (voxelsNode == null)
                return;

            List<P2CNode> voxelNodes = voxelsNode.GetNodes("Voxel");
            foreach (P2CNode voxelNode in voxelNodes)
            {
                Voxel voxel = new Voxel(this, voxelNode);
                SetVoxel(voxel.Position, voxel);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the voxel at the specified position.
        /// </summary>
        public Voxel GetVoxel(Point3 position)
        {
            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            if (x < 0 || y < 0 || z < 0)
                return Voxel.Empty;

            if (x >= width || y >= depth || z >= height)
            {
                if (!AutoGrow)
                    return Voxel.Empty;

                Resize(Math.Max(x + 1, width), Math.Max(y + 1, depth), Math.Max(z + 1, height));
            }

            return voxels[(x * depth * height) + (y * height) + z];
        }

        /// <summary>
        /// Copies the voxel data to the voxel at the specified position.
        /// </summary>
        internal void SetVoxel(Point3 position, Voxel voxel)
        {
            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            if (x < 0 || y < 0 || z < 0)
                return;

            if (x >= width || y >= depth || z >= height)
            {
                if (!AutoGrow)
                    return;

                Resize(Math.Max(x + 1, width), Math.Max(y + 1, depth), Math.Max(z + 1, height));
            }

            voxels[(x * depth * height) + (y * height) + z].SetData(voxel);
        }

        internal void SetPortalable(Point3 position, Direction direction, bool portalable)
        {
            voxels[(position.X * depth * height) + (position.Y * height) + position.Z].SetPortalable(direction, portalable);
        }

        /// <summary>
        /// Sets the specified voxel data on the voxel at the specified position.
        /// </summary>
        public void SetVoxelData(Point3 position, VoxelData mask, VoxelData data)
        {
            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            if (x < 0 || y < 0 || z < 0)
                return;

            if (x >= width || y >= depth || z >= height)
            {
                if (!AutoGrow)
                    return;

                Resize(Math.Max(x + 1, width), Math.Max(y + 1, depth), Math.Max(z + 1, height));
            }

            voxels[(x * depth * height) + (y * height) + z].SetData(mask, data);
        }

        /// <summary>
        /// Resizes the voxel field.
        /// </summary>
        internal void Resize(int sx, int sy, int sz)
        {
            if (sx <= 1 || sy <= 1 || sz <= 1)
                return;

            if (sx == width && sy == depth && sz == height)
                return;

            int planewidth = sy * sz;
            int rowwidth = sz;

            Voxel[] newArray = new Voxel[sx * sy * sz];

            // Fill out new array
            for (int x = 0; x < sx; x++)
            {
                for (int y = 0; y < sy; y++)
                {
                    int rowOffset = x * (sy * sz) + y * (sz);

                    // Copy values that will fit in new array
                    // Fill out rest
                    if (voxels != null && x < width && y < depth)
                    {
                        Array.Copy(voxels, x * (depth * height) + y * (height),
                                   newArray, rowOffset, height);
                    }

                    // Fill out rest of values
                    int minz = (x < width && y < depth) ? height : 0;

                    for (int z = minz; z < sz; z++)
                    {
                        newArray[rowOffset + z] = new Voxel(this, new Point3(x, y, z));
                    }

                    if (x == sx - 1 || y == sy - 1)
                    {
                        // Set all data on last row to solid
                        for (int z = 0; z < sz; z++)
                        {
                            newArray[rowOffset + z].SetData(VoxelData.IsSolid, VoxelData.IsSolid);
                        }
                    }
                    else
                    {
                        // Set last voxel on row to solid
                        newArray[rowOffset + sz - 1].SetData(VoxelData.IsSolid, VoxelData.IsSolid);
                    }
                }
            }

            // Set array
            voxels = newArray;
            width = sx;
            depth = sy;
            height = sz;
        }

        /// <summary>
        /// Sets a range of data.
        /// </summary>
        public void SetRange(Point3 position, Point3 size, VoxelData mask, VoxelData values)
        {
            // Grow size if necessary
            Point3 max = position + size + Point3.One;
            if (AutoGrow && (max.X > width || max.Y > depth || max.Z > height))
            {
                Resize(Math.Max(max.X, width), Math.Max(max.Y, depth), Math.Max(max.Z, height));
            }

            // TODO: If setting positive portalable values on a range, try to overlap the data to reduce property access.

            // Set values
            for (int dx = 0; dx < size.X; dx++)
            {
                int x = dx + position.X;
                if (x >= width)
                    break;

                int ox = x * (depth * height);

                for (int dy = 0; dy < size.Y; dy++)
                {
                    int y = dy + position.Y;
                    if (y >= depth)
                        break;

                    int oy = y * (height);

                    for (int dz = 0; dz < size.Z; dz++)
                    {
                        int z = dz + position.Z;
                        if (z >= height)
                            break;

                        int i = ox + oy + z;
                        voxels[i].SetData(mask, values);
                    }
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the position of the last non-solid voxel directly above the specified position.
        /// </summary>
        public Point3 GetCeilingVoxel(Point3 location)
        {
            Point3 pos = location;
            while (!GetVoxel(pos + Point3.UnitZ).IsSolid)
            {
                pos.Z += 1;
            }
            return pos;
        }

        /// <summary>
        /// Gets the position of the last non-solid voxel directly below the specified position.
        /// </summary>
        public Point3 GetFloorVoxel(Point3 location)
        {
            Point3 pos = location;
            while (!GetVoxel(pos - Point3.UnitZ).IsSolid)
            {
                pos.Z -= 1;
            }
            return pos;
        }

        #endregion

        #region I/O Operations

        /// <summary>
        /// Gets a P2CNode representing the voxel field.
        /// </summary>
        internal P2CNode GetNode()
        {
            P2CNode voxelsNode = new P2CNode("Voxels");
            for (int x = 0; x < width; x++)
            {
                int ox = x * depth * height;
                for (int y = 0; y < depth; y++)
                {
                    int oy = y * height;
                    for (int z = 0; z < height; z++)
                    {
                        int i = ox + oy + z;
                        int ni1 = (x <= 0) ? -1 : ((x - 1) * depth * height) + oy + z;
                        int ni2 = (y <= 0) ? -1 : ox + ((y - 1) * height) + z;
                        int ni3 = (z <= 0) ? -1 : ox + oy + (z - 1);

                        // Skip any empty voxels that have all empty negative neighbors
                        if (!voxels[i].IsSolid &&
                            x != width - 1 && y != depth - 1 && z != height - 1 &&
                            ni1 != -1 && ni2 != -1 && ni3 != -1 &&
                            !voxels[ni1].IsSolid &&
                            !voxels[ni2].IsSolid &&
                            !voxels[ni3].IsSolid)
                            continue;

                        voxelsNode.Nodes.Add(voxels[i].GetNode());
                    }
                }
            }
            return voxelsNode;
        }

        #endregion
    }
}
