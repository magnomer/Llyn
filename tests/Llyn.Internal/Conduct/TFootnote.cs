using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TFootnote
{
    [Fact]
    public void FootnoteRowsRead_NoSourceChosen_ListsEveryEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TFootnoteEntrySave(engine, "water");
        (CFootnote footnote, _, _) = TFootnotePrepare(engine, atelier);

        Assert.Equal(water.LEntryId, Assert.Single(footnote.CFootnoteRowsRead()).CVistaRowId);
    }

    [Fact]
    public void FootnoteRowsRead_UncitedSourceChosen_ListsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFootnoteEntrySave(engine, "water");
        LReference book = engine.TEngineCitationCreate("Book");
        (CFootnote footnote, LVista parent, _) = TFootnotePrepare(engine, atelier);

        parent.TVistaSelect(book.LReferenceId);

        Assert.Empty(footnote.CFootnoteRowsRead());
    }

    [Fact]
    public void FootnoteQuerySet_UnmatchedText_EmptiesTheRowsAndWordsTheEmptyList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TFootnoteEntrySave(engine, "water");
        (CFootnote footnote, _, _) = TFootnotePrepare(engine, atelier);

        Assert.Equal("Source.Vacant", footnote.CFootnoteEmptyKey);

        footnote.CFootnoteQuerySet("zzz");

        Assert.Empty(footnote.CFootnoteRowsRead());
        Assert.Equal("Source.Unmatched", footnote.CFootnoteEmptyKey);

        footnote.CFootnoteQuerySet("  ");

        Assert.Equal("Source.Vacant", footnote.CFootnoteEmptyKey);
    }

    [Fact]
    public void FootnoteEntryCreate_SourceChosen_OpensAFreshEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference book = engine.TEngineCitationCreate("Book");
        (CFootnote footnote, LVista parent, CEditor editor) = TFootnotePrepare(engine, atelier);
        parent.TVistaSelect(book.LReferenceId);

        footnote.TFootnoteEntryCreate();

        Assert.True(footnote.CFootnotePanel.CPanelEditing);
        Assert.False(footnote.CFootnotePanel.CPanelBinEnabled);
        CExampleDraft? cited = editor
            .CEditorDraftRead()?.CEntryDraftMeanings[0].CCardDraftSentence[0].CSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.CExampleDraftReference);
    }

    [Fact]
    public void FootnoteEntryCreate_NoSourceChosen_OpensAFreshEntryCitingNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        (CFootnote footnote, _, CEditor editor) = TFootnotePrepare(engine, atelier);

        footnote.TFootnoteEntryCreate();

        Assert.True(footnote.CFootnotePanel.CPanelEditing);
        Assert.All(
            editor.CEditorDraftRead()!.CEntryDraftMeanings.SelectMany(meaning => meaning.CCardDraftSentence),
            sentence => Assert.Null(sentence.CSentenceDraftExample?.CExampleDraftReference));
    }

    private static (CFootnote, LVista, CEditor) TFootnotePrepare(
        LEngine engine, CAtelier atelier)
    {
        CEnvoy envoy = TInterfaceConduct.TEnvoyCreate(false, []);
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CFootnote footnote = new(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            editor,
            envoy,
            static _ => true,
            static () => true);
        LVista parent = engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName);
        LVista vista = engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword);
        footnote.TFootnoteVistaRestore(parent, vista);
        editor.TEditorVistaRestore(vista);
        return (footnote, parent, editor);
    }

    private static LEntry TFootnoteEntrySave(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
