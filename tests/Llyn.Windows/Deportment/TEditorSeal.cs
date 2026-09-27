using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorSeal
{
    [Fact]
    public void CardStateRead_UnknownValue_CarriesTheUncertainVerdict()
    {
        Assert.Equal(
            new CStateValue(string.Empty, true, false),
            TInterfaceDeportment.TCardStateRead(LStateValue.LStateValueUnknown));
    }

    [Fact]
    public void CardEntryRead_ThreePronunciations_SplitsThePrimaryFromTheAccents()
    {
        LEntryDraft draft = TInterface.TEntryDraftCreate("colour", "English", "ˈkʌlə", "a note", [], []) with
        {
            LEntryDraftPronunciations =
            [
                TInterface.TPronunciationDraftCreate("ˈkʌlə", "RP"),
                TInterface.TPronunciationDraftCreate("ˈkʌlɚ", "GA"),
                TInterface.TPronunciationDraftCreate(string.Empty, "AU"),
            ],
        };

        CEntryDraft shaped = TInterfaceDeportment.TCardEntryRead(draft);

        Assert.Equal("RP", shaped.CEntryDraftPronunciation?.CPronunciationDraftVariety);
        Assert.Equal(["GA", "AU"], shaped.CEntryDraftAccents.Select(spoken => spoken.CPronunciationDraftVariety));
        Assert.False(shaped.CEntryDraftReflected);
    }

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
