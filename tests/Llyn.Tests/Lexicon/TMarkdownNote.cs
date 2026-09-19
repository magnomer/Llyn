using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkdownNote
{
    [Fact]
    public void MarkdownNormalize_WindowsLineEndsAndEdgeBlanks_YieldsCanonical()
    {
        Assert.Equal("one\n\ntwo", TInterface.TMarkdownNormalize("\r\none  \r\n\r\ntwo \n\n"));
        Assert.Equal(string.Empty, TInterface.TMarkdownNormalize("   \r\n \n"));
    }

    [Fact]
    public void MarkdownParse_EveryBlockShape_YieldsOneBlockEach()
    {
        IReadOnlyList<LMarkdownBlock> blocks = TInterface.TMarkdownParse(
            "## Title\n\nfirst line\nsecond line\n\n- one\n  - two\n1. three\n> said\n> so\n\n```\nx < y\n```\n---");

        Assert.Equal(
            [
                LMarkdownKind.LMarkdownKindHeading,
                LMarkdownKind.LMarkdownKindParagraph,
                LMarkdownKind.LMarkdownKindBullet,
                LMarkdownKind.LMarkdownKindBullet,
                LMarkdownKind.LMarkdownKindNumber,
                LMarkdownKind.LMarkdownKindQuote,
                LMarkdownKind.LMarkdownKindCode,
                LMarkdownKind.LMarkdownKindRule,
            ],
            blocks.Select(block => block.LMarkdownBlockKind));
        Assert.Equal(2, blocks[0].LMarkdownBlockLevel);
        Assert.Equal("first line\nsecond line", blocks[1].LMarkdownBlockSpan[0].LMarkdownSpanText);
        Assert.Equal(1, blocks[3].LMarkdownBlockLevel);
        Assert.Equal("said\nso", blocks[5].LMarkdownBlockSpan[0].LMarkdownSpanText);
        Assert.Equal("x < y", blocks[6].LMarkdownBlockText);
    }

    [Fact]
    public void MarkdownParse_BoldItalicCodeLink_YieldsStyledSpans()
    {
        IReadOnlyList<LMarkdownSpan> spans = TInterface.TMarkdownParse(
            "a **b *c*** `d` [e](https://x.y) \\*f\\*")[0].LMarkdownBlockSpan;

        Assert.Equal(["a ", "b ", "c", " ", "d", " ", "e", " *f*"], spans.Select(span => span.LMarkdownSpanText));
        Assert.True(spans[1].LMarkdownSpanBold);
        Assert.True(spans[2].LMarkdownSpanBold && spans[2].LMarkdownSpanItalic);
        Assert.True(spans[4].LMarkdownSpanCode);
        Assert.Equal("https://x.y", spans[6].LMarkdownSpanLink);
        Assert.False(spans[7].LMarkdownSpanItalic);
    }

    [Fact]
    public void MarkdownParse_StrayMarkerAndSnakeCase_KeepsThemLiteral()
    {
        IReadOnlyList<LMarkdownSpan> spans =
            TInterface.TMarkdownParse("2 * 3 in snake_case_name")[0].LMarkdownBlockSpan;

        Assert.Single(spans);
        Assert.Equal("2 * 3 in snake_case_name", spans[0].LMarkdownSpanText);
        Assert.False(spans[0].LMarkdownSpanItalic);
    }

    [Fact]
    public void EntrySave_NoteWithWindowsLineEnds_StoresNormalizedMarkdown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TMarkdownNoteCreate("# Title \r\n\r\n- one  \r\n"));

        Assert.Equal("# Title\n\n- one", engine.TEngineNoteRead(entry.LEntryId)?.LNoteText);
    }

    [Fact]
    public void NoteSave_WindowsLineEnds_StoresNormalizedMarkdown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TMarkdownNoteCreate(string.Empty));
        engine.TEngineNoteSave(TInterface.TNoteCreate(entry.LEntryId, "one \r\ntwo\r\n\r\n"));

        Assert.Equal("one\ntwo", engine.TEngineNoteRead(entry.LEntryId)?.LNoteText);
    }

    private static LEntryDraft TMarkdownNoteCreate(string note)
    {
        return TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            note,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            []);
    }

    [Fact]
    public void MarkdownParse_NumberedRuns_NumbersEachRunFromOne()
    {
        IReadOnlyList<LMarkdownBlock> blocks = TInterface.TMarkdownParse("1. one\n2. two\n\nplain\n\n1. again");

        Assert.Equal([1, 2, 0, 1], blocks.Select(block => block.LMarkdownBlockOrdinal));
        Assert.Equal([true, true, false, true], blocks.Select(block => block.LMarkdownBlockNumbered));
    }

    [Fact]
    public void MarkdownSpanAddress_WebAndLocalLinks_KeepsOnlyWebAddresses()
    {
        IReadOnlyList<LMarkdownSpan> spans = TInterface.TMarkdownParse(
            "[web](https://example.org/a) and [file](file:///tmp/a) and plain")[0].LMarkdownBlockSpan;

        Assert.Equal("https://example.org/a", spans[0].LMarkdownSpanAddress?.AbsoluteUri);
        Assert.Null(spans[2].LMarkdownSpanAddress);
        Assert.Null(spans[3].LMarkdownSpanAddress);
    }
}
