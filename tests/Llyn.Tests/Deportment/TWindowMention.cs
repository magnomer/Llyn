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

    [Fact]
    public void WindowAnchorToggle_FakeDraftPort_ForwardsArguments()
    {
        List<(IReadOnlyList<long>, long, bool)> asked = [];
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineAnchorToggle"] = args =>
            {
                asked.Add(((IReadOnlyList<long>)args![0]!, (long)args[1]!, (bool)args[2]!));
                return TInterface.TAnchorToggle((IReadOnlyList<long>)args[0]!, (long)args[1]!, (bool)args[2]!);
            },
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(drafts);

        IReadOnlyList<long> anchors = window.TWindowAnchorToggle([3], 5, true);

        Assert.Equal([3, 5], anchors);
        Assert.Single(asked);
        Assert.Equal(5, asked[0].Item2);
    }
}
