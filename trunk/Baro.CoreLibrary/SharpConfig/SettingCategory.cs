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

namespace SharpConfig
{
    /// <summary>
    /// A group of settings.
    /// </summary>
    public sealed class SettingCategory
    {
        private string mName;
        private SettingCollection mSettings;

        internal CommentCollection mPreComments;
        internal Comment? mInlineComment;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="name">The name of the category.</param>
        public SettingCategory(string name)
        {
            mName = name;
            mSettings = new SettingCollection();
        }

        /// <summary>
        /// Assign the values of this category to an object's public properties.
        /// </summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The object.</param>
        ///
        /// <returns>
        /// Returns the object that was given to this method.
        /// Useful for structures.
        /// </returns>
        public T AssignTo<T>(T obj)
        {
            Type type = typeof(T);

            var properties = type.GetProperties();

            object boxedObj = obj;

            foreach (var prop in properties)
            {
                foreach (var setting in mSettings)
                {
                    if (prop.CanWrite && prop.Name.Equals(setting.Name))
                    {
                        var propType = prop.PropertyType;

                        prop.SetValue(boxedObj, setting.GetValue(propType), null);
                    }
                }
            }

            return (T)boxedObj;
        }

        /// <summary>
        /// Gets the name of this category.
        /// </summary>
        ///
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return mName; }
            internal set { mName = value; }
        }

        /// <summary>
        /// Gets the settings of this category.
        /// </summary>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public SettingCollection Settings
        {
            get { return mSettings; }
        }

        /// <summary>
        /// Gets the pre-comments of this category.
        /// </summary>
        ///
        /// <value>
        /// The pre-comments.
        /// </value>
        public CommentCollection PreComments
        {
            get
            {
                if (mPreComments == null)
                    mPreComments = new CommentCollection();

                return mPreComments;
            }
        }

        /// <summary>
        /// Gets the comment of this category.
        /// </summary>
        ///
        /// <value>
        /// The comment.
        /// </value>
        public Comment? Comment
        {
            get { return mInlineComment; }
        }

        /// <summary>
        /// Gets a setting by its name.
        /// </summary>
        ///
        /// <param name="name">      The name of the setting.</param>
        ///
        /// <returns>
        /// The setting if found, <b>null</b> otherwise.
        /// </returns>
        public Setting this[string name]
        {
            get { return mSettings[name]; }
        }

        /// <summary>
        /// Gets a setting by its name.
        /// </summary>
        ///
        /// <param name="name">      The name of the setting.</param>
        /// <param name="ignoreCase">True to ignore case, false otherwise.</param>
        ///
        /// <returns>
        /// The setting if found, <b>null</b> otherwise.
        /// </returns>
        public Setting this[string name, bool ignoreCase]
        {
            get { return mSettings[name, ignoreCase]; }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        ///
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        ///
        /// <param name="includeComment">True to include, false to exclude the comment.</param>
        ///
        /// <returns>
        /// includeComment as a string.
        /// </returns>
        public string ToString(bool includeComment)
        {
            if (includeComment && mInlineComment.HasValue)
                return string.Format("[{0}] {1}", mName, mInlineComment.ToString());
            else
                return string.Format("[{0}]", mName);
        }
    }
}
