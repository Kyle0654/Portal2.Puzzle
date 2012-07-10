using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection point that can receive a single connection.
    /// </summary>
    public class SingleConnectionPointReceiver : ConnectionPointReceiver
    {
        #region Fields

        protected Connection connection;

        #endregion

        #region Properties

        internal Connection Connection { get { return connection; } }

        internal ConnectionPointSender Sender
        {
            get { return connection == null ? null : connection.Sender; }
            set { ConnectTo(value); }
        }

        #endregion

        #region Constructor

        internal SingleConnectionPointReceiver(Item owner, string connectionType)
            : base(owner, connectionType)
        {
        }

        #endregion

        #region Connection API

        internal override void Connect(Connection connection)
        {
            this.connection = connection;
        }

        internal override void Disconnect(Connection connection)
        {
            if (this.connection == connection)
                this.connection = null;
        }

        protected void ConnectTo(ConnectionPointSender sender)
        {
            // Potentially no work to do
            if (connection != null && connection.Sender == sender)
                return;

            Disconnect();

            Connection.Connect(Owner.Owner, sender, this);
        }

        /// <summary>
        /// Disconnects this receiver from the current sender.
        /// </summary>
        public void Disconnect()
        {
            if (connection != null)
                connection.Disconnect();
        }

        #endregion
    }
}
