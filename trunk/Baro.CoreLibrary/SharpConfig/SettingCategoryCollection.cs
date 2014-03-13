﻿/*
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
    /// A collection of setting categories.
    /// </summary>
    public sealed class SettingCategoryCollection : Collection<SettingCategory>
    {
        internal SettingCategoryCollection()
        { }

        /// <summary>
        /// Inserts an item.
        /// </summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="index">Zero-based index of the item.</param>
        /// <param name="item"> The item.</param>
        protected override void InsertItem(int index, SettingCategory item)
        {
            if (this[item.Name] != null)
                throw new Exception(string.Format("A category named '{0}' is already present.", item.Name));

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Gets a category by its name using ordinal string sorting rules.
        /// </summary>
        ///
        /// <param name="name">The name of the category.</param>
        ///
        /// <returns>
        /// The category if found, <b>null</b> otherwise.
        /// </returns>
        public SettingCategory this[string name]
        {
            get
            {
                return this[name, false];
            }
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
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    SettingCategory cat = this[i];
                    if (cat.Name.Equals(name, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.Ordinal))
                        return cat;
                }

                return null;
            }
        }

    }
}
