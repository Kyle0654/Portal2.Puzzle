using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection receiver point.
    /// </summary>
    public abstract class ConnectionPointReceiver : ConnectionPoint
    {
        #region Events

        internal event EventHandler Connected;

        #endregion

        #region Properties

        /// <summary>
        /// The type of connection this endpoint receives.
        /// </summary>
        public string ConnectionType { get; protected set; }

        #endregion

        #region Constructor

        protected ConnectionPointReceiver(Item owner, string connectionType)
            : base(owner, ConnectionPointEnd.Receiver)
        {
            ConnectionType = connectionType;
            owner.RegisterConnection(this);
        }

        #endregion

        #region Connection API

        internal abstract void Connect(Connection connection);
        internal abstract void Disconnect(Connection connection);

        internal void OnConnect()
        {
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        #endregion
    }
}
