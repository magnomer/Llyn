using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TCustomsLoss
{
    [Fact]
    public void CustomsCardScan_NestedChildren_CountsEveryCard()
    {
        LCardDraft leaf = TInterface.TDraftCardCreate("leaf");
        LCardDraft parent = TInterface.TDraftCardCreate("parent") with { LCardDraftChild = [leaf, leaf] };

        Assert.Equal(4, TInterfaceDeportment.TCustomsCardScan([parent, leaf]));
    }

    [Fact]
    public void CustomsIntakeCreate_OneCandidate_MergesIntoIt()
    {
        LMarkupIntake first = TInterfaceDeportment.TCustomsIntakeCreate(
            2, [TInterface.TEntryCreate(7, "cat", "English", 0, null, null)]);

        Assert.Equal((2, LMarkupMode.LMarkupModeMerge, 7L),
            (first.LMarkupIntakeIndex, first.LMarkupIntakeMode, first.LMarkupIntakeTarget));
    }

    [Fact]
    public void CustomsIntakeCreate_NewRowHoldingAPick_SendsNoTarget()
    {
        LMarkupIntake fresh = TInterfaceDeportment.TCustomsIntakeCreate(1, LMarkupMode.LMarkupModeNew, 7);
        LMarkupIntake merged = TInterfaceDeportment.TCustomsIntakeCreate(1, LMarkupMode.LMarkupModeMerge, 7);

        Assert.Equal(0, fresh.LMarkupIntakeTarget);
        Assert.Equal(7, merged.LMarkupIntakeTarget);
    }

    [Fact]
    public void CustomsReadyCheck_MergeWithoutTarget_IsFalse()
    {
        Assert.False(TInterfaceDeportment.TCustomsReadyCheck(LMarkupMode.LMarkupModeMerge, 0));
        Assert.True(TInterfaceDeportment.TCustomsReadyCheck(LMarkupMode.LMarkupModeNew, 0));
    }

    [Fact]
    public void CustomsLossResolve_ReplaceStoredEntry_CountsItsCards()
    {
        LEntryDraft stored = TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TDraftCardCreate("pet"), TInterface.TDraftCardCreate("feline")],
            [TInterface.TDraftCardCreate("cat nap")]);
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(
            new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEntryLoad"] = _ => stored,
        });
        LWindow window = TInterfaceDeportment.TWindowCreate(entries);
        LMarkupMode replace = LMarkupMode.LMarkupModeReplace;

        Assert.Equal("2/1", TInterfaceDeportment.TCustomsLossResolve(window, replace, 5));
        Assert.Empty(TInterfaceDeportment.TCustomsLossResolve(window, LMarkupMode.LMarkupModeMerge, 5));
        Assert.Empty(TInterfaceDeportment.TCustomsLossResolve(window, replace, 0));
        Assert.Empty(TInterfaceDeportment.TCustomsLossResolve(null, replace, 5));
    }
}
