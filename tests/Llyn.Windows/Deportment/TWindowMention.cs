using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowMention
{
    [Fact]
    public void WindowMentionDivide_FakeDraftPort_ReturnsSpanPieces()
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionDivide"] = args =>
                TInterface.TMentionSpanDivide((string)args![0]!, (IReadOnlyList<LMention>)args[1]!),
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(drafts);
        IReadOnlyList<CMention> mentions = [new CMention(1, 4, 3, 7, 0)];

        IReadOnlyList<CMentionPiece> pieces = window.TWindowMentionDivide("the cat sat", mentions);

        Assert.Equal(3, pieces.Count);
        Assert.Equal("cat", pieces[1].CMentionPieceText);
        Assert.Equal(4, pieces[1].CMentionPieceOffset);
        Assert.Equal(7, pieces[1].CMentionPieceEnd);
        Assert.Null(pieces[0].CMentionPieceLinked);
        Assert.True(pieces[1].CMentionPieceLinked);
    }
}
