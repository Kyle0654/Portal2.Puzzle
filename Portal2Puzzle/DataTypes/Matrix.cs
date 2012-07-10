using System;
using System.Collections.Generic;
using System.Text;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A 3x3 integer matrix, used primarily for rotation calculations.
    /// </summary>
    internal struct Matrix
    {
        #region Static Values

        internal static Matrix Identity { get { return new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1); } }

        #endregion

        #region Static Data

        static Matrix[] rotx = new Matrix[] {
                new Matrix(1, 0, 0,  0, 0, 1,  0, -1, 0),
                Matrix.Identity,
                new Matrix(1, 0, 0,  0, 0, -1,  0, 1, 0),
                new Matrix(1, 0, 0,  0, -1, 0,  0, 0, -1)
            };

        static Matrix[] roty = new Matrix[] {
                new Matrix(0, 0, -1,  0, 1, 0,  1, 0, 0),
                Matrix.Identity,
                new Matrix(0, 0, 1,  0, 1, 0,  -1, 0, 0),
                new Matrix(-1, 0, 0,  0, 1, 0,  0, 0, -1)
            };

        static Matrix[] rotz = new Matrix[] {
                new Matrix(0, 1, 0,  -1, 0, 0,  0, 0, 1),
                Matrix.Identity,
                new Matrix(0, -1, 0,  1, 0, 0,  0, 0, 1),
                new Matrix(-1, 0, 0,  0, -1, 0,  0, 0, 1)
            };

        #endregion

        #region Fields

        internal int m11, m12, m13, m21, m22, m23, m31, m32, m33;

        #endregion

        #region Constructor

        internal Matrix(
            int m11, int m12, int m13,
            int m21, int m22, int m23,
            int m31, int m32, int m33)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        #endregion

        #region Static API

        /// <summary>
        /// Multiplies the point by the specified matrix.
        /// </summary>
        internal static Point3 Multiply(Matrix m, Point3 p)
        {
            return new Point3(
                m.m11 * p.X + m.m12 * p.Y + m.m13 * p.Z,
                m.m21 * p.X + m.m22 * p.Y + m.m23 * p.Z,
                m.m31 * p.X + m.m32 * p.Y + m.m33 * p.Z);
        }

        /// <summary>
        /// Rotates the specified point by the specified rotation (in x, y, z order, angles must be increments of 90).
        /// </summary>
        internal static Point3 Rotate(Point3 point, Point3 rotation)
        {
            int ax = rotation.X;
            int ay = rotation.Y;
            int az = rotation.Z;

            while (ax > 180) ax -= 360;
            while (ax < -90) ax += 360;
            while (ay > 180) ay -= 360;
            while (ay < -90) ay += 360;
            while (az > 180) az -= 360;
            while (az < -90) az += 360;

            // rotations can only be in 90 degree increments, and will be clamped to the nearest 90 degrees
            int rx = ((ax + 90) / 90);
            int ry = ((ay + 90) / 90);
            int rz = ((az + 90) / 90);

            Point3 result = Matrix.Multiply(rotx[rx], point);
            result = Matrix.Multiply(roty[ry], result);
            result = Matrix.Multiply(rotz[rz], result);

            return result;
        }

        /// <summary>
        /// Reverses the specified rotation (multiply by inverses in inverse order).
        /// </summary>
        internal static Point3 Unrotate(Point3 point, Point3 rotation)
        {
            int ax = -rotation.X;
            int ay = -rotation.Y;
            int az = -rotation.Z;

            while (ax > 180) ax -= 360;
            while (ax < -90) ax += 360;
            while (ay > 180) ay -= 360;
            while (ay < -90) ay += 360;
            while (az > 180) az -= 360;
            while (az < -90) az += 360;

            // rotations can only be in 90 degree increments, and will be clamped to the nearest 90 degrees
            int rx = ((ax + 90) / 90);
            int ry = ((ay + 90) / 90);
            int rz = ((az + 90) / 90);

            Point3 result = Matrix.Multiply(rotz[rz], point);
            result = Matrix.Multiply(roty[ry], result);
            result = Matrix.Multiply(rotx[rx], result);

            return result;
        }

        #endregion
    }
}
