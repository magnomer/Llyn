using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCatalogChosen
{
    [Fact]
    public void Find_CatalogRows_CarrySelectionAndTwinnedNames()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterfaceState.TStateValueCreate("same");
        LExample example = engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "French", text, null, LStateAnchor.LStateAnchorUnspecified));
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LReference reference = engine.TEngineCitationCreate("Source");
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Writer"));
        LTag tag = engine.TEngineTagCreate("Tag");
        LRegister register = engine.TEngineRegisterCreate("Register");

        LVista corpus = TCatalogVistaCreate(engine, "corpus", example.LExampleId);
        IReadOnlyList<LCatalogExample> examples = engine.TEngineExampleFind(corpus);
        LCatalogExample chosen = Assert.Single(examples, row => row.LCatalogExampleChosen);
        Assert.Equal(example.LExampleId, chosen.LCatalogExampleStored.LExampleId);
        Assert.Equal(["same (1)", "same (2)"], examples.Select(row => row.LCatalogExampleName));
        corpus.TVistaSelect(null);
        Assert.All(engine.TEngineExampleFind(corpus), row => Assert.False(row.LCatalogExampleChosen));

        LVista situationVista = TCatalogVistaCreate(engine, "repertoire", situation.LSituationId);
        LCatalogSituation situationRow = Assert.Single(engine.TEngineSituationFind(situationVista));
        Assert.True(situationRow.LCatalogSituationChosen);
        Assert.Equal("same", situationRow.LCatalogSituationName);
        LVista referenceVista = TCatalogVistaCreate(engine, "reference", reference.LReferenceId);
        LCatalogReference referenceRow = Assert.Single(
            engine.TEngineReferenceFind(referenceVista), row => row.LCatalogReferenceChosen);
        Assert.True(referenceRow.LCatalogReferenceChosen);
        Assert.Equal("Source", referenceRow.LCatalogReferenceName);
        LVista authorVista = TCatalogVistaCreate(engine, "guild", author.LAuthorId);
        LCatalogAuthor authorRow = Assert.Single(engine.TEngineAuthorFind(authorVista));
        Assert.True(authorRow.LCatalogAuthorChosen);
        Assert.Equal("Writer", authorRow.LCatalogAuthorName);
        LVista tagVista = TCatalogVistaCreate(engine, "taxonomy", tag.LTagId);
        Assert.True(Assert.Single(engine.TEngineTagFind(tagVista)).LCatalogTagChosen);
        LVista registerVista = TCatalogVistaCreate(engine, "tenor", register.LRegisterId);
        Assert.True(Assert.Single(engine.TEngineRegisterFind(registerVista)).LCatalogRegisterChosen);
    }

    [Fact]
    public void Find_UnknownExamples_TwinsTheLocalizedFallback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue unknown = LStateValue.LStateValueUnknown;
        LStateAnchor anchor = LStateAnchor.LStateAnchorUnspecified;
        engine.TEngineExampleCreate(TInterfaceExample.TExampleCreate(0, "English", unknown, null, anchor));
        engine.TEngineExampleCreate(TInterfaceExample.TExampleCreate(0, "French", unknown, null, anchor));
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        Assert.Equal(["Unknown (1)", "Unknown (2)"], engine.TEngineExampleFind(vista, "Unknown", "Unwritten")
            .Select(row => row.LCatalogExampleName));
    }

    [Fact]
    public void Find_ChildVista_UsesParentSelectionAndChildQuery()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LTagDraft> fluid = TInterface.TTagDraftCreate("fluid");
        LCardDraft card = TInterface.TCardCreate("liquid", 1) with { LCardDraftTag = fluid };
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [card], []));
        LVista parent = TCatalogVistaCreate(engine, "taxonomy", long.MaxValue);
        LVista child = TCatalogVistaCreate(engine, "membership", entry.LEntryId);
        Assert.Empty(engine.TEngineEntryFind(parent, child));
        LTag tag = Assert.Single(engine.TEngineTagFind("fluid", LCatalogOrder.LCatalogOrderName));
        parent.TVistaSelect(tag.LTagId);
        LVistaRow row = Assert.Single(engine.TEngineEntryFind(parent, child));
        Assert.Equal(entry.LEntryId, row.LVistaRowId);
        Assert.True(row.LVistaRowChosen);
        child.TVistaQuerySet("no-such-word");
        Assert.Empty(engine.TEngineEntryFind(parent, child));
    }

    [Fact]
    public void Find_ChildVista_ListsInChildOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista parent = TCatalogTagSave(engine, ("apple", "English"), ("water", "English"));
        LVista child = engine.TEngineVistaStart("membership", LCatalogOrder.LCatalogOrderReverse);
        Assert.Equal(
            ["water", "apple"],
            engine.TEngineEntryFind(parent, child).Select(row => row.LVistaRowHeadword));
    }

    [Fact]
    public void Find_ChildVistaEqualHeadwords_ListsByLanguageThenId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista parent = TCatalogTagSave(engine, ("bank", "French"), ("bank", "English"));
        LVista child = engine.TEngineVistaStart("membership", LCatalogOrder.LCatalogOrderName);
        Assert.Equal(
            ["English", "French"],
            engine.TEngineEntryFind(parent, child).Select(row => row.LVistaRowLanguage));
    }

    private static LVista TCatalogTagSave(LEngine engine, params (string, string)[] entries)
    {
        long tagId = 0;
        foreach ((string headword, string language) in entries)
        {
            LTagDraft fluid = TInterface.TTagDraftCreate("fluid")[0] with { LTagDraftId = tagId };
            LCardDraft card = TInterface.TCardCreate("liquid", 1) with { LCardDraftTag = [fluid] };
            engine.TEngineEntrySave(TInterface.TEntryDraftCreate(headword, language, "", "", [card], []));
            tagId = Assert.Single(engine.TEngineTagFind("fluid", LCatalogOrder.LCatalogOrderName)).LTagId;
        }

        return TCatalogVistaCreate(engine, "taxonomy", tagId);
    }

    private static LVista TCatalogVistaCreate(LEngine engine, string tab, long id)
    {
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(id);
        return vista;
    }
}
