using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierMention
{
    [Fact]
    public void MentionDivide_LinkedMention_ReturnsSpanPieces()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine, []);

        IReadOnlyList<CMentionPiece> pieces =
            atelier.CAtelierMention.CMentionDivide("the cat sat", [new CMentionMark(1, 4, 3, 7, 0)]);

        Assert.Equal(3, pieces.Count);
        Assert.Equal("cat", pieces[1].CMentionPieceText);
        Assert.Equal(4, pieces[1].CMentionPieceOffset);
        Assert.Equal(7, pieces[1].CMentionPieceEnd);
        Assert.Null(pieces[0].CMentionPieceLinked);
        Assert.True(pieces[1].CMentionPieceLinked);
    }

    [Fact]
    public void MentionResolve_UnlinkedLabel_ReadsSilentName()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(
            engine,
            [
                TInterface.TMentionLabelCreate(1, "cat", 7, "Cat", "animal"),
                TInterface.TMentionLabelCreate(2, "Tom", 0, "Tom", string.Empty),
            ]);

        IReadOnlyList<CMentionLabel> labels = atelier.CAtelierMention.CMentionResolve(
            "the cat Tom",
            [new CMentionDraft(1, 4, 3, 7, 0), new CMentionDraft(2, 8, 3, 0, 0)],
            "silent");

        Assert.Equal(new CMentionLabel(1, "cat", "Cat", "animal"), labels[0]);
        Assert.Equal(new CMentionLabel(2, "Tom", "silent", string.Empty), labels[1]);
    }

    [Fact]
    public void MentionSpanRead_Selection_ReadsCodePointSpan()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine, []);

        (int offset, int length) = atelier.CAtelierMention.CMentionSpanRead("\U0001F600 cat", 3, 3);

        Assert.Equal((2, 3), (offset, length));
    }

    [Fact]
    public void MentionOffsetRead_UnitRead_RoundTrips()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TAtelierMentionCreate(engine, []);
        string text = "\U0001F600 cat";

        int unit = atelier.CAtelierMention.CMentionUnitRead(text, 2);

        Assert.Equal(3, unit);
        Assert.Equal(2, atelier.CAtelierMention.CMentionOffsetRead(text, unit));
    }

    [Fact]
    public void MentionMarkRead_Drafts_ReadsStoredMarks()
    {
        IReadOnlyList<CMentionMark> marks = CMention.CMentionMarkRead(
            [TInterface.TMentionDraftCreate(3, 4, 3, 7, 9)]);

        Assert.Equal([new CMentionMark(3, 4, 3, 7, 9)], marks);
    }

    [Fact]
    public void MentionMarkRead_NoDrafts_ReadsEmpty()
    {
        Assert.Empty(CMention.CMentionMarkRead(null));
    }

    private static CAtelier TAtelierMentionCreate(LEngine engine, IReadOnlyList<LMentionLabel> labels)
    {
        return TInterfaceConduct.TAtelierCreate(engine, new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => labels,
            ["LEngineMentionDivide"] = args =>
                TInterface.TMentionSpanDivide((string)args![0]!, (IReadOnlyList<LMention>)args[1]!),
            ["LEngineSpanRead"] = args =>
                TInterface.TMentionSpanRead((string)args![0]!, (int)args[1]!, (int)args[2]!),
            ["LEngineUnitRead"] = args =>
                TInterface.TMentionUnitRead((string)args![0]!, (int)args[1]!),
            ["LEngineOffsetRead"] = args =>
                TInterface.TMentionOffsetRead((string)args![0]!, (int)args[1]!),
        });
    }
}
