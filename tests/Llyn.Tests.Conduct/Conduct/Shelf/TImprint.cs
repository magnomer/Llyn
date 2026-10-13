using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TImprint
{
    [Fact]
    public void ImprintTitleSet_RawText_HoldsSpecifiedTitle()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        imprint.TImprintOpen(null);

        imprint.CImprintTitleSet("Book");
        imprint.CImprintYearSet(" ");

        LReference held = imprint.CImprintDesk.TDeskRead()!.LDraftReference!;
        Assert.Equal("Book", held.LReferenceTitle.TStateValueShow());
        Assert.True(held.LReferenceYear.LStateValueEmpty);
        Assert.Equal("Source.Year", held.LReferenceYearHint);
        Assert.True(imprint.CImprintDesk.TDeskChangeCheck());
    }

    [Fact]
    public void ImprintKindSet_Tag_RoundTripsThroughDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        imprint.TImprintOpen(null);

        imprint.CImprintKindSet("journal");
        imprint.CImprintKindSet(null);

        LReference held = imprint.CImprintDesk.TDeskRead()!.LDraftReference!;
        Assert.Equal("journal", held.LReferenceKindTag);
        Assert.True(imprint.CImprintDesk.CDeskChronicle.CDeskChronicleRead().CDeskBackward);
    }

    [Fact]
    public void ImprintKindSet_KindAlreadyHeld_SendsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        imprint.TImprintOpen(null);

        imprint.CImprintKindSet("unspecified");

        Assert.False(imprint.CImprintDesk.TDeskChangeCheck());
        Assert.False(imprint.CImprintDesk.CDeskChronicle.CDeskChronicleRead().CDeskBackward);
    }

    [Fact]
    public void ImprintSave_UnchangedStoredSource_KeepsDraftHeld()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        LReference book = engine.TEngineCitationCreate("Book");
        imprint.TImprintOpen(book.LReferenceId);
        CSession session =
            TInterfaceConductDesk.TSessionCreate(imprint.CImprintDesk, [], static () => true, static _ => { });

        session.CSessionSave();

        Assert.True(imprint.CImprintHeld);
        Assert.Equal(TInterface.TLocalizationTextRead("Source.UsageNone"), imprint.CImprintTallyRead());

        imprint.CImprintTitleSet("Tome");
        session.CSessionSave();

        Assert.False(imprint.CImprintHeld);
        Assert.Equal("Tome", engine.TEngineReferenceRead(book.LReferenceId)?.LReferenceTitle.TStateValueShow());
    }

    [Fact]
    public void ImprintOpen_StoredSource_ShowsItsFieldsThroughTheReferenceNotice()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        LReference book = engine.TEngineCitationCreate("Book");
        List<CReference> shown = [];
        imprint.CImprintReferenceChanged += shown.Add;

        imprint.TImprintOpen(book.LReferenceId);

        CReference reference = Assert.Single(shown);
        Assert.Equal("Book", reference.CReferenceTitle);
        Assert.Equal("Source.Untitled", reference.CReferenceTitleHint);
        Assert.Equal(string.Empty, reference.CReferenceYear);
        Assert.Equal("Source.Year", reference.CReferenceYearHint);
        Assert.Equal("unspecified", reference.CReferenceKindTag);
    }

    [Fact]
    public void ImprintKindRead_Menu_OffersEachKindWithItsTagAndKeyInMenuOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        IReadOnlyList<CReferenceKind> menu = TImprintPrepare(engine, atelier).CImprintKindRead();

        Assert.Equal(
            ["unspecified", "book", "journal", "article", "web", "video", "audio", "picture", "other", "unknown"],
            menu.Select(static kind => kind.CReferenceKindTag));
        Assert.Equal(
            [
                "Source.KindUnspecified", "Source.KindBook", "Source.KindJournal", "Source.KindArticle",
                "Source.KindWeb", "Source.KindVideo", "Source.KindAudio", "Source.KindPicture",
                "Source.KindOther", "Source.KindUnknown",
            ],
            menu.Select(static kind => kind.CReferenceKindKey));
    }

    [Fact]
    public void ImprintKindRead_NoRows_AnswersAnEmptyMenu()
    {
        Assert.Empty(TInterfaceConductPanel.TImprintKindRead([]));
    }

    [Fact]
    public void ImprintKindRead_RepeatedTags_KeepsTheFirstKindOfEach()
    {
        IReadOnlyList<CReferenceKind> menu = TInterfaceConductPanel.TImprintKindRead(
        [
            ("book", "Source.KindBook"),
            ("zither", "Source.KindZither"),
            ("book", "Source.KindOther"),
            ("Book", "Source.KindCase"),
            ("zither", "Source.KindZither"),
        ]);

        Assert.Equal(
            [
                new CReferenceKind("book", "Source.KindBook"),
                new CReferenceKind("zither", "Source.KindZither"),
                new CReferenceKind("Book", "Source.KindCase"),
            ],
            menu);
    }

    [Fact]
    public void ImprintEmptyRead_NoDraft_ReadsTheBlankSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);

        CReference blank = imprint.CImprintEmptyRead();

        Assert.Equal(string.Empty, blank.CReferenceTitle);
        Assert.Equal("Source.Untitled", blank.CReferenceTitleHint);
        Assert.Equal("Source.Url", blank.CReferenceUrlHint);
        Assert.Equal("Source.Note", blank.CReferenceNoteHint);
        Assert.Equal("Source.KindUnspecified", blank.CReferenceKindKey);
        Assert.Equal("unspecified", blank.CReferenceKindTag);
    }

    [Fact]
    public void ImprintCancel_HeldDraft_DropsTheDraftAndClosesTheByline()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CImprint imprint = TImprintPrepare(engine, atelier);
        engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
        imprint.TImprintOpen(null);
        imprint.CImprintByline.CBylineWordSet("Ad", true);
        imprint.CImprintByline.CBylineRowsRead();
        int changed = 0;
        imprint.CImprintByline.CBylineChanged += () => changed++;

        imprint.TImprintCancel();

        Assert.False(imprint.CImprintHeld);
        Assert.False(imprint.CImprintByline.CBylineShown);
        Assert.Empty(imprint.CImprintByline.CBylineRowsRead());
        Assert.Equal(1, changed);
    }

    internal static CImprint TImprintPrepare(LEngine engine, CAtelier atelier)
    {
        CImprint imprint = TInterfaceCitation.TImprintCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        imprint.CImprintDesk.TDeskVistaRestore(
            engine.TEngineVistaStart("reference", LCatalogOrder.LCatalogOrderName));
        return imprint;
    }
}
