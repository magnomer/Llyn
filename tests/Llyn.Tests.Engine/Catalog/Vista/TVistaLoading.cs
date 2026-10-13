using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TVistaLoading
{
    [Theory]
    [InlineData("yunjing")]
    [InlineData("yunmu")]
    public void Delete_StructuralVista_DoesNotTreatItsSelectionAsAnEntry(string tab)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(entry.LEntryId);
        Assert.Null(vista.LVistaSubject);
        Assert.Null(engine.TVistaLoad(vista));
        Assert.Null(engine.TVistaDelete(vista));
        Assert.NotNull(engine.TEngineEntryLoad(entry.LEntryId));
    }

    [Fact]
    public void FileRead_NoVista_ReturnsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        Assert.Equal("entry", engine.TVistaFileRead(null));
    }

    [Fact]
    public void FileRead_ChosenEntry_ReturnsCleanedHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("wa/ter", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(entry.LEntryId);

        Assert.Equal("wa_ter", engine.TVistaFileRead(vista));
    }

    [Theory]
    [InlineData("library")]
    [InlineData("phonology")]
    [InlineData("favorite")]
    [InlineData("quotation")]
    [InlineData("footnote")]
    [InlineData("occurrence")]
    [InlineData("membership")]
    [InlineData("cohort")]
    [InlineData("xiaoyun")]
    public void Load_EntryVista_UsesChosenAndDeleteClearsIt(string tab)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderHeadword);
        Assert.Null(engine.TVistaLoad(vista));
        Assert.Null(engine.TVistaDelete(vista));

        vista.TVistaSelect(entry.LEntryId);
        Assert.Equal("water", engine.TVistaLoad(vista)!.LDraftContent.LEntryDraftHeadword);
        Assert.NotNull(engine.TVistaDelete(vista));
        Assert.Null(vista.LVistaChosen);
        Assert.Null(engine.TEngineEntryLoad(entry.LEntryId));

        vista.TVistaSelect(entry.LEntryId);
        Assert.Null(engine.TVistaLoad(vista));
        vista.TVistaSelect(null);
        Assert.Null(engine.TVistaLoad(vista));
    }

    [Fact]
    public void Load_CatalogVistas_ReturnTheirOwnSubjectWithoutStartingAnEditingDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterfaceState.TStateValueCreate("water");
        LExample example = engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LReference reference = engine.TEngineCitationCreate("Source");
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Writer"));
        LTag tag = engine.TEngineTagCreate("Tag");
        LRegister register = engine.TEngineRegisterCreate("Register");

        Assert.Equal(example, TVistaLoadRead(engine, "corpus", example.LExampleId).LDraftExample);
        Assert.Equal(situation, TVistaLoadRead(engine, "repertoire", situation.LSituationId).LDraftSituation);
        Assert.Equal(reference, TVistaLoadRead(engine, "reference", reference.LReferenceId).LDraftReference);
        Assert.Equal(reference, TVistaLoadRead(engine, "oeuvre", reference.LReferenceId).LDraftReference);
        Assert.Equal(author, TVistaLoadRead(engine, "guild", author.LAuthorId).LDraftAuthorHeld);
        TVistaLoadRead(engine, "taxonomy", tag.LTagId);
        TVistaLoadRead(engine, "tenor", register.LRegisterId);
    }

    [Theory]
    [InlineData("corpus")]
    [InlineData("repertoire")]
    [InlineData("reference")]
    [InlineData("oeuvre")]
    [InlineData("guild")]
    [InlineData("taxonomy")]
    [InlineData("tenor")]
    public void Load_CatalogMissingOrCleared_ReturnsNull(string tab)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(long.MaxValue);
        Assert.Null(engine.TVistaLoad(vista));
        vista.TVistaSelect(null);
        Assert.Null(engine.TVistaLoad(vista));
    }

    [Fact]
    public void Delete_CatalogVistas_DeleteTheirOwnSubjectAndDetachFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterfaceState.TStateValueCreate("water");
        LExample example = engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        LSituation situation = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LReference reference = engine.TEngineCitationCreate("Source");
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Writer"));
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "",
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a meaning", [], [], [], [], [], 1)], []));
        engine.TRequestQuoteApply(entry.LEntryId, example.LExampleId);


        Assert.Null(TVistaDeleteRun(engine, "corpus", example.LExampleId));
        Assert.Null(engine.TEngineExampleRead(example.LExampleId));
        Assert.Null(TVistaDeleteRun(engine, "repertoire", situation.LSituationId));
        Assert.Null(engine.TEngineSituationRead(situation.LSituationId));
        Assert.Null(TVistaDeleteRun(engine, "reference", reference.LReferenceId));
        Assert.Null(engine.TEngineReferenceRead(reference.LReferenceId));
        Assert.Null(TVistaDeleteRun(engine, "guild", author.LAuthorId));
        Assert.Null(engine.TEngineAuthorRead(author.LAuthorId));
        Assert.NotNull(engine.TEngineEntryLoad(entry.LEntryId));
    }

    [Fact]
    public void Delete_TagAndRegisterVistas_DeleteNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("Tag");
        LRegister register = engine.TEngineRegisterCreate("Register");

        LVista taxonomy = engine.TEngineVistaStart("taxonomy", LCatalogOrder.LCatalogOrderName);
        taxonomy.TVistaSelect(tag.LTagId);
        Assert.Null(engine.TVistaDelete(taxonomy));
        Assert.Equal(tag.LTagId, taxonomy.LVistaChosen);
        Assert.Contains(engine.TEngineTagRead(), row => row.LTagId == tag.LTagId);

        LVista tenor = engine.TEngineVistaStart("tenor", LCatalogOrder.LCatalogOrderName);
        tenor.TVistaSelect(register.LRegisterId);
        Assert.Null(engine.TVistaDelete(tenor));
        Assert.Equal(register.LRegisterId, tenor.LVistaChosen);
        Assert.Contains(
            engine.TEngineRegisterFind(tenor), row => row.LCatalogRegisterStored.LRegisterId == register.LRegisterId);
    }

    private static LRevision? TVistaDeleteRun(LEngine engine, string tab, long id)
    {
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(id);
        LRevision? revision = engine.TVistaDelete(vista);
        Assert.Null(vista.LVistaChosen);
        return revision;
    }

    private static LDraft TVistaLoadRead(LEngine engine, string tab, long id)
    {
        LVista vista = engine.TEngineVistaStart(tab, LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(id);
        LDraft draft = Assert.IsType<LDraft>(engine.TVistaLoad(vista));
        Assert.Equal(0, draft.LDraftId);
        return draft;
    }
}
