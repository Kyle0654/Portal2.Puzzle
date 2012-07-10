using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;
using Portal2.DataTypes;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection point that can receive multiple connections.
    /// </summary>
    public class MultiConnectionPointReceiver : ConnectionPointReceiver
    {
        #region Fields

        List<Connection> connections = new List<Connection>();

        internal List<Connection> Connections { get { return connections; } }

        #endregion

        #region Constructor

        internal MultiConnectionPointReceiver(Item owner, string connectionType)
            : base(owner, connectionType)
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

        protected void ConnectTo(ConnectionPointSender sender)
        {
            // Potentially no work to do
            if (connections.Find(c => c.Sender == sender) != null)
                return;

            Connection.Connect(Owner.Owner, sender, this);
        }

        /// <summary>
        /// Disconnects this receiver from the specified sender.
        /// </summary>
        protected void Disconnect(ConnectionPointSender sender)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].Sender != sender)
                    continue;

                connections[i].Disconnect();
                connections.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Disconnects all senders.
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

        #region Input / Output

        internal P2CProperty GetConnectionCount()
        {
            return GetConnectionCount("ITEM_PROPERTY_CONNECTION_COUNT");
        }

        internal P2CProperty GetConnectionCount(string propertyName)
        {
            return new P2CProperty(propertyName, connections.Count);
        }

        #endregion
    }
}
