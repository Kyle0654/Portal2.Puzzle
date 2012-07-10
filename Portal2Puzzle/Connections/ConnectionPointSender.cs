using System;
using System.Collections.Generic;
using System.Text;
using Portal2.Items;

namespace Portal2.Connections
{
    /// <summary>
    /// A connection sender point.
    /// </summary>
    public abstract class ConnectionPointSender : ConnectionPoint
    {
        #region Events

        internal event EventHandler Connected;

        #endregion

        #region Constructor

        protected ConnectionPointSender(Item owner)
            : base(owner, ConnectionPointEnd.Sender)
        {
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
