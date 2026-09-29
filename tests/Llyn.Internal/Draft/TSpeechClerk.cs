using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSpeechClerk
{
    [Fact]
    public void SpeechParse_TypedText_AppendsItTrimmedOnce()
    {
        IReadOnlyList<LSpeechDraft> held = [TInterface.TSpeechCreate(5, "Noun")];

        Assert.Equal(["Noun", "verb"], TInterface.TSpeechParse(held, " verb ").Select(TSpeechNameRead));
        Assert.Same(held, TInterface.TSpeechParse(held, "noun"));
        Assert.Same(held, TInterface.TSpeechParse(held, "   "));
    }

    [Fact]
    public void SpeechAdd_DeclinedName_KeepsItAsTyped()
    {
        LSpeechDraft added = Assert.Single(TInterface.TSpeechAdd([], " Noun ", static _ => null)!);

        Assert.Equal("Noun", added.LSpeechDraftName);
        Assert.Equal(0, added.LSpeechDraftValue);
    }

    [Fact]
    public void SpeechAdd_DeclaredName_CarriesTheCatalogValue()
    {
        List<string> declared = [];

        LSpeechDraft added = Assert.Single(TInterface.TSpeechAdd([], "noun", name =>
        {
            declared.Add(name);
            return TInterface.TSpeechValueCreate(7, "Noun");
        })!);

        Assert.Equal(["noun"], declared);
        Assert.Equal((7L, "Noun"), (added.LSpeechDraftValue, added.LSpeechDraftName));
    }

    [Fact]
    public void SpeechAdd_BlankOrHeldName_DeclaresNothing()
    {
        IReadOnlyList<LSpeechDraft> held = [TInterface.TSpeechCreate(5, "Noun")];
        List<string> declared = [];

        Assert.Null(TInterface.TSpeechAdd(held, "  ", name => { declared.Add(name); return null; }));
        Assert.Same(held, TInterface.TSpeechAdd(held, "NOUN", name => { declared.Add(name); return null; }));
        Assert.Empty(declared);
    }

    [Fact]
    public void SpeechRemove_ShownName_DropsThatChipOnly()
    {
        IReadOnlyList<LSpeechDraft> held = [TInterface.TSpeechCreate(5, "Noun"), TInterface.TSpeechCreate(0, "verb")];

        Assert.Equal(["verb"], TInterface.TSpeechRemove(held, "Noun").Select(TSpeechNameRead));
    }

    [Fact]
    public void SpeechSettle_DraftAgrees_KeepsTheTypedText()
    {
        IReadOnlyList<LSpeechDraft> held = [TInterface.TSpeechCreate(5, "Noun")];
        IReadOnlyList<LSpeechDraft> shown = [TInterface.TSpeechCreate(5, "Noun"), TInterface.TSpeechCreate(0, "ver")];

        (IReadOnlyList<LSpeechDraft> kept, string typed) = TInterface.TSpeechSettle(shown, held, "ver");

        Assert.Same(held, kept);
        Assert.Equal("ver", typed);
    }

    [Fact]
    public void SpeechSettle_DraftChangedElsewhere_TakesItsCleanedPartsAndClearsTheText()
    {
        IReadOnlyList<LSpeechDraft> shown =
        [
            TInterface.TSpeechCreate(5, " Noun "),
            TInterface.TSpeechCreate(0, "noun"),
            TInterface.TSpeechCreate(0, "  "),
            TInterface.TSpeechCreate(0, "verb"),
        ];

        (IReadOnlyList<LSpeechDraft> kept, string typed) = TInterface.TSpeechSettle(shown, [], "adj");

        Assert.Equal(
            [(5L, "Noun"), (0L, "verb")], kept.Select(static row => (row.LSpeechDraftValue, row.LSpeechDraftName)));
        Assert.Equal(string.Empty, typed);
    }

    [Fact]
    public void SpeechPendingRead_TypedText_NamesOnlyAnAppendedPart()
    {
        IReadOnlyList<LSpeechDraft> held = [TInterface.TSpeechCreate(0, "noun")];

        Assert.Equal("verb", TInterface.TSpeechPendingRead(held, " verb "));
        Assert.Null(TInterface.TSpeechPendingRead(held, "Noun"));
        Assert.Null(TInterface.TSpeechPendingRead(held, "  "));
    }

    [Fact]
    public void SpeechChipRead_PendingLast_LeavesItOutOfTheChips()
    {
        IReadOnlyList<LSpeechDraft> shown = [TInterface.TSpeechCreate(0, "noun"), TInterface.TSpeechCreate(0, "verb")];

        Assert.Equal(["noun"], TInterface.TSpeechChipRead(shown, "verb").Select(TSpeechNameRead));
        Assert.Equal(["noun", "verb"], TInterface.TSpeechChipRead(shown, "noun").Select(TSpeechNameRead));
        Assert.Equal(["noun", "verb"], TInterface.TSpeechChipRead(shown, null).Select(TSpeechNameRead));
    }

    private static string TSpeechNameRead(LSpeechDraft speech) => speech.LSpeechDraftName;
}
