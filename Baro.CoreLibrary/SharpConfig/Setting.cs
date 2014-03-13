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
using System.Linq;

namespace SharpConfig
{
    /// <summary>
    /// A setting.
    /// </summary>
    public sealed class Setting
    {
        #region Fields

        private string mName;
        private string mStringValue;

        internal CommentCollection mPreComments;
        internal Comment? mInlineComment;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="name"> The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public Setting(string name, string value)
        {
            mName = name;
            mStringValue = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this setting.
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
        /// Gets the pre-comments of this setting.
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
        /// Gets the comment of this setting.
        /// </summary>
        ///
        /// <value>
        /// The comment.
        /// </value>
        public Comment? Comment
        {
            get { return mInlineComment; }
            set { mInlineComment = value; }
        }

        /// <summary>
        /// Gets or sets the string value of this setting.
        /// </summary>
        ///
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get { return mStringValue; }
            set { mStringValue = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this setting is an array.
        /// </summary>
        ///
        /// <value>
        /// true if this object is array, false if not.
        /// </value>
        public bool IsArray
        {
            get
            {
                return
                    mStringValue[0] == '[' &&
                    mStringValue[mStringValue.Length - 1] == ']';
            }
        }

        #endregion

        #region GetValue(Type)

        /// <summary>
        /// Gets this setting's value as a specific type.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///                                     illegal values.</exception>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>
        /// The value.
        /// </returns>
        public object GetValue(Type type)
        {
            if (type.IsArray)
                throw new ArgumentException("Specified type is an array type. To read an array, use the ValueArray method.");

            return GetValue(type, mStringValue);
        }

        /// <summary>
        /// Gets this setting's value as a specific type array.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
        ///                                     illegal values.</exception>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>
        /// The value array.
        /// </returns>
        public object[] GetValueArray(Type type)
        {
            if (type.IsArray)
            {
                throw new ArgumentException(
                    "Specified type is an array type. To read an array, only specify the single type like \"bool\" instead of \"bool[]\".");
            }

            if (mStringValue[0] != '[' || mStringValue[mStringValue.Length - 1] != ']')
            {
                throw new Exception(string.Format(
                        "Value '{0}' cannot be converted to type {1}[], because it is not declared as an array.\n" +
                        "To specify an array, enclose your values in brackets ('[' and ']').", mStringValue, type));
            }

            string str = mStringValue.Trim(new[] { '[', ']' });

            string[] split = str.Split(',').Where<string>((s) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return false;
                    else return true;
                }
            ).ToArray();

            var ret = new object[split.Length];

            for (int i = 0; i < split.Length; i++)
            {
                ret[i] = GetValue(type, split[i]);
            }

            return ret;
        }

        private object GetValue(Type type, string val)
        {
            string v = val.Trim();

            if (type == typeof(bool) ||
                type.IsArray && type == typeof(bool[]))
            {
                v = v.ToLower();

                if (v.Equals("off") || v.Equals("no") || v.Equals("0"))
                    v = bool.FalseString;

                if (v.Equals("on") || v.Equals("yes") || v.Equals("1"))
                    v = bool.TrueString;
            }

            if (type.BaseType == typeof(Enum))
            {
                // It's possible that the value is something like:
                // UriFormat.Unescaped
                // We, and especially Enum.Parse do not want this format.
                // Instead, it wants the clean name like:
                // Unescaped
                //
                // Because of that, let's get rid of unwanted type names.
                int indexOfLastDot = v.LastIndexOf('.');

                if (indexOfLastDot >= 0)
                    v = v.Substring(indexOfLastDot + 1, v.Length - indexOfLastDot - 1).Trim();

                try
                {
                    return System.Enum.Parse(type, v, true);
                }
                catch
                {
                    throw new SettingValueCastException(mStringValue, type);
                }
            }

            try
            {
                return Convert.ChangeType(v, type, Configuration.NumberFormat);
            }
            catch
            {
                throw new SettingValueCastException(mStringValue, type);
            }
        }

        #endregion

        #region GetValue<T>

        /// <summary>
        /// Gets this setting's value as a specific type.
        /// </summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>
        /// The value.
        /// </returns>
        public T GetValue<T>()
        {
            return (T)GetValue(typeof(T), mStringValue);
        }

        /// <summary>
        /// Gets this setting's value as a specific type array.
        /// </summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>
        /// The value array.
        /// </returns>
        public T[] GetValueArray<T>()
        {
            object[] arr = GetValueArray(typeof(T));

            var ret = new T[arr.Length];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = (T)arr[i];

            return ret;
        }

        private T GetValue<T>(string val)
        {
            return (T)GetValue(typeof(T), val);
        }

        #endregion

        #region Public Methods

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
        /// Returns a string that represents the current object.
        /// </summary>
        ///
        /// <param name="includeComment">True to include, false to exclude the comment.</param>
        ///
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public string ToString(bool includeComment)
        {
            if (includeComment && mInlineComment.HasValue)
                return string.Format("{0} = {1} {2}", mName, mStringValue, mInlineComment.ToString());
            else
                return string.Format("{0} = {1}", mName, mStringValue);
        }

        #endregion
    }
}
