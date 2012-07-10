using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;
using Portal2.DataTypes;
using System.IO;
using Portal2.Connections;

namespace Portal2.Items
{
    /// <summary>
    /// A connection between two items.
    /// </summary>
    public class Connection
    {
        #region Fields

        ConnectionPointSender sender;
        ConnectionPointReceiver receiver;

        Puzzle owner;

        #endregion

        #region Properties

        /// <summary>
        /// The type of connection.
        /// </summary>
        public string Type { get; private set; }

        internal ConnectionPointSender Sender
        {
            get { return sender; }
            set { SetSender(value); }
        }

        internal ConnectionPointReceiver Receiver
        {
            get { return receiver; }
            set { SetReceiver(value); }
        }

        #endregion

        #region Constructor

        protected Connection(Puzzle owner, string type)
        {
            Type = type;
            this.owner = owner;
        }

        #endregion

        #region Methods

        private void RemoveSender()
        {
            if (sender == null)
                return;

            sender.Disconnect(this);
            sender = null;
        }

        private void RemoveReceiver()
        {
            if (receiver == null)
                return;

            receiver.Disconnect(this);
            receiver = null;
        }

        private void SetSender(ConnectionPointSender newSender)
        {
            RemoveSender();

            sender = newSender;
            newSender.Connect(this);
        }

        private void SetReceiver(ConnectionPointReceiver newReceiver)
        {
            RemoveReceiver();

            receiver = newReceiver;
            newReceiver.Connect(this);
        }

        /// <summary>
        /// Destroys the connection.
        /// </summary>
        public void Disconnect()
        {
            owner.Connections.Remove(this);
            RemoveSender();
            RemoveReceiver();
        }

        #endregion

        #region Input / Output

        /// <summary>
        /// Reads a P2CNode for a connection.
        /// </summary>
        internal static Connection ReadConnection(Puzzle owner, P2CNode node)
        {
            if (node.Key != "Connection")
                throw new ArgumentException("Specified node is not a Connection");

            Connection connection = new Connection(owner, node.GetProperty("Type").Value);
            int senderid = node.GetProperty("Sender").GetInt32();
            int receiverid = node.GetProperty("Receiver").GetInt32();
            string connectionType = node.GetProperty("Type").Value;

            Item sender = owner.Items.Find(i => i.index == senderid);
            Item receiver = owner.Items.Find(i => i.index == receiverid);

            connection.Sender = sender.GetConnectionSender();
            connection.Receiver = receiver.GetConnectionReceiver(connectionType);

            return connection;
        }

        /// <summary>
        /// Gets a P2CNode for saving.
        /// </summary>
        internal P2CNode GetNode()
        {
            P2CNode node = new P2CNode("Connection");

            Item isender = sender.Owner as Item;
            Item ireceiver = receiver.Owner as Item;

            if (isender == null || ireceiver == null)
                throw new InvalidDataException("Invalid connection, sender or receiver is null.");

            node.Nodes.Add(new P2CProperty("Sender", isender.index));
            node.Nodes.Add(new P2CProperty("Receiver", ireceiver.index));
            node.Nodes.Add(new P2CProperty("Type", Type));

            return node;
        }

        #endregion

        #region Static API

        /// <summary>
        /// Connects two connection points.
        /// </summary>
        internal static void Connect(Puzzle owner, ConnectionPointSender sender, ConnectionPointReceiver receiver)
        {
            if (sender == null || receiver == null)
                throw new ArgumentNullException("Sender or receiver is null.");

            Connection connection = new Connection(owner, receiver.ConnectionType)
            {
                Sender = sender,
                Receiver = receiver
            };

            sender.OnConnect();
            receiver.OnConnect();

            owner.Connections.Add(connection);
        }

        #endregion
    }
}
