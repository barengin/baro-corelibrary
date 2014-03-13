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
using System.IO;
using System.Text;

namespace SharpConfig
{
    public sealed partial class Configuration
    {
        private const char AssignmentOperator = '=';
        private static readonly char[] CommentChars = new char[] { '#', ';' };

        private static int mLineNumber;
        private static SettingCategory mGlobalCategory;

        private static bool IsInQuoteMarks(string line, int startIndex)
        {
            // Check for quote marks.
            // Note: the way it's done here is pretty primitive.
            // It will only check if there are quote marks to the left and right.
            // If so, it presumes that it's a comment symbol inside quote marks and thus, it's not a comment.
            int i = startIndex;
            bool left = false, right = false;

            while (--i >= 0)
            {
                if (line[i] == '\"')
                {
                    left = true;
                    break;
                }
            }

            right = (line.IndexOf('\"', startIndex) > 0);

            if (left && right)
                return true;

            return false;
        }

        private static bool ParseComment(string line, out Comment comment, out bool lineIsOnlyComment)
        {
            comment = default(Comment);
            lineIsOnlyComment = true;

            int commentIndex = line.IndexOfAny(CommentChars);
            if (commentIndex < 0)
                return false; // This line does not contain a comment.

            // Tip from MarkAJones:
            // Database connection strings can contain semicolons, which should not be
            // treated as comments, but rather separators.
            // To avoid this, we have to check for two things:
            // 1. Is the comment inside a string? If so, ignore.
            // 2. Is the comment symbol backslashed (an escaping value)? If so, ignore also.
            
            // If the char before the comment is a backslash, it's not a comment.
            if (commentIndex >= 1 && line[commentIndex - 1] == '\\')
                return false;

            if (IsInQuoteMarks(line, commentIndex))
                return false;

            comment.Type = CommentType.Semicolon;

            foreach(var commentChar in CommentChars)
            {
                if (line[commentIndex] == commentChar)
                {
                    comment.Type = Comment.GetCommentType(commentChar);
                    break;
                }
            }

            comment.Value = line.Substring(commentIndex+1).Trim();

            lineIsOnlyComment = (commentIndex == 0);

            return true;
        }

        // Parses a configuration from a source string.
        // This is the core parsing function.
        private static Configuration Parse(string source, ParseFlags flags)
        {
            Configuration config = new Configuration();

            mGlobalCategory = config.GlobalCategory;
            SettingCategory currentCategory = mGlobalCategory;

            var preComments = new List<Comment>();
            var tmpComment = default(Comment);

            bool readComments = (flags & ParseFlags.IgnoreComments) != ParseFlags.IgnoreComments;

            using (var reader = new StringReader(source))
            {
                string line = null;

                // Read until EOF.
                while ((line = reader.ReadLine()) != null)
                {
                    mLineNumber++;

                    // Remove all leading / trailing white-spaces.
                    line = line.Trim();

                    // Skip blank lines.
                    if (string.IsNullOrEmpty(line))
                        continue;

                    bool isLineOnlyComment = false;

                    if (readComments)
                    {
                        // See if this line is (or contains) a comment.
                        // If so, parse and add to the result.
                        if (ParseComment(line, out tmpComment, out isLineOnlyComment))
                        {
                            if (isLineOnlyComment)
                            {
                                preComments.Add(tmpComment);
                                continue;
                            }
                        }
                    }

                    // Strip away line comments.
                    line = RemoveLineComment(line);

                    // If the line is empty after we've stripped the comments, it means
                    // the line was a comment to begin with, so skip.
                    if (string.IsNullOrEmpty(line))
                        continue;

                    // Categories start with a '['.
                    if (line.StartsWith("["))
                    {
                        currentCategory = ReadCategory(line);

                        if (!string.IsNullOrEmpty(tmpComment.Value))
                            currentCategory.mInlineComment = tmpComment;

                        if (preComments.Count > 0)
                        {
                            currentCategory.mPreComments = new CommentCollection(preComments);
                            preComments.Clear();
                        }

                        if (!currentCategory.Name.Equals(Configuration.GlobalCategoryName))
                            config.Categories.Add(currentCategory);
                    }
                    else
                    {
                        Setting setting = ParseSetting(line, flags);

                        if (!string.IsNullOrEmpty(tmpComment.Value))
                            setting.mInlineComment = tmpComment;

                        if (preComments.Count > 0)
                        {
                            setting.mPreComments = new CommentCollection(preComments);
                            preComments.Clear();
                        }

                        currentCategory.Settings.Add(setting);
                    }
                    
                }
            }

            // Reset temporary fields.
            mLineNumber = 1;
            mGlobalCategory = null;

            return config;
        }

        /**
         * Removes the comment part from a line.
         * For example if you have a line like:
         * 
         * Age = 20 # this is a comment
         * 
         * The result of this method would be:
         * Age = 20
         */
        private static string RemoveLineComment(string line)
        {
            int indexOfComment = line.IndexOf("#");
            
            if (indexOfComment < 0)
                indexOfComment = line.IndexOf(";");

            if (indexOfComment < 0)
                return line; // No comment found, return unmodified line.

            return line.Substring(0, indexOfComment);
        }

        private static SettingCategory ReadCategory(string line)
        {
            line = line.Trim();

            // The closing bracket must be the last char.
            if (line[line.Length-1] != ']')
                throw new ParserException("closing bracket missing.", mLineNumber);

            // Read the category name, and trim all leading / trailing white-spaces.
            string categoryName = line.Substring(1, line.Length - 2).Trim();

            // Check if the global category is specified.
            // If so, return the reference to the global category,
            // instead of creating a new one.
            if (categoryName.Equals(Configuration.GlobalCategoryName))
                return mGlobalCategory;

            // Otherwise, return a fresh category.
            return new SettingCategory(categoryName);
        }

        private static Setting ParseSetting(string line, ParseFlags flags)
        {
            // Find the assignment operator.
            int indexOfAssignOp = line.IndexOf(AssignmentOperator);

            if (indexOfAssignOp < 0)
                throw new ParserException("setting assignment expected.", mLineNumber);
            
            // Trim the setting name and value.
            string settingName = line.Substring(0, indexOfAssignOp).Trim();
            string settingValue = line.Substring(indexOfAssignOp + 1, line.Length - indexOfAssignOp - 1).Trim();

            // Check if non-null name / value is given.
            if (string.IsNullOrEmpty(settingName))
                throw new ParserException("setting name expected.", mLineNumber);

            bool acceptEmptyValue = (flags & ParseFlags.AcceptEmptyValues) == ParseFlags.AcceptEmptyValues;

            if (string.IsNullOrEmpty(settingValue))
            {
                if (!acceptEmptyValue)
                    throw new ParserException("setting value expected.", mLineNumber);
                else
                    settingValue = string.Empty;
            }
            else
            {
                // Trim all quote marks in the value. This is done to provide a clean raw value.
                settingValue = settingValue.Trim(new[] { '\"' });

                int leftBrackets = 0;
                int rightBrackets = 0;

                int i = 0;
                while (settingValue[i++] == '[')
                    leftBrackets++;

                i = 0;
                while (settingValue[settingValue.Length - 1 - (i++)] == ']')
                    rightBrackets++;

                if (leftBrackets > 0 && rightBrackets == 0)
                    throw new ParserException("closing bracket expected for an array.", mLineNumber);

                if (leftBrackets > 1 || rightBrackets > 1)
                    throw new ParserException("too many brackets for an array.", mLineNumber);
            }

            return new Setting(settingName, settingValue);
        }

    }
}
