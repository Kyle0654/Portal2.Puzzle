using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A point with 3 floating-point coordinates.
    /// </summary>
    public struct FPoint3 : IEquatable<FPoint3>
    {
        #region Static Values

        public static FPoint3 Zero
        {
            get { return new FPoint3(0, 0, 0); }
        }

        public static FPoint3 UnitX
        {
            get { return new FPoint3(1, 0, 0); }
        }

        public static FPoint3 UnitY
        {
            get { return new FPoint3(0, 1, 0); }
        }

        public static FPoint3 UnitZ
        {
            get { return new FPoint3(0, 0, 1); }
        }

        #endregion

        #region Fields

        public float X;
        public float Y;
        public float Z;

        #endregion

        #region Constructor

        public FPoint3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create from a string of the format "x y z"
        /// </summary>
        public FPoint3(string point)
        {
            if (string.IsNullOrEmpty(point))
                throw new ArgumentNullException("point", "Null or empty point string specified");

            string[] coords = point.Split(' ');
            if (coords.Length != 3)
                throw new ArgumentException("Point must be of the format \"x y z\"", "point");

            if (!float.TryParse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out X))
                throw new ArgumentException("X coordinate is invalid: \"" + coords[0] + "\"", "point");

            if (!float.TryParse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out Y))
                throw new ArgumentException("Y coordinate is invalid: \"" + coords[1] + "\"", "point");

            if (!float.TryParse(coords[2], NumberStyles.Float, CultureInfo.InvariantCulture, out Z))
                throw new ArgumentException("Z coordinate is invalid: \"" + coords[2] + "\"", "point");
        }

        #endregion

        #region Operators

        public static FPoint3 operator +(FPoint3 a, FPoint3 b)
        {
            return new FPoint3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static FPoint3 operator -(FPoint3 a, FPoint3 b)
        {
            return new FPoint3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static FPoint3 operator *(FPoint3 a, float b)
        {
            return new FPoint3(a.X * b, a.Y * b, a.Z * b);
        }

        public static FPoint3 operator /(FPoint3 a, float b)
        {
            return new FPoint3(a.X / b, a.Y / b, a.Z / b);
        }

        public static FPoint3 operator -(FPoint3 a)
        {
            return new FPoint3(-a.X, -a.Y, -a.Z);
        }

        public static bool operator ==(FPoint3 a, FPoint3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(FPoint3 a, FPoint3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public static implicit operator Point3(FPoint3 a)
        {
            return new Point3((int)a.X, (int)a.Y, (int)a.Z);
        }

        #endregion

        #region IEquatable

        public bool Equals(FPoint3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FPoint3))
                return false;

            FPoint3 other = (FPoint3)obj;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            // TODO: Make a better hash code?
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X.ToString("0.###", CultureInfo.InvariantCulture), Y.ToString("0.###", CultureInfo.InvariantCulture), Z.ToString("0.###", CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
