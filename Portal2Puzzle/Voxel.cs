using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Items;

namespace Portal2
{
    /// <summary>
    /// Data of a voxel in bitwise combinations. Combine with logical or.
    /// </summary>
    [Flags]
    public enum VoxelData : byte
    {
        None = 0,
        NotSolid = 0,

        /// <summary>
        /// All sides are portalable by default, and the voxel is solid.
        /// </summary>
        Default = 15,

        NegPortalable = 7,
        PosPortalable = 112,
        AllPortalable = 119,
        IsPortalable = 119,
        NotPortalable = 0,

        IsPortalableNegX = 1,
        IsPortalableLeft = 1,
        IsPortalableNegY = 2,
        IsPortalableBack = 2,
        IsPortalableNegZ = 4,
        IsPortalableBottom = 4,
        IsSolid = 8,
        All = 15,

        // Special logical values
        IsPortalableX = 16,
        IsPortalableRight = 16,
        IsPortalableY = 32,
        IsPortalableForward = 32,
        IsPortalableZ = 64,
        IsPortalableTop = 64,
    }

    /// <summary>
    /// A location in a level with three portalable sides that can be solid or not.
    /// </summary>
    public struct Voxel : IEquatable<Voxel>
    {
        #region Static Values

        /// <summary>
        /// The empty voxel (at position -1, -1, -1)
        /// </summary>
        public static Voxel Empty { get { return new Voxel(null, new Point3(-1, -1, -1)); } }

        #endregion

        #region Fields

        VoxelField parent;

        Point3 position;
        VoxelData data;

        #endregion

        #region Properties

        /// <summary>
        /// The field containing the voxel.
        /// </summary>
        public VoxelField Parent { get { return parent; } }

        /// <summary>
        /// The position of the voxel within the field.
        /// </summary>
        public Point3 Position { get { return position; } }

        /// <summary>
        /// Whether or not the voxel is solid.
        /// </summary>
        public bool IsSolid
        {
            get { return (data & VoxelData.IsSolid) == VoxelData.IsSolid; }
            internal set { SetData(VoxelData.IsSolid, value ? VoxelData.IsSolid : VoxelData.None); }
        }
        
        /// <summary>
        /// Whether or not the -X (left) side of the voxel is portalable or not. 
        /// </summary>
        public bool IsPortalableNegX
        {
            get { return (data & VoxelData.IsPortalableNegX) == VoxelData.IsPortalableNegX; }
            internal set { SetData(VoxelData.IsPortalableNegX, value ? VoxelData.IsPortalableNegX : VoxelData.None); }
        }

        /// <summary>
        /// Whether or not the -Y (back/near) side of the voxel is portalable or not. 
        /// </summary>
        public bool IsPortalableNegY
        {
            get { return (data & VoxelData.IsPortalableNegY) == VoxelData.IsPortalableNegY; }
            internal set { SetData(VoxelData.IsPortalableNegY, value ? VoxelData.IsPortalableNegY : VoxelData.None); }
        }

        /// <summary>
        /// Whether or not the -Z (bottom) side of the voxel is portalable or not. 
        /// </summary>
        public bool IsPortalableNegZ
        {
            get { return (data & VoxelData.IsPortalableNegZ) == VoxelData.IsPortalableNegZ; }
            internal set { SetData(VoxelData.IsPortalableNegZ, value ? VoxelData.IsPortalableNegZ : VoxelData.None); }
        }

        /// <summary>
        /// Whether or not the X (right) side of the voxel is portalable or not.
        /// </summary>
        /// <remarks>This will set the left side of the voxel to the right of the current voxel.</remarks>
        public bool IsPortalableX
        {
            get { return parent.GetVoxel(position + Point3.UnitX).IsPortalableNegX; }
            internal set { parent.SetVoxelData(position + Point3.UnitX, VoxelData.IsPortalableNegX, value ? VoxelData.IsPortalableNegX : VoxelData.None); }
        }

        /// <summary>
        /// Whether or not the Y (forward/far) side of the voxel is portalable or not.
        /// </summary>
        /// <remarks>This will set the back side of the voxel to the front of the current voxel.</remarks>
        public bool IsPortalableY
        {
            get { return parent.GetVoxel(position + Point3.UnitY).IsPortalableNegY; }
            internal set { parent.SetVoxelData(position + Point3.UnitY, VoxelData.IsPortalableNegY, value ? VoxelData.IsPortalableNegY : VoxelData.None); }
        }

        /// <summary>
        /// Whether or not the Z (top) side of the voxel is portalable or not.
        /// </summary>
        /// <remarks>This will set the bottom side of the voxel to the top of the current voxel.</remarks>
        public bool IsPortalableZ
        {
            get { return parent.GetVoxel(position + Point3.UnitZ).IsPortalableNegZ; }
            internal set { parent.SetVoxelData(position + Point3.UnitZ, VoxelData.IsPortalableNegZ, value ? VoxelData.IsPortalableNegZ : VoxelData.None); }
        }

