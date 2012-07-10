using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// The end of a connection point.
    /// </summary>
    public enum ConnectionPointEnd
    {
        Sender,
        Receiver
    }

    /// <summary>
    /// A connection point - can be connected to another connection point.
    /// </summary>
    public abstract class ConnectionPoint
    {
        #region Properties

        /// <summary>
        /// The owner of this connection point.
        /// </summary>
        public Item Owner { get; private set; }

        /// <summary>
        /// Whether this point is a sender or receiver.
        /// </summary>
        public ConnectionPointEnd ConnectionEnd { get; private set; }

        #endregion

        #region Constructors

        protected ConnectionPoint(Item owner, ConnectionPointEnd connectionEnd)
        {
            Owner = owner;
            ConnectionEnd = connectionEnd;
        }

        #endregion
    }
}
