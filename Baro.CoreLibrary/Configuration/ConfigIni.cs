using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace Baro.CoreLibrary.Configuration
{
    public class ConfigIni : IConfigRepository
    {
        private bool m_disposed;
        private readonly string m_filename;
        private readonly Dictionary<string, IConfigSection> m_sections = ConfigUtil.CreateSectionDictionary();
        private readonly CultureInfo m_culture = new CultureInfo("en-US");

        public ConfigIni(string filename)
        {
            this.AutoSave = true;
            this.m_filename = filename;
            Reload();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.Save();
                    m_sections.Clear();
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                // { code }

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            m_disposed = true;
        }

        ~ConfigIni()
        {
            Dispose(false);
        }

        #region IConfigRepository Members

        public void Reload()
        {
            Clear();

            if (!File.Exists(m_filename))
            {
                File.Create(m_filename).Close();
            }

            using (StreamReader sr = new StreamReader(m_filename, System.Text.Encoding.UTF8))
            {
                string s, sectionName = "anonim";
                IConfigSection section;

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();

                    if (ConfigUtil.isKeyValuePair(s))
                    {
                        if (!m_sections.TryGetValue(sectionName, out section))
                        {
                            section = new ConfigIniSection(sectionName, m_culture);
                            m_sections.Add(section.Name, section);
                        }

                        string[] kvp = s.Trim().Split('=');
                        section.AsString(kvp[0], ConfigUtil.ConvertToString(kvp[1]));
                    }
                    else
                        if (ConfigUtil.GetSectionTitle(s) != null)
                        {
                            sectionName = ConfigUtil.GetSectionTitle(s);
                        }
                }
            }
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(m_filename, false, System.Text.Encoding.UTF8))
            {
                foreach (ConfigIniSection s in this)
                {
                    s.SaveToStream(sw);
                }
            }
        }

        public void Clear()
        {
            m_sections.Clear();
        }

        public IConfigSection GetSection(string section)
        {
            return m_sections[section];
        }

        public IConfigSection CreateSection(string section)
        {
            IConfigSection s = new ConfigIniSection(section, m_culture);
            m_sections.Add(section, s);
            return s;
        }

        public void RemoveSection(string section)
        {
            m_sections.Remove(section);
        }

        public void RemoveSection(IConfigSection section)
        {
            m_sections.Remove(section.Name);
        }
        #endregion

        #region IEnumerable<IConfigSection> Members

        public IEnumerator<IConfigSection> GetEnumerator()
        {
            return m_sections.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_sections.Values.GetEnumerator();
        }

        #endregion

        #region IConfigRepository Members

        public bool AutoSave { get; set; }

        public IConfigSection CreateOrGetSection(string section)
        {
            if (m_sections.ContainsKey(section))
            {
                return m_sections[section];
            }
            else
            {
                return CreateSection(section);
            }
        }

        #endregion
    }
}
