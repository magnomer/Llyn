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
    public void EntrySave_ChosenFreedom_ReloadsFreedom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry entry = engine.TEngineEntrySave(
            TUnitDraftCreate() with { LEntryDraftFreedom = LFreedom.LFreedomStandalone });

        Assert.Equal(LFreedom.LFreedomStandalone, engine.TEngineEntryRead(entry.LEntryId)!.LEntryFreedom);
        Assert.Equal(LFreedom.LFreedomStandalone, engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftFreedom);
    }

    [Fact]
    public void MarkupFormat_ChosenUnit_ParsesUnit()
    {
        LMarkupEntry entry = TInterface.TMarkupEntryCreate("run away", "English", unit: LUnit.LUnitExpression);

        string text = TInterface.TMarkupFormat([entry]);

        Assert.Contains("<unit>expression</unit>", text, StringComparison.Ordinal);
        Assert.Equal(LUnit.LUnitExpression, Assert.Single(TInterface.TMarkupParse(text)).LMarkupEntryUnit);
    }

    [Fact]
    public void MarkupFormat_ChosenFreedom_ParsesFreedom()
    {
        LMarkupEntry entry = TInterface.TMarkupEntryCreate("un", "English", freedom: LFreedom.LFreedomDependent);

        string text = TInterface.TMarkupFormat([entry]);

        Assert.Contains("<freedom>dependent</freedom>", text, StringComparison.Ordinal);
        Assert.Equal(LFreedom.LFreedomDependent, Assert.Single(TInterface.TMarkupParse(text)).LMarkupEntryFreedom);
    }

    [Fact]
    public void MarkupFormat_EmptyUnitAndFreedom_WritesNeither()
    {
        string text = TInterface.TMarkupFormat([TInterface.TMarkupEntryCreate("cat", "English")]);

        Assert.DoesNotContain("unit", text, StringComparison.Ordinal);
        Assert.DoesNotContain("freedom", text, StringComparison.Ordinal);
        LMarkupEntry parsed = Assert.Single(TInterface.TMarkupParse(text));
        Assert.Equal(LUnit.LUnitEmpty, parsed.LMarkupEntryUnit);
        Assert.Equal(LFreedom.LFreedomEmpty, parsed.LMarkupEntryFreedom);
    }

    [Fact]
    public void MarkupParse_RetiredUnit_LeavesUnitEmpty()
    {
        string text = TInterface.TMarkupFormat([TInterface.TMarkupEntryCreate("the", "English", unit: LUnit.LUnitWord)])
            .Replace("<unit>word</unit>", "<unit>function</unit>", StringComparison.Ordinal);

        Assert.Equal(LUnit.LUnitEmpty, Assert.Single(TInterface.TMarkupParse(text)).LMarkupEntryUnit);
    }

    [Fact]
    public void LanguageSet_OtherLanguage_KeepsUnitAndFreedom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);
        tenure.TTenureLanguageSet("English");
        tenure.TTenureUnitSet(LUnit.LUnitWord);
        tenure.TTenureFreedomSet(LFreedom.LFreedomDependent);

        tenure.TTenureLanguageSet("Thai");

        Assert.Equal(LUnit.LUnitWord, tenure.TTenureUnitRead());
        Assert.Equal(LFreedom.LFreedomDependent, tenure.TTenureFreedomRead());
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
    public void FreedomSet_HeldFreedom_ClearsFreedom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);
        tenure.TTenureLanguageSet("English");
        tenure.TTenureFreedomSet(LFreedom.LFreedomStandalone);

        tenure.TTenureFreedomSet(LFreedom.LFreedomStandalone);

        Assert.Equal(LFreedom.LFreedomEmpty, tenure.TTenureFreedomRead());
    }

    [Fact]
    public void UnitScan_AnyLanguage_OffersMorphemeWordAndExpression()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);

        tenure.TTenureLanguageSet("English");
        Assert.Equal([LUnit.LUnitMorpheme, LUnit.LUnitWord, LUnit.LUnitExpression], tenure.TTenureUnitScan());

        tenure.TTenureLanguageSet("Thai");
        Assert.Equal([LUnit.LUnitMorpheme, LUnit.LUnitWord, LUnit.LUnitExpression], tenure.TTenureUnitScan());
    }

    [Fact]
    public void FreedomScan_AnyLanguage_OffersDependentAndStandalone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TUnitTenureStart(engine);

        tenure.TTenureLanguageSet("Mandarin");

        Assert.Equal([LFreedom.LFreedomDependent, LFreedom.LFreedomStandalone], tenure.TTenureFreedomScan());
    }

    [Fact]
    public void LiveryFormat_ChosenUnitAndFreedom_LeadTheSpeechChips()
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
            speeches: ["Determiner"]) with
        {
            LEntryDraftUnit = LUnit.LUnitWord,
            LEntryDraftFreedom = LFreedom.LFreedomDependent,
        });
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<span class=\"llyn-unit\">Unit.Word</span> <span class=\"llyn-freedom\">Freedom.Dependent</span> " +
            "<span class=\"llyn-speech\">Determiner</span>",
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
