using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexRow
{
    [Fact]
    public void ReflexLanguageKey_NamedOrBlank_PrefixesTheReflexKey()
    {
        CReflex named = TReflexRowCreate("Wu", "Go-on", false);
        CReflex blank = TReflexRowCreate(string.Empty, string.Empty, false);

        Assert.Equal("Reflex.Wu", named.CReflexLanguageKey);
        Assert.Equal("Reflex.Go-on", named.CReflexKindKey);
        Assert.Equal("Reflex.", blank.CReflexLanguageKey);
        Assert.Equal("Reflex.", blank.CReflexKindKey);
    }

    [Fact]
    public void ReflexLeadRead_ThreeRuns_LeadsEachRunOnce()
    {
        Assert.Equal([true, false, true, true], TInterfaceConductSound.TReflexLeadRead(["Wu", "Wu", "Jin", "Wu"]));
        Assert.Equal([true, true], TInterfaceConductSound.TReflexLeadRead(["Wu", "Wu "]));
        Assert.Empty(TInterfaceConductSound.TReflexLeadRead([]));
    }

    [Fact]
    public void ReflexHiddenCheck_FoldAndOpening_HidesAFoldedRowWhileClosed()
    {
        Assert.True(TReflexRowCreate("Jin", string.Empty, true).CReflexHiddenCheck(false));
        Assert.False(TReflexRowCreate("Jin", string.Empty, true).CReflexHiddenCheck(true));
        Assert.False(TReflexRowCreate("Wu", string.Empty, false).CReflexHiddenCheck(false));
    }

    [Fact]
    public void ReflexTypedApply_TypedCell_ChangesThatCellAlone()
    {
        CReflex held = TReflexRowCreate("Wu", "Go-on", true);

        CReflex language = held.CReflexTypedApply(CReflexField.CReflexFieldLanguage, "Jin");
        CReflex note = held.CReflexTypedApply(CReflexField.CReflexFieldNote, "literary");

        Assert.Equal(held with { CReflexLanguage = "Jin" }, language);
        Assert.Equal("Reflex.Jin", language.CReflexLanguageKey);
        Assert.Equal(held with { CReflexNote = "literary" }, note);
        Assert.Equal("Wu", held.CReflexLanguage);
        Assert.True(note.CReflexLead);
    }

    private static CReflex TReflexRowCreate(string language, string kind, bool folded)
    {
        return new CReflex(
            1, language, kind, "ipa", string.Empty, string.Empty, string.Empty, false, string.Empty, [],
            new CRespellingMark(false, string.Empty, string.Empty), folded, true);
    }
}
