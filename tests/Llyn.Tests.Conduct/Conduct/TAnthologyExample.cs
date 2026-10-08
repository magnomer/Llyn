using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnthologyExample
{
    [Fact]
    public void AnthologyTextSetAndSpeakerSet_TypedTextAndPickedLanguage_WriteTheTranscriptAndAnswerTheTextHint()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        string hint = anthology.CAnthologyTextSet("a cat");
        desk.CDeskPersist();
        anthology.CAnthologySpeakerSet("French");

        CExample? held = anthology.TAnthologyDraftRead(desk.TDeskRead());
        Assert.Equal("Example.Text", hint);
        Assert.Equal("a cat", held?.CExampleText.CStateValueText);
        Assert.Equal("French", held?.CExampleLanguage);
    }

    [Fact]
    public void AnthologyTextCheck_BlankField_MatchesAnEmptyText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out _);

        Assert.True(anthology.CAnthologyTextCheck("  ", CStateValue.CStateValueEmpty));
        Assert.True(anthology.CAnthologyTextCheck("a cat", new CStateValue("a cat", false)));
        Assert.False(anthology.CAnthologyTextCheck("a cat ", new CStateValue("a cat", false)));
    }

    [Fact]
    public void AnthologyExampleRead_NoExample_ReturnsNone()
    {
        Assert.Null(TInterfaceConductPanel.TAnthologyExampleRead(null, string.Empty, string.Empty));
    }

    [Fact]
    public void AnthologyExampleRead_SoundText_DividesExcerptAtItsMentions()
    {
        LExample example = TInterfaceExample.TExampleCreate(
                5, "English", TInterfaceState.TStateValueCreate("a cat"), null, TInterfaceState.TStateAnchorRead(9))
            .TExampleMentionAdd(TInterfaceMentionSpan.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceConductPanel.TAnthologyExampleRead(example, "Field Notes", "2 quotes");

        Assert.NotNull(held);
        Assert.Equal("English", held!.CExampleLanguage);
        Assert.Equal("a cat", held.CExampleText.CStateValueText);
        Assert.Equal(9, held.CExampleSource);
        Assert.Equal("Field Notes", held.CExampleCitation);
        Assert.Equal("Example.Text", held.CExampleTextHint);
        Assert.Null(held.CExampleWording);
        Assert.False(held.CExampleMuted);
        Assert.Equal("2 quotes", held.CExampleTally);
        Assert.Equal(["a ", "cat"], held.CExamplePiece.Select(piece => piece.CMentionPieceText));
        Assert.Equal([null, true], held.CExamplePiece.Select(piece => piece.CMentionPieceLinked));
    }

    [Fact]
    public void AnthologyExampleRead_UnknownText_LeavesExcerptUnlinked()
    {
        LExample example = TInterfaceExample.TExampleCreate(
                5,
                "English",
                TInterfaceState.TStateValueResolve(null, true),
                null,
                TInterfaceState.TStateAnchorRead(null))
            .TExampleMentionAdd(TInterfaceMentionSpan.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceConductPanel.TAnthologyExampleRead(example, string.Empty, string.Empty);

        Assert.NotNull(held);
        Assert.True(held!.CExampleText.CStateValueUncertain);
        Assert.Equal("Display.Unknown", held.CExampleTextHint);
        Assert.Equal("Display.Unknown", held.CExampleWording);
        Assert.False(held.CExampleMuted);
        Assert.Null(held.CExampleSource);
        Assert.Empty(held.CExamplePiece);
    }

    [Fact]
    public void AnthologyExampleRead_UnwrittenText_WordsItUnwrittenAndMuted()
    {
        LExample example = TInterfaceExample.TExampleCreate(
            5,
            "English",
            TInterfaceState.TStateValueCreate(string.Empty),
            null,
            TInterfaceState.TStateAnchorRead(null));

        CExample? held = TInterfaceConductPanel.TAnthologyExampleRead(example, string.Empty, string.Empty);

        Assert.NotNull(held);
        Assert.Equal("Example.Unwritten", held!.CExampleWording);
        Assert.True(held.CExampleMuted);
    }
}
