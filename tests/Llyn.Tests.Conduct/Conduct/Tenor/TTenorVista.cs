using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenorVista
{
    [Fact]
    public async Task TenorRowsLoad_StoredRegister_AnswersTheRowsAndTheLanguagesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineRegisterCreate("motion");
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CEnsignSheet<IReadOnlyList<CCatalogRegister>> sheet =
            await tenor.CTenorRowsLoad(static (_, _) => static () => { });

        Assert.Equal(["motion"], sheet.CEnsignSheetRows.Select(static row => row.CCatalogRegisterStored.CRegisterName));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void TenorVistaRestore_QueriesHeld_CarriesBothIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineRegisterCreate("motion");
        engine.TEngineRegisterCreate("botany");
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        tenor.CTenorAperture.CApertureQuerySet("mot");
        tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureQuerySet("zzz");

        tenor.TTenorVistaRestore();

        Assert.Equal(["motion"], tenor.CTenorRowsRead().Select(row => row.CCatalogRegisterStored.CRegisterName));
        Assert.Equal("Register.Unmatched", tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureKey);
    }

    [Fact]
    public void TenorVistaRestore_SettingsNoticeAfterASecondRestore_RaisesTheRegisterRowsThenTheEntryRowsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CTenor tenor = CTenor.CTenorCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        tenor.TTenorVistaRestore();
        List<string> seen = TTenorWatchPrepare(tenor);

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(["register", "entry"], seen);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void TenorCreate_SettingsNoticeWithoutARestore_RaisesTheRegisterRowsThenTheEntryRowsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CTenor tenor = CTenor.CTenorCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        List<string> seen = TTenorWatchPrepare(tenor);

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(["register", "entry"], seen);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void TenorVistaRestore_RegisterNoticeWithAnEntryShown_RaisesTheRowsBeforeTheDraftReread()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        long register = engine.TEngineRegisterCreate("motion").LRegisterId;
        tenor.CTenorCohort.CCohortPanel.CPanelRowOpen(hearth.LEntryId);
        List<string> seen = TTenorWatchPrepare(tenor);

        engine.TEngineBulletinRaise(LSubject.LSubjectRegister, register);

        Assert.Equal(["register", "entry", "draft"], seen);
        Assert.True(tenor.CTenorCohort.CCohortPanel.CPanelBinEnabled);
    }

    [Fact]
    public void TenorVistaRestore_WorkspaceNotice_ClosesTheEntryLetsGoOfTheRegisterAndTellsTheDriverLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CTenor tenor = TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        LEntry hearth = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hearth", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
        long register = engine.TEngineRegisterCreate("motion").LRegisterId;
        tenor.TTenorRegisterOpen(register);
        tenor.CTenorCohort.CCohortPanel.CPanelRowOpen(hearth.LEntryId);
        List<string> seen = [];
        tenor.CTenorAperture.CApertureRowsChanged += () =>
        {
            seen.Add("register");
            tenor.CTenorRowsRead();
        };
        tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureRowsChanged += () => seen.Add("entry");
        tenor.CTenorWorkspaceChanged += () => seen.Add("workspace");

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.False(tenor.CTenorCohort.CCohortPanel.CPanelBinEnabled);
        Assert.Null(tenor.CTenorAperture.CApertureChosen);
        Assert.Equal(["entry", "register", "entry", "workspace"], seen);
    }

    [Fact]
    public void TenorRowsRead_EngineFails_ShowsOneLoadFailureAndLeavesTheEntriesUnread()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTenor tenor = TTenorFailPrepare(engine, asked);
        int entries = 0;
        tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureRowsChanged += () => entries++;

        Assert.Empty(tenor.CTenorRowsRead());
        Assert.Equal(["Register.LoadFailed"], asked);
        Assert.Equal(0, entries);
    }

    [Fact]
    public void TenorVistaRestore_SettingsNoticeWithBothReadsFailing_ShowsOneLoadFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTenor tenor = TTenorFailPrepare(engine, asked);
        tenor.CTenorAperture.CApertureRowsChanged += () => tenor.CTenorRowsRead();
        tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureRowsChanged +=
            () => tenor.CTenorCohort.CCohortRowsRead();

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(["Register.LoadFailed"], asked);
    }

    [Fact]
    public void CohortRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CTenor tenor = TTenorFailPrepare(engine, asked);

        Assert.Empty(tenor.CTenorCohort.CCohortRowsRead());
        Assert.Equal(["Register.LoadFailed"], asked);
    }

    private static List<string> TTenorWatchPrepare(CTenor tenor)
    {
        List<string> seen = [];
        tenor.CTenorAperture.CApertureRowsChanged += () =>
        {
            seen.Add("register");
            tenor.CTenorRowsRead();
        };
        tenor.CTenorCohort.CCohortPanel.CPanelAperture.CApertureRowsChanged += () => seen.Add("entry");
        tenor.CTenorCohort.CCohortPanel.CPanelChanged += () => seen.Add("draft");
        return seen;
    }

    private static CTenor TTenorFailPrepare(LEngine engine, List<string> asked)
    {
        CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineRegisterFind"] = _ => throw new InvalidOperationException("no registers"),
                ["LEngineEntryFind"] = _ => throw new InvalidOperationException("no entries"),
            });
        return TTenor.TTenorPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, asked));
    }
}
