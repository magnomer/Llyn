using System;
using System.Collections.Generic;
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
        IReadOnlyList<LMention> mentions = [TInterface.TMentionCreate(1, 4, 3, 7)];

        IReadOnlyList<LMentionPiece> pieces = window.TWindowMentionDivide("the cat sat", mentions);

        Assert.Equal(TInterface.TMentionSpanDivide("the cat sat", mentions), pieces);
        Assert.Equal(3, pieces.Count);
        Assert.Equal(4, pieces[1].LMentionPieceOffset);
        Assert.True(pieces[1].LMentionPieceStored!.LMentionLinked);
    }
}
