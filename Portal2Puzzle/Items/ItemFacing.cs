using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// Directions in world space.
    /// </summary>
    public enum Direction : int
    {
        NegX = 0,
        NegY = 1,
        NegZ = 2,
        X = 3,
        Y = 4,
        Z = 5
    }

    /// <summary>
    /// The facing of an item.
    /// </summary>
    public struct ItemFacing
    {
        #region Supporting Structs

        private struct InitFacingAngle
        {
            private ItemFacing Facing;
            private Point3 Angle;

            internal InitFacingAngle(ItemFacing facing, Point3 angle)
            {
                Facing = facing;
                Angle = angle;
            }

            public override int GetHashCode()
            {
                return Facing.GetHashCode() | (Angle.GetHashCode() << 6);
            }
        }

        private struct InitDestFacing
        {
            private ItemFacing Init;
            private ItemFacing Dest;

            internal InitDestFacing(ItemFacing init, ItemFacing dest)
            {
                Init = init;
                Dest = dest;
            }

            public override int GetHashCode()
            {
                return Init.GetHashCode() | (Dest.GetHashCode() << 6);
            }
        }

        #endregion

        #region Static Data

        /// <summary>
        /// Normal is represented as "1" within a facing point.
        /// </summary>
        private static Point3[] NormalDirections = new Point3[] {
            -Point3.UnitX,
            -Point3.UnitY,
            -Point3.UnitZ,
            Point3.UnitX,
            Point3.UnitY,
            Point3.UnitZ,
        };

        /// <summary>
        /// Right is represented as "2" within a facing point.
        /// </summary>
        private static Point3[] RightDirections = new Point3[] {
            new Point3(-2, 0, 0),
            new Point3(0, -2, 0),
            new Point3(0, 0, -2),
            new Point3(2, 0, 0),
            new Point3(0, 2, 0),
            new Point3(0, 0, 2)
        };

        static Dictionary<InitFacingAngle, ItemFacing> StartAngleToFacing = new Dictionary<InitFacingAngle, ItemFacing>();
        static Dictionary<InitDestFacing, Point3> StartEndToAngle = new Dictionary<InitDestFacing, Point3>();

        #endregion

        #region Fields

        private Direction startNormal;
        private Direction startRight;

        private Direction normal;
        private Direction right;
        private Point3 angles;

        #endregion

        #region Properties

        /// <summary>
        /// The normal direction of the item.
        /// </summary>
        public Direction Normal { get { return normal; } }

        /// <summary>
        /// The right direction of the item.
        /// </summary>
        public Direction Right { get { return right; } }

        /// <summary>
        /// The rotation angles for the item.
        /// </summary>
        public Point3 Angles { get { return angles; } }

        #endregion

        #region Constructors

        public ItemFacing(Direction normal, Direction right)
        {
            // Normal and right can not be parallel.
            if ((int)normal % 3 == (int)right % 3)
                throw new ArgumentException("Normal and right can not be parallel.");

            this.normal = normal;
            this.right = right;

            // These are pretty much junk values
            startNormal = normal;
            startRight = right;

            angles = Point3.Zero;
        }

        /// <summary>
        /// Create a facing from a start orientation and specified rotation angles.
        /// </summary>
        internal ItemFacing(Direction startNormal, Direction startRight, Point3 angles)
        {
            this.startNormal = startNormal;
            this.startRight = startRight;
            this.angles = angles;

            InitFacingAngle startAngle = new InitFacingAngle(new ItemFacing(startNormal, startRight), angles);
            ItemFacing facing = StartAngleToFacing[startAngle];

            this.normal = facing.normal;
            this.right = facing.right;
        }

        /// <summary>
        /// Create a facing from a start orientation and destination orientation.
        /// </summary>
        internal ItemFacing(Direction startNormal, Direction startRight, Direction destNormal, Direction destRight)
        {
            this.startNormal = startNormal;
            this.startRight = startRight;
            this.normal = destNormal;
            this.right = destRight;

            InitDestFacing startEnd = new InitDestFacing(new ItemFacing(startNormal, startRight), new ItemFacing(destNormal, destRight));
            this.angles = StartEndToAngle[startEnd];
        }

        #endregion

        #region Static API

        static ItemFacing()
        {
            // We calculate all potential start and end facings and associated angles and store them.
            // It's a bit more work on startup, but will save us work later (at the cost of a small amount of memory).
            for (int sn = 0; sn < 6; sn++)
            {
                Direction startNormal = (Direction)sn;
                for (int sr = 0; sr < 6; sr++)
                {
                    // Skip if normal and right are parallel
                    if (sn % 3 == sr % 3)
                        continue;

                    Direction startRight = (Direction)sr;

                    ItemFacing startFacing = new ItemFacing(startNormal, startRight);
                    Point3 startDir = FacingToPoint(startFacing);

                    // Generate the facing to/from angle dictionaries
                    for (int x = 0; x < 4; x++)
                    {
                        int ax = x * 90 - 90;
                        for (int y = 0; y < 4; y++)
                        {
                            int ay = y * 90 - 90;
                            for (int z = 0; z < 4; z++)
                            {
                                int az = z * 90 - 90;

                                Point3 angle = new Point3(ax, ay, az);
                                Point3 endDir = Matrix.Rotate(startDir, angle);
                                ItemFacing endFacing = PointToFacing(endDir);

                                InitFacingAngle startAngleToFacing = new InitFacingAngle(startFacing, angle);
                                InitDestFacing startEndToAngle = new InitDestFacing(startFacing, endFacing);

                                // Add mappings to dictionaries
                                StartAngleToFacing.Add(startAngleToFacing, endFacing);
                                if (!StartEndToAngle.ContainsKey(startEndToAngle))
                                    StartEndToAngle.Add(startEndToAngle, angle);
                            }
                        }
                    }
                }
            }
        }

        static Point3 FacingToPoint(ItemFacing facing)
        {
            return NormalDirections[(int)facing.Normal] + RightDirections[(int)facing.Right];
        }

        static ItemFacing PointToFacing(Point3 point)
        {
            Direction normal;
            Direction right;

            int x = point.X;
            int y = point.Y;
            int z = point.Z;

            if (Math.Abs(x) == 1)
                normal = x > 0 ? Direction.X : Direction.NegX;
            else if (Math.Abs(y) == 1)
                normal = y > 0 ? Direction.Y : Direction.NegY;
            else
                normal = z > 0 ? Direction.Z : Direction.NegZ;

            if (Math.Abs(x) == 2)
                right = x > 0 ? Direction.X : Direction.NegX;
            else if (Math.Abs(y) == 2)
                right = y > 0 ? Direction.Y : Direction.NegY;
            else
                right = z > 0 ? Direction.Z : Direction.NegZ;

            return new ItemFacing(normal, right);
        }

        /// <summary>
        /// Gets the opposite direction to a specified direction.
        /// </summary>
        public static Direction OppositeDirection(Direction direction)
        {
            return (Direction)(((int)direction + 3) % 6);
        }

        /// <summary>
        /// Gets a unit vector in the specified direction.
        /// </summary>
        public static Point3 DirectionToPoint(Direction direction)
        {
            return NormalDirections[(int)direction];
        }

        internal static Direction PointToDirection(Point3 point)
        {
            if (Math.Abs(point.X) != 0)
                return point.X > 0 ? Direction.X : Direction.NegX;
            else if (Math.Abs(point.Y) != 0)
                return point.Y > 0 ? Direction.Y : Direction.NegY;
            else
                return point.Z > 0 ? Direction.Z : Direction.NegZ;
        }

        /// <summary>
        /// Returns the cross-product direction of two directions
        /// </summary>
        public static Direction Cross(Direction a, Direction b)
        {
            // Can't cross parallels
            if ((int)a % 3 == (int)b % 3)
                return a;

            Point3 pa = NormalDirections[(int)a];
            Point3 pb = NormalDirections[(int)b];

            Point3 pc = new Point3(
                pa.Y * pb.Z - pa.Z * pb.Y,
                pa.Z * pb.X - pa.X * pb.Z,
                pa.X * pb.Y - pa.Y * pb.X);

            Direction c = PointToDirection(pc);

            return c;
        }

        #endregion

        #region IEquatable

        public override int GetHashCode()
        {
            return (int)Normal | ((int)Right << 3);
        }

        #endregion
    }
}
