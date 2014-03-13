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
    /// Represents a comment type.
    /// </summary>
    public enum CommentType
    {
        /// <summary>
        /// The comment begins with a semicolon.
        /// </summary>
        Semicolon,

        /// <summary>
        /// The comment begins with a hash symbol.
        /// </summary>
        HashSymbol
    }

    /// <summary>
    /// A comment.
    /// </summary>
    public struct Comment
    {
        private string mValue;
        private CommentType mType;

        public Comment(string value, int column, CommentType type)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            mValue = value;
            mType = type;
        }

        // Gets a comment type from a char (e.g. CommentType.HashSymbol for a '#').
        internal static CommentType GetCommentType(char c)
        {
            switch (c)
            {
                case '#': return CommentType.HashSymbol;
                case ';': return CommentType.Semicolon;
                default:
                    throw new NotSupportedException(string.Format("Unsupported comment char '{0}'.", c));
            }
        }

        internal char TypeSymbol
        {
            get
            {
                switch (mType)
                {
                    case CommentType.HashSymbol: return '#';
                    case CommentType.Semicolon: return ';';
                    default:
                        throw new NotSupportedException(string.Format("Unsupported comment type '{0}'.", mType));
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public CommentType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", TypeSymbol, Value);
        }

    }
}
