using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection point that can send multiple connections.
    /// </summary>
    public class MultiConnectionPointSender : ConnectionPointSender
    {
        #region Fields

        List<Connection> connections = new List<Connection>();

        internal List<Connection> Connections { get { return connections; } }

        #endregion

        #region Constructor

        public MultiConnectionPointSender(Item owner)
            : base(owner)
        {
        }

        #endregion

        #region Connection API

        internal override void Connect(Connection connection)
        {
            if (!connections.Contains(connection))
                connections.Add(connection);
        }

        internal override void Disconnect(Connection connection)
        {
            connections.Remove(connection);
        }

        internal void ConnectTo(ConnectionPointReceiver receiver)
        {
            // Potentially no work to do
            if (connections.Find(c => c.Receiver == receiver) != null)
                return;

            Connection.Connect(Owner.Owner, this, receiver);
        }

        /// <summary>
        /// Disconnects this sender from the specified receiver.
        /// </summary>
        protected void Disconnect(ConnectionPointReceiver receiver)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].Receiver != receiver)
                    continue;

                connections[i].Disconnect();
                connections.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Disconnects all receivers.
        /// </summary>
        public void DisconnectAll()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Disconnect();
            }

            connections.Clear();
        }

        #endregion
    }
}
