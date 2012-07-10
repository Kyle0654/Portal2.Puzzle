using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// The extent type.
    /// </summary>
    public enum ExtentType
    {
        BarrierHazard,
        Barrier,
        PistonPlatform,
        RailPlatform
    }

    /// <summary>
    /// An extent (these should only be modified through the owning item).
    /// </summary>
    public class Extent : Item
    {
        #region Constants

        internal const string BarrierHazardTypeName = "ITEM_BARRIER_HAZARD_EXTENT";
        internal const string BarrierTypeName = "ITEM_BARRIER_EXTENT";
        internal const string PistonPlatformTypeName = "ITEM_PISTON_PLATFORM_EXTENT";
        internal const string RailPlatformTypeName = "ITEM_RAIL_PLATFORM_EXTENT";

        #endregion

        #region Fields

        internal SingleConnectionPointReceiver extentConnectionReceiver;

        #endregion

        #region Properties

        /// <summary>
        /// The type of extent.
        /// </summary>
        public ExtentType ExtentType
        {
            get
            {
                switch (Type)
                {
                    case BarrierHazardTypeName:
                        return ExtentType.BarrierHazard;
                        break;
                    case BarrierTypeName:
                        return ExtentType.Barrier;
                        break;
                    case PistonPlatformTypeName:
                        return ExtentType.PistonPlatform;
                        break;
                    case RailPlatformTypeName:
                        return ExtentType.RailPlatform;
                        break;
                }
                return Items.ExtentType.BarrierHazard;
            }
            set
            {
                switch (value)
                {
                    case ExtentType.BarrierHazard:
                        Type = BarrierHazardTypeName;
                        break;
                    case ExtentType.Barrier:
                        Type = BarrierTypeName;
                        break;
                    case ExtentType.PistonPlatform:
                        Type = PistonPlatformTypeName;
                        defaultNormal = Direction.Z;
                        defaultRight = Direction.Y;
                        break;
                    case ExtentType.RailPlatform:
                        Type = RailPlatformTypeName;
                        defaultNormal = Direction.Z;
                        defaultRight = Direction.Y;
                        break;
                }
            }
        }

        #endregion

        #region Constructors

        internal Extent(Puzzle owner, string typename)
            : this(owner)
        {
            Type = typename;
        }

        public Extent(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.Z;
            defaultNormal = Direction.X;

            Type = BarrierHazardTypeName;
            deletable = false;

            extentConnectionReceiver = new SingleConnectionPointReceiver(this, "CONNECTION_BARRIER_ANCHOR_TO_EXTENT");
        }

        #endregion
    }
}
