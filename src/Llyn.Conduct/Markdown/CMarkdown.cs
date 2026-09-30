using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CMarkdown
{
    internal static IReadOnlyList<CMarkdownBlock> LMarkdownParse(LEntryPort entries, string? text)
    {
        ArgumentNullException.ThrowIfNull(entries);

        IReadOnlyList<LMarkdownBlock> blocks = entries.LEngineMarkdownParse(text);
        List<CMarkdownBlock> read = new(blocks.Count);
        foreach (LMarkdownBlock block in blocks)
        {
            read.Add(LMarkdownRead(block));
        }

        return read;
    }

    private static CMarkdownBlock LMarkdownRead(LMarkdownBlock block)
    {
        List<CMarkdownSpan> spans = new(block.LMarkdownBlockSpan.Count);
        foreach (LMarkdownSpan span in block.LMarkdownBlockSpan)
        {
            spans.Add(new CMarkdownSpan(
                span.LMarkdownSpanText,
                span.LMarkdownSpanBold,
                span.LMarkdownSpanItalic,
                span.LMarkdownSpanCode,
                span.LMarkdownSpanAddress));
        }

        return new CMarkdownBlock(
            block.LMarkdownBlockHeaded,
            block.LMarkdownBlockListed,
            block.LMarkdownBlockQuoted,
            block.LMarkdownBlockFenced,
            block.LMarkdownBlockRuled,
            block.LMarkdownBlockLevel,
            block.LMarkdownBlockMark,
            block.LMarkdownBlockText,
            spans);
    }
}
