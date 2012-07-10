using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using System.IO;

namespace Portal2.Items
{
    /// <summary>
    /// Whether the light is on the inside or outside.
    /// </summary>
    public enum LightOffsetAmount
    {
        Near,
        Far
    }

    /// <summary>
    /// A thin panel of light.
    /// </summary>
    public class LightPanel : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_LIGHT_PANEL";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;
        private LightOffsetAmount offsetAmount;

        #endregion

        #region Properties

        #region Orientation

        /// <summary>
        /// The wall the light is placed on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateOffset();
            }
        }

        /// <summary>
        /// The right of the barrier hazard.
        /// </summary>
        public Direction OffsetDirection
        {
            get { return ItemFacing.Right; }
            set
            {
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);

                UpdateOffset();
            }
        }

        public LightOffsetAmount OffsetAmount
        {
            get { return offsetAmount; }
            set
            {
                offsetAmount = value;
                UpdateOffset();
            }
        }

        #endregion

        #endregion

        #region Constructor

        public LightPanel(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.NegX;

            Type = TypeName;
            deletable = true;

            OffsetAmount = LightOffsetAmount.Near;
        }

        #endregion

        #region Offset

        private void UpdateOffset()
        {
            // Offset
            FPoint3 offsetDir = ItemFacing.DirectionToPoint(right);
            localPosition = offsetDir * ((offsetAmount == LightOffsetAmount.Near) ? 0.125f : 0.375f);
        }

        #endregion

        #region Input / Output

        internal override void FinishReadProperties()
        {
            normal = ItemFacing.Normal;
            right = ItemFacing.Right;

            // Get offset
            float x = Math.Abs(localPosition.X);
            float y = Math.Abs(localPosition.Y);
            float z = Math.Abs(localPosition.Z);

            float v = Math.Max(x, Math.Max(y, z));

            offsetAmount = (v == 0.125f) ? LightOffsetAmount.Near : LightOffsetAmount.Far;

            base.FinishReadProperties();
        }

        #endregion
    }
}
