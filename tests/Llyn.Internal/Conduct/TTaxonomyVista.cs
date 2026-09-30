using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTaxonomyVista
{
    [Fact]
    public void TaxonomyVistaRestore_QueriesHeld_CarriesBothIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineTagCreate("motion");
        engine.TEngineTagCreate("botany");
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        taxonomy.CTaxonomyQuerySet("mot");
        taxonomy.CTaxonomyMembership.CMembershipQuerySet("zzz");

        taxonomy.CTaxonomyVistaRestore();

        Assert.Equal(["motion"], taxonomy.CTaxonomyRowsRead().Select(row => row.CCatalogTagStored.CTagText));
        Assert.Equal("Tag.Unmatched", taxonomy.CTaxonomyMembership.CMembershipEmptyKey);
    }

    [Fact]
    public void TaxonomyVistaRestore_SettingsNoticeAfterASecondRestore_RaisesTheTagRowsThenTheEntryRowsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        taxonomy.CTaxonomyVistaRestore();
        taxonomy.CTaxonomyVistaRestore();
        List<string> seen = TTaxonomyWatchPrepare(taxonomy);

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(["tag", "entry"], seen);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void TaxonomyVistaRestore_TagNoticeWithAnEntryShown_RaisesTheRowsBeforeTheDraftReread()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        long tag = engine.TEngineTagCreate("motion").LTagId;
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowOpen(hearth.LEntryId);
        List<string> seen = TTaxonomyWatchPrepare(taxonomy);

        engine.TEngineBulletinRaise(LSubject.LSubjectTag, tag);

        Assert.Equal(["tag", "entry", "draft"], seen);
        Assert.True(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelBinEnabled);
    }

    [Fact]
    public void TaxonomyVistaRestore_WorkspaceNotice_ClosesTheEntryLetsGoOfTheTagAndTellsTheDriverLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        long tag = engine.TEngineTagCreate("motion").LTagId;
        taxonomy.TTaxonomyTagOpen(tag);
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowOpen(hearth.LEntryId);
        List<string> seen = [];
        taxonomy.CTaxonomyRowsChanged += () =>
        {
            seen.Add("tag");
            taxonomy.CTaxonomyRowsRead();
        };
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowsChanged += () => seen.Add("entry");
        taxonomy.CTaxonomyWorkspaceChanged += () => seen.Add("workspace");

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelBinEnabled);
        Assert.Null(taxonomy.LTaxonomyChosen);
        Assert.Equal(["entry", "tag", "entry", "workspace"], seen);
    }

    [Fact]
    public void TaxonomyRowsRead_EngineFails_ShowsOneLoadFailureAndLeavesTheEntriesUnread()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyFailPrepare(engine, asked);
        int entries = 0;
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowsChanged += () => entries++;

        Assert.Empty(taxonomy.CTaxonomyRowsRead());
        Assert.Equal(["Tag.LoadFailed"], asked);
        Assert.Equal(0, entries);
    }

    [Fact]
    public void TaxonomyVistaRestore_SettingsNoticeWithBothReadsFailing_ShowsOneLoadFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyFailPrepare(engine, asked);
        taxonomy.CTaxonomyRowsChanged += () => taxonomy.CTaxonomyRowsRead();
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowsChanged +=
            () => taxonomy.CTaxonomyMembership.CMembershipRowsRead();

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(["Tag.LoadFailed"], asked);
    }

    [Fact]
    public void MembershipRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTaxonomy taxonomy = TTaxonomyFailPrepare(engine, asked);

        Assert.Empty(taxonomy.CTaxonomyMembership.CMembershipRowsRead());
        Assert.Equal(["Tag.LoadFailed"], asked);
    }

    private static List<string> TTaxonomyWatchPrepare(CTaxonomy taxonomy)
    {
        List<string> seen = [];
        taxonomy.CTaxonomyRowsChanged += () =>
        {
            seen.Add("tag");
            taxonomy.CTaxonomyRowsRead();
        };
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowsChanged += () => seen.Add("entry");
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelChanged += () => seen.Add("draft");
        return seen;
    }

    private static CTaxonomy TTaxonomyFailPrepare(LEngine engine, List<string> asked)
    {
        CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineTagFind"] = _ => throw new InvalidOperationException("no tags"),
                ["LEngineEntryFind"] = _ => throw new InvalidOperationException("no entries"),
            });
        return TTaxonomy.TTaxonomyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
    }
}
