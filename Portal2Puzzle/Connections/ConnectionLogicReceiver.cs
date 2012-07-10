using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// Receives a connection from a logic sender.
    /// </summary>
    public class ConnectionLogicReceiver : MultiConnectionPointReceiver
    {
        #region Constructors

        internal ConnectionLogicReceiver(Item owner)
            : base(owner, "CONNECTION_STANDARD")
        {
        }

        internal ConnectionLogicReceiver(Item owner, string connectionType)
            : base(owner, connectionType)
        {
        }

        #endregion

        #region Connection API

        /// <summary>
        /// Connects to the specified sender.
        /// </summary>
        public void ConnectTo(ConnectionLogicSender sender)
        {
            base.ConnectTo(sender);
        }

        /// <summary>
        /// Disconnects from the specified sender.
        /// </summary>
        public void Disconnect(ConnectionLogicSender sender)
        {
            base.Disconnect(sender);
        }

        #endregion
    }
}
