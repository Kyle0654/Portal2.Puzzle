using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// Sends a connection from a logic receiver.
    /// </summary>
    public class ConnectionLogicSender : MultiConnectionPointSender
    {
        #region Constructors

        internal ConnectionLogicSender(Item owner)
            : base(owner)
        {
        }

        #endregion

        #region Connection API

        /// <summary>
        /// Connects to the specified receiver.
        /// </summary>
        public void ConnectTo(ConnectionLogicReceiver receiver)
        {
            base.ConnectTo(receiver);
        }

        /// <summary>
        /// Disconnects from the specified receiver.
        /// </summary>
        public void Disconnect(ConnectionLogicReceiver receiver)
        {
            base.Disconnect(receiver);
        }

        #endregion
    }
}
