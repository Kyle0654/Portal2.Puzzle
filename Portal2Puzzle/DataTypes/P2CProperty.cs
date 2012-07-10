using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Portal2.DataTypes
{
    /// <summary>
    /// A property in a puzzle file.
    /// </summary>
    internal class P2CProperty : IP2CNode
    {
        #region Fields

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Key;

        /// <summary>
        /// The value of the property.
        /// </summary>
        public string Value;

        #endregion

        #region Constructors

        public P2CProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public P2CProperty(string key, int value)
        {
            Key = key;
            Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public P2CProperty(string key, bool value)
        {
            Key = key;
            Value = value ? "1" : "0";
        }

        public P2CProperty(string key, double value)
        {
            Key = key;
            Value = value.ToString("0.000000", CultureInfo.InvariantCulture);
        }

        public P2CProperty(string key, DateTime value)
        {
            Key = key;

            long seconds = (long)value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            Value = "0x" + seconds.ToString("X16", CultureInfo.InvariantCulture);
        }

        public P2CProperty(string key, Point3 value)
        {
            Key = key;
            Value = value.ToString();
        }

        public P2CProperty(string key, FPoint3 value)
        {
            Key = key;
            Value = value.ToString();
        }

        #endregion

        #region Accessors

        public int GetInt32()
        {
            return Int32.Parse(Value, CultureInfo.InvariantCulture);
        }

        public bool GetBool()
        {
            return Value == "1";
        }

        public double GetDouble()
        {
            return Double.Parse(Value, CultureInfo.InvariantCulture);
        }

        public DateTime GetDateTime()
        {
            string val = Value;
            if (val.StartsWith("0x"))
                val = val.Substring(2);

            long seconds = long.Parse(val, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(seconds);
        }

        public Point3 GetPoint3()
        {
            return new Point3(Value);
        }

        public FPoint3 GetFPoint3()
        {
            return new FPoint3(Value);
        }

        #endregion

        #region Input / Output

        public void Read(StreamReader reader)
        {
            // No reading to be done - P2CNode handles that
        }

        /// <summary>
        /// Writes the property to the specified stream writer.
        /// </summary>
        public void Write(StreamWriter writer, string linePrefix)
        {
            writer.WriteLine("{0}\"{1}\"\t\t\"{2}\"", linePrefix, Key, Value);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("\"{0}\" = \"{1}\"", Key, Value);
        }

        #endregion
    }
}
