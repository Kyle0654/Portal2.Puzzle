using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection point that sends a single connection.
    /// </summary>
    public class SingleConnectionPointSender : ConnectionPointSender
    {
        #region Fields

        protected Connection connection;

        #endregion

        #region Properties

        internal Connection Connection { get { return connection; } }

        internal ConnectionPointReceiver Receiver
        {
            get { return connection == null ? null : connection.Receiver; }
            set { ConnectTo(value); }
        }

        #endregion

        #region Constructor

        public SingleConnectionPointSender(Item owner)
            : base(owner)
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

        protected void ConnectTo(ConnectionPointReceiver receiver)
        {
            // Potentially no work to do
            if (connection != null && connection.Receiver == receiver)
                return;

            Disconnect();

            Connection.Connect(Owner.Owner, this, receiver);
        }

        /// <summary>
        /// Disconnects this sender from the current receiver.
        /// </summary>
        public void Disconnect()
        {
            if (connection != null)
                connection.Disconnect();
        }

        #endregion
    }
}
