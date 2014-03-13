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
using System.Collections.ObjectModel;
using System.Text;

namespace SharpConfig
{
    /// <summary>
    /// Collection of comments.
    /// </summary>
    public class CommentCollection : Collection<Comment>
    {
        internal CommentCollection()
        { }

        internal CommentCollection(IList<Comment> list)
        {
            foreach (var comment in list)
                Add(comment);
        }

        /// <summary>
        /// Converts the type of all comments to a specific type.
        /// </summary>
        ///
        /// <param name="newType">The new comment type.</param>
        public void ConvertType(CommentType newType)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Comment c = Items[i];
                c.Type = newType;
                Items[i] = c;
            }
        }
    }
}
