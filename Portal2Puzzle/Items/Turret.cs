using System;
using System.Collections.Generic;
using System.Text;

namespace Portal2.Items
{
    /// <summary>
    /// A turret that will target a player entering its line of sight.
    /// </summary>
    /// <remarks>Hello? Are you there?</remarks>
    public class Turret : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_TURRET";

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The direction the turret is facing, with 0 at Negative X, rotating counter-clockwise around Z.
        /// </summary>
        public int FacingAngle
        {
            get { return facing.Y; }
            set
            {
                int angle = value;
                while (angle <= -180)
                    angle += 360;
                while (angle > 180)
                    angle -= 360;

                facing.Y = angle;
            }
        }

        #endregion

        #endregion

        #region Constructor

        public Turret(Puzzle owner)
            : base(owner)
        {
            Type = TypeName;
            deletable = true;
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Sets the facing to a specified direction.
        /// </summary>
        public void SetFacingDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.NegX:
                    facing.Y = 0;
                    break;
                case Direction.NegY:
                    facing.Y = -90;
                    break;
                case Direction.X:
                    facing.Y = 180;
                    break;
                case Direction.Y:
                    facing.Y = 90;
                    break;
            }
        }

        #endregion
    }
}
