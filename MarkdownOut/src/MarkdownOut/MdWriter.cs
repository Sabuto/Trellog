﻿using System;
using System.IO;

namespace MarkdownOut
{
    /// <summary>
    ///     A wrapper around <see cref="StreamWriter" /> used to write Markdown text to a file.
    /// </summary>
    public class MdWriter : IDisposable
    {
        private StreamWriter stream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MdWriter" /> class, opening a stream to the
        ///     file at the specified path.
        /// </summary>
        /// <param name="path">The full path to the output file, including file extension.</param>
        /// <param name="append">
        ///     If true, output is appended to the file's existing contents; otherwise, the file is
        ///     overwritten.
        /// </param>
        public MdWriter(string path, bool append = false)
        {
            stream = new StreamWriter(path, append);
        }

        #region IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        #endregion

        /// <summary>
        ///     Closes the stream to the output file.
        /// </summary>
        public void Close()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
        }

        /// <summary>
        ///     Writes the provided output to the file.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="style">The optional Markdown style to apply.</param>
        /// <param name="format">The optional Markdown format to apply.</param>
        /// <param name="useMdLineBreaks">
        ///     If true, when the text is cleansed, all newlines will be replaced by Markdown line
        ///     breaks to maintain the assumed intended line breaks in the text; otherwise, the text's
        ///     newlines will not parsed as line breaks by Markdown parsers.
        /// </param>
        public void Write(object output, MdStyle style = MdStyle.None,
            MdFormat format = MdFormat.None, bool useMdLineBreaks = true)
        {
            string text = MdText.StyleAndFormat(output, style, format);
            stream.Write(MdText.Cleanse(text, useMdLineBreaks));
        }

        /// <summary>
        ///     Writes the provided output to the file, followed by a Markdown paragraph break to
        ///     terminate the line and start a new paragraph.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="style">The optional Markdown style to apply.</param>
        /// <param name="format">The optional Markdown format to apply.</param>
        /// <param name="useMdLineBreaks">
        ///     If true, when the text is cleansed, all newlines will be replaced by Markdown line
        ///     breaks to maintain the assumed intended line breaks in the text; otherwise, the text's
        ///     newlines will not parsed as line breaks by Markdown parsers.
        /// </param>
        public void WriteLine(object output, MdStyle style = MdStyle.None,
            MdFormat format = MdFormat.None, bool useMdLineBreaks = true)
        {
            string text = MdText.StyleAndFormat(output, style, format);
            stream.Write(MdText.Cleanse(text, useMdLineBreaks) + MdText.ParagraphBreak);
        }

        /// <summary>
        ///     Writes the provided output to the file, followed by a Markdown line break to terminate
        ///     the line but not start a new paragraph.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="style">The optional Markdown style to apply.</param>
        /// <param name="format">
        ///     The optional Markdown format to apply. Some formats may not be terminated by the
        ///     Markdown line break.
        /// </param>
        /// <param name="useMdLineBreaks">
        ///     If true, when the text is cleansed, all newlines will be replaced by Markdown line
        ///     breaks to maintain the assumed intended line breaks in the text; otherwise, the text's
        ///     newlines will not parsed as line breaks by Markdown parsers.
        /// </param>
        public void WriteLineSingle(object output, MdStyle style = MdStyle.None,
            MdFormat format = MdFormat.None, bool useMdLineBreaks = true)
        {
            string text = MdText.StyleAndFormat(output, style, format);
            stream.Write(MdText.Cleanse(text, useMdLineBreaks) + MdText.LineBreak);
        }

        /// <summary>
        ///     Writes the provided output to the file using the
        ///     <see cref="MdFormat.UnorderedListItem" /> format, followed by a Markdown paragraph break
        ///     to terminate the list item.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="listIndent">
        ///     The optional indent of the list item (0 adds no indent, 1 adds a single indentation to
        ///     create a sublist, etc). Negative values are ignored.
        /// </param>
        /// <param name="style">The optional Markdown style to apply.</param>
        public void WriteUnorderedListItem(object output, int listIndent = 0,
            MdStyle style = MdStyle.None)
        {
            string text = MdText.StyleAndFormat(output, style, MdFormat.UnorderedListItem);
            text = MdText.Indent(text, listIndent);
            stream.Write(MdText.Cleanse(text, true) + MdText.ParagraphBreak);
        }

        /// <summary>
        ///     Writes the provided output to the file using the <see cref="MdFormat.OrderedListItem" />
        ///     format, followed by a Markdown paragraph break to terminate the list item.
        /// </summary>
        /// <param name="output">The output to write.</param>
        /// <param name="itemNumber">
        ///     The optional item number of the list item. Does not affect parsed Markdown output or the
        ///     list order, only the raw text. Negative values are ignored.
        /// </param>
        /// <param name="listIndent">
        ///     The optional indent of the list item (0 adds no indent, 1 adds a single indentation to
        ///     create a sublist, etc). Negative values are ignored.
        /// </param>
        /// <param name="style">The optional Markdown style to apply.</param>
        public void WriteOrderedListItem(object output, int itemNumber = 1, int listIndent = 0,
            MdStyle style = MdStyle.None)
        {
            string text = MdText.StyleAndFormat(output, style, MdFormat.OrderedListItem);
            //replace the list item number supplied by MdText with the number provided
            if (itemNumber != MdText.DefaultListItemNumber && itemNumber >= 0) text = itemNumber + text.Substring(1);
            text = MdText.Indent(text, listIndent);
            stream.Write(MdText.Cleanse(text, true) + MdText.ParagraphBreak);
        }
    }
}