        #endregion

        #region Constructors

        internal Voxel(VoxelField parent, Point3 position)
        {
            this.parent = parent;
            this.position = position;
            data = VoxelData.Default;
        }

        /// <summary>
        /// Creates a voxel from a p2c Voxel node.
        /// </summary>
        /// <exception cref="ArgumentException">The specified node is not a Voxel.</exception>
        /// <exception cref="NullReferenceException">A piece of data was missing in the node.</exception>
        internal Voxel(VoxelField parent, P2CNode node)
        {
            this.parent = parent;

            if (node.Key != "Voxel")
                throw new ArgumentException("Specified node is not a Voxel");

            position = node.GetProperty("Position").GetPoint3();

            VoxelData value = VoxelData.None;
            if (node.GetProperty("Solid").GetBool()) value |= VoxelData.IsSolid;
            if (node.GetProperty("Portal0").GetBool()) value |= VoxelData.IsPortalableNegX;
            if (node.GetProperty("Portal1").GetBool()) value |= VoxelData.IsPortalableNegY;
            if (node.GetProperty("Portal2").GetBool()) value |= VoxelData.IsPortalableNegZ;
            data = value;
        }

        #endregion

        #region Methods

        internal void SetData(VoxelData mask, VoxelData value)
        {
            if ((mask & VoxelData.IsPortalableX) == VoxelData.IsPortalableX)
                IsPortalableX = ((value & VoxelData.IsPortalableX) == VoxelData.IsPortalableX);

            if ((mask & VoxelData.IsPortalableY) == VoxelData.IsPortalableY)
                IsPortalableY = ((value & VoxelData.IsPortalableY) == VoxelData.IsPortalableY);

            if ((mask & VoxelData.IsPortalableZ) == VoxelData.IsPortalableZ)
                IsPortalableZ = ((value & VoxelData.IsPortalableZ) == VoxelData.IsPortalableZ);

            mask &= VoxelData.All;
            data = (data & ~mask) | (value & mask);
        }

        internal void SetData(Voxel other)
        {
            SetData(VoxelData.All, other.data);
        }

        /// <summary>
        /// Creates a P2CNode representing this voxel.
        /// </summary>
        internal P2CNode GetNode()
        {
            P2CNode node = new P2CNode("Voxel");
            node.Nodes.Add(new P2CProperty("Position", Position));
            node.Nodes.Add(new P2CProperty("Solid", IsSolid));
            node.Nodes.Add(new P2CProperty("Portal0", IsPortalableNegX));
            node.Nodes.Add(new P2CProperty("Portal1", IsPortalableNegY));
            node.Nodes.Add(new P2CProperty("Portal2", IsPortalableNegZ));
            return node;
        }

        #endregion

        #region Portalable

        internal bool IsPortalable(Direction direction)
        {
            switch (direction)
            {
                case Direction.NegX:
                    return IsPortalableNegX;
                    break;
                case Direction.NegY:
                    return IsPortalableNegY;
                    break;
                case Direction.NegZ:
                    return IsPortalableNegZ;
                    break;
                case Direction.X:
                    return IsPortalableX;
                    break;
                case Direction.Y:
                    return IsPortalableY;
                    break;
                case Direction.Z:
                    return IsPortalableZ;
                    break;
            }

            return false;
        }

        internal void SetPortalable(Direction direction, bool value)
        {
            switch (direction)
            {
                case Direction.NegX:
                    IsPortalableNegX = value;
                    break;
                case Direction.NegY:
                    IsPortalableNegY = value;
                    break;
                case Direction.NegZ:
                    IsPortalableNegZ = value;
                    break;
                case Direction.X:
                    IsPortalableX = value;
                    break;
                case Direction.Y:
                    IsPortalableY = value;
                    break;
                case Direction.Z:
                    IsPortalableZ = value;
                    break;
            }
        }

        #endregion

        #region Operators

        public static bool operator==(Voxel a, Voxel b)
        {
            return a.position == b.position && a.data == b.data;
        }

        public static bool operator !=(Voxel a, Voxel b)
        {
            return a.position != b.position || a.data != b.data;
        }

        #endregion

        #region IEquatable

        public bool Equals(Voxel other)
        {
            return position == other.position && data == other.data;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Voxel))
                return false;

            Voxel other = (Voxel)obj;

            return position == other.position && data == other.data;
        }

        public override int GetHashCode()
        {
            return (position.GetHashCode() << 8) | (int)data;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}) {3}", position.X, position.Y, position.Z, parent);
        }

        #endregion
    }
}
