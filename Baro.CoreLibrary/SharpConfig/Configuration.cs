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
using System.Globalization;
using System.IO;
using System.Text;

namespace SharpConfig
{
    /// <summary>
    /// Contains information about a configuration file or stream.
    /// </summary>
    public sealed partial class Configuration
    {
        #region Fields

        /// <summary>
        /// Name of the global category.
        /// </summary>
        public const string GlobalCategoryName = "General";

        internal static readonly NumberFormatInfo NumberFormat;
        private SettingCategoryCollection mCategories;

        #endregion

        #region Construction

        static Configuration()
        {
            NumberFormat = CultureInfo.InvariantCulture.NumberFormat;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Configuration()
        {
            mCategories = new SettingCategoryCollection();
            mCategories.Add(new SettingCategory(Configuration.GlobalCategoryName));
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads a configuration from a file, including comments. Empty setting values are not accepted.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        ///
        /// <param name="filename">The location of the configuration file.</param>
        ///
        /// <returns>
        /// The loaded <see cref="Configuration"/> object.
        /// </returns>
        public static Configuration Load(string filename)
        {
            return Load(filename, ParseFlags.None);
        }

        /// <summary>
        /// Loads a configuration from a file.
        /// </summary>
        ///
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        ///
        /// <param name="filename">The location of the configuration file.</param>
        /// <param name="flags">Parsing flags.</param>
        ///
        /// <returns>
        /// The loaded <see cref="Configuration"/> object.
        /// </returns>
        public static Configuration Load(string filename, ParseFlags flags)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("Configuration file not found:" + filename);

            string src;
            
            using (StreamReader sr = File.OpenText(filename))
            {
                src = sr.ReadToEnd();
            }

            // string src = File.ReadAllText(filename);
            Configuration config = Parse(src, flags);

            return config;
        }

        /// <summary>
        /// Loads a configuration from a stream.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="stream">The stream to load the configuration from.</param>
        /// <param name="flags">Parsing flags.</param>
        ///
        /// <returns>
        /// The loaded <see cref="Configuration"/> object.
        /// </returns>
        public static Configuration Load(Stream stream, ParseFlags flags)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string src = null;
            using (var sr = new StreamReader(stream))
                src = sr.ReadToEnd();

            Configuration config = Parse(src, flags);

            return config;
        }

        #endregion

        #region LoadBinary

        /// <summary>
        /// Loads a configuration from a binary file, using the default <see cref="BinaryReader"/>.
        /// </summary>
        ///
        /// <param name="filename">The location of the configuration file.</param>
        ///
        /// <returns>
        /// The configuration.
        /// </returns>
        public static Configuration LoadBinary(string filename)
        {
            return LoadBinary(null, filename);
        }

        /// <summary>
        /// Loads a configuration from a binary file, using a specific reader.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="reader">  The reader to use. Specifiy <b>null</b> to use the default reader.</param>
        /// <param name="filename">The location of the configuration file.</param>
        ///
        /// <returns>
        /// The configuration.
        /// </returns>
        public static Configuration LoadBinary(BinaryReader reader, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            return DeserializeBinary(reader, filename);
        }

        /// <summary>
        /// Loads a configuration from a binary stream, using the default <see cref="BinaryReader"/>.
        /// </summary>
        ///
        /// <param name="stream">The stream to load the configuration from.</param>
        ///
        /// <returns>
        /// The configuration.
        /// </returns>
        public static Configuration LoadBinary(Stream stream)
        {
            return LoadBinary(null, stream);
        }

        /// <summary>
        /// Loads a configuration from a binary stream, using a specific <see cref="BinaryReader"/>.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="reader">The reader to use. Specifiy <b>null</b> to use the default reader.</param>
        /// <param name="stream">The stream to load the configuration from.</param>
        ///
        /// <returns>
        /// The configuration.
        /// </returns>
        public static Configuration LoadBinary(BinaryReader reader, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            return DeserializeBinary(reader, stream);
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the configuration to a file.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="filename">The location of the configuration file.</param>
        public void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            Serialize(filename);
        }

        /// <summary>
        /// Saves the configuration to a stream.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="stream">The stream to save the configuration to.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Serialize(stream);
        }

        #endregion

        #region SaveBinary

        /// <summary>
        /// Saves the configuration to a binary file, using the default <see cref="BinaryWriter"/>.
        /// </summary>
        ///
        /// <param name="filename">The location of the configuration file.</param>
        public void SaveBinary(string filename)
        {
            SaveBinary(null, filename);
        }

        /// <summary>
        /// Saves the configuration to a binary file, using a specific <see cref="BinaryWriter"/>.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="writer">  The writer to use. Specifiy <b>null</b> to use the default writer.</param>
        /// <param name="filename">The location of the configuration file.</param>
        public void SaveBinary(BinaryWriter writer, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            SerializeBinary(writer, filename);
        }

        /// <summary>
        /// Saves the configuration to a binary stream, using the default <see cref="BinaryWriter"/>.
        /// </summary>
        ///
        /// <param name="stream">The stream to save the configuration to.</param>
        public void SaveBinary(Stream stream)
        {
            SaveBinary(null, stream);
        }

        /// <summary>
        /// Saves the configuration to a binary file, using a specific <see cref="BinaryWriter"/>.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="writer">The writer to use. Specifiy <b>null</b> to use the default writer.</param>
        /// <param name="stream">The stream to save the configuration to.</param>
        public void SaveBinary(BinaryWriter writer, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            SerializeBinary(writer, stream);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the names of all categories and settings
        /// within this configuration to lower-case.
        /// </summary>
        public void ConvertToLowerCase()
        {
            foreach (var category in Categories)
            {
                category.Name = category.Name.ToLower();

                foreach (var setting in category.Settings)
                    setting.Name = setting.Name.ToLower();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the global category of this configuration.
        /// </summary>
        ///
        /// <value>
        /// The global category.
        /// </value>
        public SettingCategory GlobalCategory
        {
            get { return mCategories[GlobalCategoryName]; }
        }

        /// <summary>
        /// Gets the categories of this configuration.
        /// </summary>
        ///
        /// <value>
        /// The categories.
        /// </value>
        public SettingCategoryCollection Categories
        {
            get { return mCategories; }
        }

        /// <summary>
        /// Gets a category by its name.
        /// </summary>
        ///
        /// <param name="name">      The name of the category.</param>
        ///
        /// <returns>
        /// The category if found, <b>null</b> otherwise.
        /// </returns>
        public SettingCategory this[string name]
        {
            get { return mCategories[name]; }
        }

        /// <summary>
        /// Gets a category by its name.
        /// </summary>
        ///
        /// <param name="name">      The name of the category.</param>
        /// <param name="ignoreCase">True to ignore case, false otherwise.</param>
        ///
        /// <returns>
        /// The category if found, <b>null</b> otherwise.
        /// </returns>
        public SettingCategory this[string name, bool ignoreCase]
        {
            get { return mCategories[name, ignoreCase]; }
        }

        #endregion
    }
}
