using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace OpenNETCF.Diagnostics
{
    internal static class LogFile
    {
        private static string m_target;
        private static string m_indent;
        private static int m_indentLevel;
        private static bool m_timestamp;

        static LogFile()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            m_target = Path.Combine(path, "log.txt");
            m_indent = string.Empty;
        }

        [Conditional("DEBUG_LOG")]
        public static void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        [Conditional("DEBUG_LOG")]
        public static void Write(string text)
        {
            string line = string.Format("{0}{1}{2}",
                m_timestamp ? DateTime.Now.ToString("mm/dd/yy hh:mm:ss") : string.Empty,
                m_indent,
                text);

            using (TextWriter writer = new StreamWriter(m_target, true, Encoding.ASCII))
            {
                writer.Write(line);
            }
        }

        public static bool TimeStampLines
        {
            set { m_timestamp = value; }
            get { return m_timestamp; }
        }

        [Conditional("DEBUG_LOG")]
        public static void Indent()
        {
            m_indentLevel++;
            m_indent = new string(' ', 2 * m_indentLevel);
        }

        [Conditional("DEBUG_LOG")]
        public static void Outdent()
        {
            m_indentLevel--;
            if (m_indentLevel < 0) m_indentLevel = 0;

            m_indent = new string(' ', 2 * m_indentLevel);
        }

        public static int IndentLevel
        {
            get { return m_indentLevel; }
            set
            {
                m_indentLevel = value;
                if (m_indentLevel < 0) m_indentLevel = 0;

                m_indent = new string(' ', 2 * m_indentLevel);
            }
        }

        public static string LogFileName
        {
            set { m_target = value; }
            get { return m_target; }
        }
    }
}
