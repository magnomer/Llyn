using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorSeal
{
    [Fact]
    public void SoundingParadigmRead_TwoSlotsOneForm_JoinsThemIntoOneRow()
    {
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LSpeechValue verb = TInterface.TSpeechValueCreate("English", 2, "verb", 1) with { LSpeechValueId = 2 };
        LMorphology plural = TInterface.TMorphologyCreate(1, 1, "plural", 0);
        LMorphology past = TInterface.TMorphologyCreate(2, 1, "past", 0);
        LInflection wolves = TInterface.TInflectionCreate(1, 0, "wolves", null, 1, [1]);
        IReadOnlyList<LParadigmSlot> slots =
        [
            TInterface.TParadigmSlotCreate(noun, plural, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(noun, past, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(verb, past, null, LState.LStateUnknown),
        ];

        IReadOnlyList<CParadigmSlot> rows = TInterfaceDeportment.TSoundingParadigmRead(slots);

        Assert.Equal(
            [
                new CParadigmSlot("noun", "plural, past", "wolves", false),
                new CParadigmSlot("verb", "past", null, true),
            ],
            rows);
    }

    [Fact]
    public void SoundingFrequencyRead_NoRows_ReturnsNone()
    {
        Assert.Null(TInterfaceDeportment.TSoundingFrequencyRead([], "once"));
    }
}
