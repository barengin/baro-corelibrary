using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace Baro.CoreLibrary.Configuration
{
    public sealed class ConfigIniSection : IConfigSection
    {
        private readonly Dictionary<string, string> m_keys = ConfigUtil.CreateKVPDictionary();
        private readonly CultureInfo m_culture;

        internal ConfigIniSection(string name, CultureInfo culture)
        {
            this.Name = name;
            this.m_culture = culture;
        }

        #region IConfigSection Members

        public string Name
        {
            get;
            internal set;
        }

        public void Clear()
        {
            m_keys.Clear();
        }

        public string AsString(string key)
        {
            try
            {
                return m_keys[key];
            }
            catch(KeyNotFoundException)
            {
                throw new ConfigRepositoryException(this.Name + " bölümü içinde " + key + " anahtarı bulunamadı. Config dosyası hatalı olabilir.");
            }
        }

        public int AsInt32(string key)
        {
            return int.Parse(AsString(key), m_culture);
        }

        public long AsLong(string key)
        {
            return long.Parse(AsString(key), NumberStyles.Any, m_culture);
        }

        public double AsDouble(string key)
        {
            return double.Parse(AsString(key), m_culture);
        }

        public bool AsBool(string key)
        {
            return bool.Parse(AsString(key));
        }

        public void AsString(string key, string value)
        {
            if (m_keys.ContainsKey(key))
                m_keys.Remove(key);

            m_keys.Add(key, value);
        }

        public void AsInt32(string key, int value)
        {
            if (m_keys.ContainsKey(key))
                m_keys.Remove(key);

            m_keys.Add(key, value.ToString(m_culture));
        }

        public void AsLong(string key, long value)
        {
            if (m_keys.ContainsKey(key))
                m_keys.Remove(key);

            m_keys.Add(key, value.ToString(m_culture));
        }

        public void AsDouble(string key, double value)
        {
            if (m_keys.ContainsKey(key))
                m_keys.Remove(key);

            m_keys.Add(key, value.ToString(m_culture));
        }

        public void AsBool(string key, bool value)
        {
            if (m_keys.ContainsKey(key))
                m_keys.Remove(key);

            m_keys.Add(key, value.ToString());
        }

        public void RemoveKey(string key)
        {
            m_keys.Remove(key);
        }

        public bool ContainsKey(string key)
        {
            return m_keys.ContainsKey(key);
        }

        #endregion

        #region IEnumerable<> Members

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return m_keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_keys.GetEnumerator();
        }

        #endregion

        internal void SaveToStream(StreamWriter sw)
        {
            sw.WriteLine("[" + this.Name + "]");

            foreach (KeyValuePair<string, string> kvp in this)
            {
                sw.WriteLine(kvp.Key + "=" + ConfigUtil.ConvertToConfigValue(kvp.Value));
            }

            sw.WriteLine();
        }

        public string AsString(string key, string defaultValue, bool addIfNotExists)
        {
            if (m_keys.ContainsKey(key))
            {
                return m_keys[key];
            }
            else
            {
                if (addIfNotExists)
                {
                    AsString(key, defaultValue);
                }

                return defaultValue;
            }
        }

        public int AsInt32(string key, int defaultValue, bool addIfNotExists)
        {
            if (m_keys.ContainsKey(key))
            {
                return AsInt32(key);
            }
            else
            {
                if (addIfNotExists)
                {
                    AsInt32(key, defaultValue);
                }

                return defaultValue;
            }
        }

        public long AsLong(string key, long defaultValue, bool addIfNotExists)
        {
            if (m_keys.ContainsKey(key))
            {
                return AsLong(key);
            }
            else
            {
                if (addIfNotExists)
                {
                    AsLong(key, defaultValue);
                }

                return defaultValue;
            }
        }

        public double AsDouble(string key, double defaultValue, bool addIfNotExists)
        {
            if (m_keys.ContainsKey(key))
            {
                return AsDouble(key);
            }
            else
            {
                if (addIfNotExists)
                {
                    AsDouble(key, defaultValue);
                }

                return defaultValue;
            }
        }

        public bool AsBool(string key, bool defaultValue, bool addIfNotExists)
        {
            if (m_keys.ContainsKey(key))
            {
                return AsBool(key);
            }
            else
            {
                if (addIfNotExists)
                {
                    AsBool(key, defaultValue);
                }

                return defaultValue;
            }
        }

        #region IConfigSection Members


        public string this[string key]
        {
            get { return AsString(key); }
        }

        #endregion
    }
}
