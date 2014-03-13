/*
 * Copyright (c) 2013 Cem Dervis
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
 * OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SharpConfig
{
    public sealed partial class Configuration
    {
        private void Serialize(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Serialize(stream);
                stream.Close();
            }
        }

        private void Serialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var sb = new StringBuilder();

            // Write all categories.
            foreach (var category in Categories)
            {
                sb.AppendLine(category.ToString(true));

                // Write all settings.
                foreach (var setting in category.Settings)
                {
                    // If the setting has pre-comments, write them first.
                    if (setting.mPreComments != null)
                    {
                        sb.AppendLine();

                        foreach (var preComment in setting.mPreComments)
                            sb.AppendLine(preComment.ToString());
                    }

                    sb.AppendLine(setting.ToString(true));

                    if (setting.mPreComments != null)
                        sb.AppendLine();
                }

                sb.AppendLine();
            }

            // Replace triple new-lines with double new-lines.
            sb.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            
            // Write to stream.
            using (var wr = new StreamWriter(stream))
            {
                wr.Write(sb.ToString());
                wr.Close();
            }
        }

        private int GetStringIndex(StringBuilder sb, int line, int column)
        {
            int currentLine = 1;

            for (int i = 0; i < sb.Length; i++)
            {
                if (currentLine == line)
                {
                    return (i + line * column - column);
                }
                else if (sb[i] == '\n')
                    currentLine++;
            }

            return -1;
        }

        private void SerializeBinary(BinaryWriter writer, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                SerializeBinary(writer, stream);
            }
        }

        public void SerializeBinary(BinaryWriter writer, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (writer == null)
                writer = new BinaryWriter(stream);

            writer.Write(Categories.Count);

            foreach (var category in Categories)
            {
                writer.Write(category.Name);
                writer.Write(category.Settings.Count);

                foreach (var setting in category.Settings)
                {
                    writer.Write(setting.Name);
                    writer.Write(setting.GetValue<string>());
                }
            }

            writer.Close();
        }
    }
}
