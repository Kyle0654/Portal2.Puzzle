using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A point with 3 integer coordinates.
    /// </summary>
    public struct Point3 : IEquatable<Point3>
    {
        #region Static Values

        public static Point3 Zero
        {
            get { return new Point3(0, 0, 0); }
        }

        public static Point3 One
        {
            get { return new Point3(1, 1, 1); }
        }

        public static Point3 UnitX
        {
            get { return new Point3(1, 0, 0); }
        }

        public static Point3 UnitY
        {
            get { return new Point3(0, 1, 0); }
        }

        public static Point3 UnitZ
        {
            get { return new Point3(0, 0, 1); }
        }

        #endregion

        #region Fields

        public int X;
        public int Y;
        public int Z;

        #endregion

        #region Constructors

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create from a string of the format "x y z"
        /// </summary>
        public Point3(string point)
        {
            if (string.IsNullOrEmpty(point))
                throw new ArgumentNullException("point", "Null or empty point string specified");

            string[] coords = point.Split(' ');
            if (coords.Length != 3)
                throw new ArgumentException("Point must be of the format \"x y z\"", "point");

            if (!int.TryParse(coords[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out X))
                throw new ArgumentException("X coordinate is invalid: \"" + coords[0] + "\"", "point");

            if (!int.TryParse(coords[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out Y))
                throw new ArgumentException("Y coordinate is invalid: \"" + coords[1] + "\"", "point");

            if (!int.TryParse(coords[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out Z))
                throw new ArgumentException("Z coordinate is invalid: \"" + coords[2] + "\"", "point");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the absolute maximum value in any dimension.
        /// </summary>
        public int MaxDimension()
        {
            return Math.Max(Math.Abs(X), Math.Max(Math.Abs(Y), Math.Abs(Z)));
        }

        #endregion

        #region Operators

        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3 operator -(Point3 a, Point3 b)
        {
            return new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3 operator *(Point3 a, int b)
        {
            return new Point3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Point3 operator /(Point3 a, int b)
        {
            return new Point3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Point3 operator -(Point3 a)
        {
            return new Point3(-a.X, -a.Y, -a.Z);
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Point3 a, Point3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public static implicit operator FPoint3(Point3 a)
        {
            return new FPoint3((float)a.X, (float)a.Y, (float)a.Z);
        }

        #endregion

        #region IEquatable

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point3))
                return false;

            Point3 other = (Point3)obj;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return X ^ (Y << 8) ^ (Z << 16);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X.ToString(CultureInfo.InvariantCulture), Y.ToString(CultureInfo.InvariantCulture), Z.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
