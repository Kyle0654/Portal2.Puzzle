using System;
using System.Collections.Generic;
using System.Text;
using Portal2.DataTypes;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// Button type.
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// A standard circular button.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// A button activated by a cube.
        /// </summary>
        Cube = 1,

        /// <summary>
        /// A button activated by a sphere.
        /// </summary>
        Sphere = 2
    }

    /// <summary>
    /// A button that can activate other items.
    /// </summary>
    public class Button : Item
    {
        #region Constants

        internal const string TypeName = "ITEM_BUTTON_FLOOR";

        #endregion

        #region Properties

        /// <summary>
        /// The logic sending connection point.
        /// </summary>
        public ConnectionLogicSender ConnectionSender { get; private set; }

        #region Orientation

        /// <summary>
        /// The wall the button is sitting on.
        /// </summary>
        public Direction Wall
        {
            get { return GetWall(); }
            set
            {
                Direction normal = GetNormalFromWall(value);
                ItemFacing = new ItemFacing(normal, (Direction)(((int)normal + 1) % 6));
            }
        }

        #endregion

        /// <summary>
        /// The type of the button.
        /// </summary>
        public ButtonType ButtonType { get; set; }

        #endregion

        #region Constructors

        public Button(Puzzle owner)
            : base(owner)
        {
            defaultRight = Direction.X;

            Type = TypeName;
            deletable = true;
            Wall = Direction.NegZ;
            ButtonType = ButtonType.Standard;

            ConnectionSender = new ConnectionLogicSender(this);
        }

        #endregion

        #region Input / Output

        internal override void ReadProperty(P2CProperty property)
        {
            if (property.Key == "ITEM_PROPERTY_BUTTON_TYPE")
            {
                ButtonType = (ButtonType)property.GetInt32();
            }

            base.ReadProperty(property);
        }

        internal override void AddProperties(P2CNode node)
        {
            node.Nodes.Add(new P2CProperty("ITEM_PROPERTY_BUTTON_TYPE", (int)ButtonType));

            base.AddProperties(node);
        }

        #endregion
    }
}
