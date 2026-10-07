using System;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TUnit
{
    [Fact]
    public void EntrySave_ChosenUnit_ReloadsUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(TUnitDraftCreate() with { LEntryDraftUnit = LUnit.LUnitMorpheme });

        Assert.Equal(LUnit.LUnitMorpheme, engine.TEngineEntryRead(entry.LEntryId)!.LEntryUnit);
        Assert.Equal(LUnit.LUnitMorpheme, engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftUnit);
    }

    [Fact]
    public void MarkupFormat_ChosenUnit_ParsesUnit()
    {
        LMarkupEntry entry = TInterface.TMarkupEntryCreate("cat", "English", unit: LUnit.LUnitFunction);

        string text = TInterface.TMarkupFormat([entry]);

        Assert.Contains("unit", text);
        Assert.Equal(LUnit.LUnitFunction, Assert.Single(TInterface.TMarkupParse(text)).LMarkupEntryUnit);
    }

    [Fact]
    public void MarkupFormat_EmptyUnit_WritesNoUnit()
    {
        string text = TInterface.TMarkupFormat([TInterface.TMarkupEntryCreate("cat", "English")]);

        Assert.DoesNotContain("unit", text);
        Assert.Equal(LUnit.LUnitEmpty, Assert.Single(TInterface.TMarkupParse(text)).LMarkupEntryUnit);
    }

    [Fact]
    public void LanguageSet_UnspacedLanguage_TurnsContentIntoWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);
        tenure.TTenureLanguageSet("English");
        tenure.TTenureUnitSet(LUnit.LUnitContent);

        tenure.TTenureLanguageSet("Mandarin");

        Assert.Equal(LUnit.LUnitWord, tenure.TTenureUnitRead());
    }

    [Fact]
    public void LanguageSet_SpacedLanguage_ClearsWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);
        tenure.TTenureLanguageSet("Thai");
        tenure.TTenureUnitSet(LUnit.LUnitWord);

        tenure.TTenureLanguageSet("English");

        Assert.Equal(LUnit.LUnitEmpty, tenure.TTenureUnitRead());
    }

    [Fact]
    public void UnitSet_HeldUnit_ClearsUnit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);
        tenure.TTenureLanguageSet("English");
        tenure.TTenureUnitSet(LUnit.LUnitMorpheme);

        tenure.TTenureUnitSet(LUnit.LUnitMorpheme);

        Assert.Equal(LUnit.LUnitEmpty, tenure.TTenureUnitRead());
    }

    [Fact]
    public void UnitScan_Language_FollowsSpacing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);

        tenure.TTenureLanguageSet("English");
        Assert.Equal([LUnit.LUnitContent, LUnit.LUnitFunction, LUnit.LUnitMorpheme], tenure.TTenureUnitScan());

        tenure.TTenureLanguageSet("Thai");
        Assert.Equal([LUnit.LUnitWord, LUnit.LUnitMorpheme], tenure.TTenureUnitScan());
    }

    [Fact]
    public void LanguageRead_ShippedPacks_ReadSpacing()
    {
        LLanguageVault languages = TInterface.TLanguageVaultCreate();

        Assert.True(languages.TLanguageRead("English").LLanguageSpaced);
        Assert.False(languages.TLanguageRead("Thai").LLanguageSpaced);
        Assert.False(languages.TLanguageRead("Vietnamese").LLanguageSpaced);
        Assert.True(languages.TLanguageRead("Vietnamese").LLanguageSeparated);
        Assert.False(languages.TLanguageRead("Mandarin").LLanguageSpaced);
    }

    [Fact]
    public void LiveryFormat_ChosenUnit_LeadsTheSpeechChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "the",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("definite article", 1)],
            [],
            speeches: ["Determiner"]) with { LEntryDraftUnit = LUnit.LUnitFunction });
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<span class=\"llyn-unit\">Unit.Function</span> <span class=\"llyn-speech\">Determiner</span>",
            body,
            StringComparison.Ordinal);
    }

    private static LEntryDraft TUnitDraftCreate()
    {
        return TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)],
            []);
    }

    private static LTenure TUnitTenureStart(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        return engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
    }
}
