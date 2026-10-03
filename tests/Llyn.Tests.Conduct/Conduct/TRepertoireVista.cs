using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRepertoireVista
{
    [Fact]
    public async Task RepertoireRowsLoad_StoredSituation_AnswersTheRowsAndTheLanguagesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TRepertoire.TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));

        CEnsignSheet<IReadOnlyList<CCatalogSituation>> sheet =
            await repertoire.CRepertoireRowsLoad(static (_, _) => static () => { });

        Assert.Equal(["at home"], sheet.CEnsignSheetRows.Select(static row => row.CCatalogSituationTitle));
        Assert.Equal(atelier.CAtelierCatalog.CCatalogLanguageRead(), sheet.CEnsignSheetLanguages);
    }

    [Fact]
    public void RepertoireVistaRestore_QueriesHeld_CarriesThemIntoTheFreshVistas()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TRepertoire.TRepertoireSituationSave(engine, "at home");
        TRepertoire.TRepertoireSituationSave(engine, "in court");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireAtlas.CAtlasQuerySet("court");
        repertoire.CRepertoireOccurrence.COccurrenceQuerySet("zzz");

        repertoire.TRepertoireVistaRestore();

        Assert.Equal(["in court"], repertoire.CRepertoireRowsRead().Select(row => row.CCatalogSituationTitle));
        Assert.Equal("Situation.Unmatched", repertoire.CRepertoireOccurrence.COccurrenceEmptyKey);
    }

    [Fact]
    public void RepertoireVistaRestore_SituationNoticeAfterASecondRestore_RaisesEachListOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        int marshalled = 0;
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        repertoire.TRepertoireVistaRestore();
        int atlas = 0;
        int occurrence = 0;
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => atlas++;
        repertoire.CRepertoireOccurrence.COccurrencePanel.CPanelRowsChanged += () => occurrence++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSituation, home.LSituationId);

        Assert.Equal(1, atlas);
        Assert.Equal(1, occurrence);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void RepertoireCreate_SituationNoticeWithoutARestore_RaisesEachListOnceThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        int marshalled = 0;
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int atlas = 0;
        int occurrence = 0;
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => atlas++;
        repertoire.CRepertoireOccurrence.COccurrencePanel.CPanelRowsChanged += () => occurrence++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSituation, home.LSituationId);

        Assert.Equal(1, atlas);
        Assert.Equal(1, occurrence);
        Assert.Equal(1, marshalled);
    }

    [Fact]
    public void RepertoireVistaRestore_EntryNotice_RaisesTheSituationRowsForTheirTallies()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        int atlas = 0;
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => atlas++;

        engine.TEngineBulletinRaise(LSubject.LSubjectEntry, hearth.LEntryId);

        Assert.Equal(1, atlas);
    }

    [Fact]
    public void RepertoireVistaRestore_WorkspaceNotice_ClosesTheChosenSituationAndTellsTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        Assert.True(repertoire.CRepertoireVignetteHeld);
        int told = 0;
        repertoire.CRepertoireWorkspaceChanged += () => told++;

        engine.TEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);

        Assert.True(repertoire.CRepertoireVignetteBlank);
        Assert.Null(repertoire.CRepertoireAtlas.CAtlasChosen);
        Assert.Equal(1, told);
    }

    [Fact]
    public void RepertoireVistaRestore_SettingsNotice_RaisesTheSituationRowsThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        int marshalled = 0;
        CRepertoire repertoire = CRepertoire.CRepertoireCreate(
            atelier,
            static () => true,
            TEnvoyFake.TEnvoyCreate(false, []),
            run =>
            {
                marshalled++;
                run();
            });
        int atlas = 0;
        repertoire.CRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += () => atlas++;

        engine.TEngineBulletinRaise(LSubject.LSubjectSettings, 0);

        Assert.Equal(1, atlas);
        Assert.Equal(1, marshalled);
    }
}
