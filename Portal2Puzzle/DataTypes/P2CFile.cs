using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A puzzle file.
    /// </summary>
    internal class P2CFile
    {
        #region Fields

        /// <summary>
        /// The root node in the file
        /// </summary>
        public P2CNode Root;

        #endregion

        #region Constructor

        private P2CFile()
        {
        }

        internal P2CFile(P2CNode root)
        {
            Root = root;
        }

        #endregion

        #region Input / Output

        /// <summary>
        /// Save a P2CFile to the specified file.
        /// </summary>
        public void Save(string filename)
        {
            using (FileStream file = File.Create(filename))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    Root.Write(writer, "");
                }
            }
        }

        #endregion

        #region Static API

        /// <summary>
        /// Opens and reads a p2c file.
        /// </summary>
        public static P2CFile Open(string filename)
        {
            if (!filename.ToLower().EndsWith(".p2c"))
                throw new ArgumentException("File extension must be .p2c");

            using (FileStream file = File.Open(filename, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string header = reader.ReadLine().Trim();
                    if (header != "\"portal2_puzzle\"")
                        throw new ArgumentException("p2c file is invalid, must start with \"portal2_puzzle\"");

                    P2CFile p2c = new P2CFile();
                    p2c.Root = new P2CNode("portal2_puzzle");
                    p2c.Root.Read(reader);

                    return p2c;
                }
            }
        }

        #endregion
    }
}
