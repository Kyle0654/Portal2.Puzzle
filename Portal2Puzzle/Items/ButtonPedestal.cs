using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Connections;
using Portal2.DataTypes;

namespace Portal2.Items
{
    /// <summary>
    /// A button that sits on a pedestal and can have a timer.
    /// </summary>
    public class ButtonPedestal : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_BUTTON_PEDESTAL";

        #endregion

        #region Fields

        private Direction normal;
        private Direction right;

        private int timerDelay = 3;

        // TODO: no idea what this is for, but it's output by the puzzle editor
        private int timerSound = 0;

        #endregion

        #region Properties

        /// <summary>
        /// The logic sending connection point.
        /// </summary>
        public ConnectionLogicSender ConnectionSender { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the button is on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                normal = GetNormalFromWall(value);
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        /// <summary>
        /// The side of the wall the button is on.
        /// </summary>
        public Direction PedestalOrientation
        {
            get { return ItemFacing.Right; }
            set
            {
                right = value;
                if ((int)normal % 3 != (int)right % 3)
                    ItemFacing = new ItemFacing(normal, right);
            }
        }

        #endregion

        /// <summary>
        /// How long this button will stay active when pressed (0 for infinite).
        /// </summary>
        public int TimerDelay
        {
            get { return timerDelay; }
            set { if (value >= 0) timerDelay = value; }
        }

        #endregion

        #region Constructor

        public ButtonPedestal(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.NegX;

            Type = TypeName;
            deletable = true;
            Wall = Direction.NegZ;

            ConnectionSender = new ConnectionLogicSender(this);
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            switch (property.Key)
            {
                case "ITEM_PROPERTY_TIMER_DELAY":
                    timerDelay = property.GetInt32();
                    break;
                case "ITEM_PROPERTY_TIMER_SOUND":
                    timerSound = property.GetInt32();
                    break;
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_TIMER_DELAY", timerDelay));
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_TIMER_SOUND", timerSound));

            base.AddProperties(node);
        }

        #endregion
    }
}
