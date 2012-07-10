using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Portal2.DataTypes
{
    /// <summary>
    /// The interface for nodes in a puzzle file.
    /// </summary>
    internal interface IP2CNode
    {
        void Read(StreamReader reader);
        void Write(StreamWriter writer, string linePrefix);
    }
}
