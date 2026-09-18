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
        LStateValue text = TInterface.TStateValueCreate("same");
        LExample example = engine.TEngineExampleCreate(TInterface.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        engine.TEngineExampleCreate(TInterface.TExampleCreate(0, "French", text, null, LStateAnchor.LStateAnchorUnspecified));
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LReference reference = engine.TEngineCitationCreate("Source");
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Writer"));
        LTag tag = engine.TEngineTagCreate("Tag");
        LRegister register = engine.TEngineRegisterCreate("Register");

        LVista corpus = TCatalogVistaCreate(engine, "corpus", example.LExampleId);
        IReadOnlyList<LCatalogExample> examples = engine.TEngineExampleFind(corpus);
        Assert.Equal(example.LExampleId, Assert.Single(examples, row => row.LCatalogExampleChosen).LCatalogExampleStored.LExampleId);
        Assert.Equal(["same (1)", "same (2)"], examples.Select(row => row.LCatalogExampleName));
        corpus.TVistaSelect(null);
        Assert.All(engine.TEngineExampleFind(corpus), row => Assert.False(row.LCatalogExampleChosen));

        LCatalogSituation situationRow = Assert.Single(engine.TEngineSituationFind(TCatalogVistaCreate(engine, "repertoire", situation.LSituationId)));
        Assert.True(situationRow.LCatalogSituationChosen);
        Assert.Equal("same", situationRow.LCatalogSituationName);
        LCatalogReference referenceRow = Assert.Single(engine.TEngineReferenceFind(TCatalogVistaCreate(engine, "reference", reference.LReferenceId)), row => row.LCatalogReferenceChosen);
        Assert.True(referenceRow.LCatalogReferenceChosen);
        Assert.Equal("Source", referenceRow.LCatalogReferenceName);
        LCatalogAuthor authorRow = Assert.Single(engine.TEngineAuthorFind(TCatalogVistaCreate(engine, "guild", author.LAuthorId)));
        Assert.True(authorRow.LCatalogAuthorChosen);
        Assert.Equal("Writer", authorRow.LCatalogAuthorName);
        Assert.True(Assert.Single(engine.TEngineTagFind(TCatalogVistaCreate(engine, "taxonomy", tag.LTagId))).LCatalogTagChosen);
        Assert.True(Assert.Single(engine.TEngineRegisterFind(TCatalogVistaCreate(engine, "tenor", register.LRegisterId))).LCatalogRegisterChosen);
    }

    [Fact]
    public void Find_UnknownExamples_TwinsTheLocalizedFallback()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineExampleCreate(TInterface.TExampleCreate(0, "English", LStateValue.LStateValueUnknown, null, LStateAnchor.LStateAnchorUnspecified));
        engine.TEngineExampleCreate(TInterface.TExampleCreate(0, "French", LStateValue.LStateValueUnknown, null, LStateAnchor.LStateAnchorUnspecified));
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        Assert.Equal(["Unknown (1)", "Unknown (2)"], engine.TEngineExampleFind(vista, "Unknown", "Unwritten")
            .Select(row => row.LCatalogExampleName));
    }

    [Fact]
    public void Find_ChildVista_UsesParentSelectionAndChildQuery()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LCardDraft card = TInterface.TCardCreate("liquid", 1) with { LCardDraftTag = TInterface.TTagDraftCreate("fluid") };
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

    private static LVista TCatalogVistaCreate(LEngine engine, string tab, long id)
    {
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(id);
        return vista;
    }
}
