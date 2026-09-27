using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed class CMarkdown
{
    private readonly CAtelier _cMarkdownAtelier;

    internal CMarkdown(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cMarkdownAtelier = atelier;
    }

    public IReadOnlyList<CMarkdownBlock> CMarkdownParse(string? text)
    {
        IReadOnlyList<LMarkdownBlock> blocks = _cMarkdownAtelier.CAtelierEntryPort.LEngineMarkdownParse(text);
        List<CMarkdownBlock> read = new(blocks.Count);
        foreach (LMarkdownBlock block in blocks)
        {
            read.Add(CMarkdownRead(block));
        }

        return read;
    }

    internal static CMarkdownBlock CMarkdownRead(LMarkdownBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);

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
