using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A node in a puzzle file.
    /// </summary>
    internal class P2CNode : IP2CNode
    {
        #region Static Data

        static Regex RegexProperty = new Regex("^(?:(?<!\\\\)\")(.*)(?:(?<!\\\\)\")\\s+(?:(?<!\\\\)\")((.|\n)*)(?:(?<!\\\\)\")$");
        static Regex RegexNode = new Regex("^(?:(?<!\\\\)\")([a-zA-Z0-9_]*)(?:(?<!\\\\)\")$");

        #endregion

        #region Fields

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string Key;

        /// <summary>
        /// Children nodes.
        /// </summary>
        public List<IP2CNode> Nodes = new List<IP2CNode>();

        #endregion

        #region Constructor

        /// <summary>
        /// Create a P2C node, assumes you know the key already.
        /// </summary>
        public P2CNode(string key)
        {
            Key = key;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the first node with the specified key
        /// </summary>
        public P2CNode GetNode(string key)
        {
            return Nodes.Find(n => n is P2CNode && ((P2CNode)n).Key == key) as P2CNode;
        }

        /// <summary>
        /// Gets all nodes with the specified key
        /// </summary>
        public List<P2CNode> GetNodes(string key)
        {
            return Nodes.FindAll(n => n is P2CNode && ((P2CNode)n).Key == key).ConvertAll<P2CNode>(n => (P2CNode)n);
        }

        /// <summary>
        /// Gets the first property with the specified key
        /// </summary>
        public P2CProperty GetProperty(string key)
        {
            return Nodes.Find(n => n is P2CProperty && ((P2CProperty)n).Key == key) as P2CProperty;
        }

        /// <summary>
        /// Gets all properties with the specified key
        /// </summary>
        public List<P2CProperty> GetProperties(string key)
        {
            return Nodes.FindAll(n => n is P2CProperty && ((P2CProperty)n).Key == key).ConvertAll<P2CProperty>(n => (P2CProperty)n);
        }

        #endregion

        #region Input / Output

        /// <summary>
        /// Reads a node from the specified stream reader.
        /// </summary>
        public void Read(StreamReader reader)
        {
            int lineCount = 0;

            // Read until we reach the end of the file (or the read breaks early)
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.Trim();
                lineCount++;

                if (string.IsNullOrEmpty(line))
                    continue;

                // Read node open and close
                if (line == "{")
                {
                    if (lineCount == 1)
                        continue;

                    // Throw an exception when invalid opening brace is found
                    throw new InvalidDataException("Node opening brace found within node");
                }

                if (line == "}")
                {
                    // End of node found, return read control to parent
                    break;
                }

                // Try to read either property or node name
                // (Properties are more likely, so check for them first)
                if (MatchProperty(line))
                    continue;

                if (MatchNode(line, reader))
                    continue;

                // Try to read a multi-line property
                while (!reader.EndOfStream && !(line.EndsWith("\"") && !line.EndsWith("\\\"")))
                {
                    line = line + "\n" + reader.ReadLine().Trim();
                }

                if (MatchProperty(line))
                    continue;

                // We didn't recognize anything, this line must be invalid
                throw new InvalidDataException("Invalid line found in file: " + line);
            }
        }

        private bool MatchProperty(string line)
        {
            Match match = RegexProperty.Match(line);
            if (match.Success)
            {
                // Found a property, store it
                string propKey = match.Groups[1].Value;
                string propValue = match.Groups[2].Value;
                P2CProperty property = new P2CProperty(propKey, propValue);
                Nodes.Add(property);
                return true;
            }

            return false;
        }

        private bool MatchNode(string line, StreamReader reader)
        {
            Match match = RegexNode.Match(line);
            if (match.Success)
            {
                // Found a node, create the node and read it in
                string nodeName = match.Groups[1].Value;
                P2CNode child = new P2CNode(nodeName);
                child.Read(reader);
                Nodes.Add(child);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes this node to the specified stream writer.
        /// </summary>
        public void Write(StreamWriter writer, string linePrefix)
        {
            writer.WriteLine(linePrefix + "\"" + Key + "\"");
            writer.WriteLine(linePrefix + "{");

            string childLinePrefix = "\t" + linePrefix;
            foreach (IP2CNode node in Nodes)
            {
                node.Write(writer, childLinePrefix);
            }

            writer.WriteLine(linePrefix + "}");
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return Key;
        }

        #endregion
    }
}
