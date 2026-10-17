using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed partial class TFault
{
    private static IReadOnlyList<TFaultRow> TFaultPortraitRows =>
    [
        new(
            "CCohort.CCohortPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultTenorStart(stage).CTenorCohort.CCohortPortraitExport)),
        new(
            "CCohort.CCohortPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultTenorStart(stage).CTenorCohort.CCohortPortraitPrint)),
        new(
            "CCorpus.CCorpusPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultCorpusStart(stage).CCorpusPortraitPrint)),
        new(
            "CFavorite.CFavoritePortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(CFavorite.CFavoriteCreate(
                TFaultAtelierStart(stage), static () => true, TFaultEnvoyCreate(stage), static run => run())
                .CFavoritePortraitExport)),
        new(
            "CFavorite.CFavoritePortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(CFavorite.CFavoriteCreate(
                TFaultAtelierStart(stage), static () => true, TFaultEnvoyCreate(stage), static run => run())
                .CFavoritePortraitPrint)),
        new(
            "CGuild.CGuildPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CGuild guild = TGuild.TGuildPrepare(TFaultAtelierCreate(stage, engine), TFaultEnvoyCreate(stage));
                LReference book = engine.TEngineCitationCreate("Book");
                LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
                engine.TRequestCreditApply(book.LReferenceId, ada.LAuthorId, 0);
                guild.CGuildAuthorSelect(ada.LAuthorId);
                guild.CGuildSourceSelect(book.LReferenceId);
                return Task.FromResult<Func<Task>>(guild.CGuildPortraitPrint);
            }),
        new(
            "CLibrary.CLibraryEntryExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                long id = engine.TEngineTranslationCreate("water", "English").LEntryId;
                CLibrary library = TLibrary.TLibraryPrepare(
                    TFaultAtelierCreate(stage, engine),
                    TFaultEnvoyCreate(stage));
                return Task.FromResult<Func<Task>>(() => library.CLibraryEntryExport(id));
            }),
        new(
            "CLibrary.CLibraryPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TLibrary.TLibraryPrepare(TFaultAtelierStart(stage), TFaultEnvoyCreate(stage)).CLibraryPortraitExport)),
        new(
            "CLibrary.CLibraryPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TLibrary.TLibraryPrepare(TFaultAtelierStart(stage), TFaultEnvoyCreate(stage)).CLibraryPortraitPrint)),
        new(
            "CMembership.CMembershipPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TFaultTaxonomyStart(stage).CTaxonomyMembership.CMembershipPortraitExport)),
        new(
            "CMembership.CMembershipPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TFaultTaxonomyStart(stage).CTaxonomyMembership.CMembershipPortraitPrint)),
        new(
            "CPhonology.CPhonologyPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(CPhonology.CPhonologyCreate(
                TFaultAtelierStart(stage), static () => true, TFaultEnvoyCreate(stage), static run => run())
                .CPhonologyPortraitExport)),
        new(
            "CPhonology.CPhonologyPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(CPhonology.CPhonologyCreate(
                TFaultAtelierStart(stage), static () => true, TFaultEnvoyCreate(stage), static run => run())
                .CPhonologyPortraitPrint)),
        new(
            "CQuotation.CQuotationPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TFaultCorpusStart(stage).CCorpusQuotation.CQuotationPortraitExport)),
        new(
            "CRepertoire.CRepertoirePortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultRepertoireStart(stage).CRepertoirePortraitExport)),
        new(
            "CRepertoire.CRepertoirePortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultRepertoireStart(stage).CRepertoirePortraitPrint)),
        new(
            "CShelf.CShelfPortraitExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultShelfStart(stage).CShelfPortraitExport)),
        new(
            "CShelf.CShelfPortraitPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(TFaultShelfStart(stage).CShelfPortraitPrint)),
        new(
            "CEntryList.CEntryListExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(CXiesheng.CXieshengCreate(
                TFaultAtelierStart(stage), static () => true, TFaultEnvoyCreate(stage), static run => run())
                .CXieshengKindred.CEntryListExport)),
        new(
            "CEntryList.CEntryListPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static async stage =>
            {
                string language = stage.TFaultStageAdd(
                    TLanguageFixture.TLanguageFixtureCreate(TXiesheng.TXieshengPack)).TLanguageFixtureName;
                TWorkspace workspace = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare());
                LEngine engine = stage.TFaultStageAdd(TXiesheng.TXieshengEngineStart(workspace));
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                LEntry entry = await TXiesheng.TXieshengStemSave(engine, language);
                CXiesheng xiesheng = CXiesheng.CXieshengCreate(
                    atelier, static () => true, TFaultEnvoyCreate(stage), static run => run());
                xiesheng.TXieshengStemOpen(language, "龍");
                xiesheng.CXieshengKindred.CEntryListPanel.CPanelRowSelect(entry.LEntryId);
                return xiesheng.CXieshengKindred.CEntryListPrint;
            }),
        new(
            "CYunjing.CYunjingXiaoyun.CEntryListExport",
            "LPortraitPort.LEnginePortraitExport",
            "Export.Failed",
            static stage => Task.FromResult<Func<Task>>(
                TYunjing.TYunjingPrepare(TFaultAtelierStart(stage), TFaultEnvoyCreate(stage))
                    .CYunjingXiaoyun.CEntryListExport)),
        new(
            "CYunjing.CYunjingXiaoyun.CEntryListPrint",
            "LPortraitPort.LEnginePressOpen",
            "Print.Failed",
            static stage =>
            {
                string language = stage.TFaultStageAdd(
                    TLanguageFixture.TLanguageFixtureCreate(TYunjing.TYunjingPack)).TLanguageFixtureName;
                TWorkspace workspace = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare());
                LEngine engine = stage.TFaultStageAdd(workspace.TWorkspaceEngineStart());
                CAtelier atelier = TFaultAtelierCreate(stage, engine);
                TXiaoyunFixture.TXiaoyunDiweiPlace(engine, workspace, language, "爛", "來");
                CYunjing yunjing = TYunjing.TYunjingPrepare(atelier, TFaultEnvoyCreate(stage));
                yunjing.TYunjingDiweiOpen(language, LDiwei.LDiweiInitial, "來");
                CEntryList xiaoyun = yunjing.CYunjingXiaoyun;
                xiaoyun.CEntryListPanel.CPanelRowSelect(Assert.Single(xiaoyun.CEntryListRead()).CVistaRowId);
                return Task.FromResult<Func<Task>>(xiaoyun.CEntryListPrint);
            }),
        new(
            "CSPress.CSPressRestore",
            "LPressQueue.LPressQueueRead",
            "Print.Failed",
            static stage => Task.FromResult<Func<Task>>(() => TInterfaceConductPortrait.TPressWindowOpen(
                new TPressBoard(),
                static _ => Task.CompletedTask,
                asked: stage.TFaultStageHeard,
                failure: new InvalidOperationException("The fake queue faults LPressQueue.LPressQueueRead.")))),
        new(
            "CSPress.CSPressSystemShow",
            "LPressBoard.LPressBoardShow",
            "Print.Failed",
            static stage => TFaultPressOpen(stage, static press => press.CSPressSystemShow())),
        new(
            "CSPress.CSPressPrint",
            "LPressBoard.LPressBoardPrint",
            "Print.Failed",
            static stage => TFaultPressOpen(stage, static press => press.CSPressPrint())),
        new(
            "CSGalley.CSGalleyDraw",
            "LPressBoard.LPressBoardDraw",
            "Print.RenderFailed",
            static stage => TFaultPressOpen(stage, static press => press.CSPressGalley.CSGalleyDraw())),
        new(
            "CSGalley.CSGalleySightSet",
            "LPressProof.LPressProofDraw",
            "Print.RenderFailed",
            static stage => TFaultPressOpen(stage, static async press =>
            {
                await press.CSPressGalley.CSGalleyDraw();
                press.TPressDelaySet(0);
                await press.CSPressGalley.CSGalleySightSet(0, 0, 0, 1.0);
            })),
    ];

    private static Task<Func<Task>> TFaultPressOpen(TFaultStage stage, Func<CSPress, Task> gate)
    {
        TPressBoard board = new()
        {
            TPressBoardCover = proof => TEngineFault.TEngineFaultCreate(
                proof, stage.TFaultStageMember, stage.TFaultStageThrown),
        };
        LPressBoard faulted = TEngineFault.TEngineFaultCreate<LPressBoard>(
            board, stage.TFaultStageMember, stage.TFaultStageThrown);
        return Task.FromResult<Func<Task>>(
            () => TInterfaceConductPortrait.TPressWindowOpen(faulted, gate, asked: stage.TFaultStageHeard));
    }

    private static CAtelier TFaultAtelierCreate(TFaultStage stage, LEngine engine) =>
        stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
            engine, stage.TFaultStageMember, stage.TFaultStageThrown));

    private static CEnvoy TFaultEnvoyCreate(TFaultStage stage) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyFileRead"] = static _ => ((string?)"fault.md", CPortraitMedium.CPortraitMediumMarkdown),
            ["CEnvoyFailureShow"] = args =>
            {
                stage.TFaultStageHeard.Add((string)args![0]!);
                return null;
            },
        });

    private static CTenor TFaultTenorStart(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CTenor tenor = TTenor.TTenorPrepare(TFaultAtelierCreate(stage, engine), TFaultEnvoyCreate(stage));
        LEntry hearth = TShelf.TShelfEntrySave(engine, "hearth");
        tenor.CTenorCohort.CCohortPanel.CPanelRowOpen(hearth.LEntryId);
        return tenor;
    }

    private static CTaxonomy TFaultTaxonomyStart(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(TFaultAtelierCreate(stage, engine), TFaultEnvoyCreate(stage));
        LEntry hearth = TTaxonomy.TTaxonomyEntrySave(engine);
        taxonomy.CTaxonomyMembership.CMembershipPanel.CPanelRowOpen(hearth.LEntryId);
        return taxonomy;
    }

    private static CCorpus TFaultCorpusStart(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CAtelier atelier = TFaultAtelierCreate(stage, engine);
        LExample cat = TCorpus.TCorpusExampleSave(engine, "a cat sat");
        LEntry water = TCorpus.TCorpusEntrySave(engine);
        engine.TRequestQuoteApply(water.LEntryId, cat.LExampleId);
        CCorpus corpus = TCorpus.TCorpusPrepare(atelier, TFaultEnvoyCreate(stage));
        corpus.TCorpusExampleOpen(cat.LExampleId);
        corpus.CCorpusDiptych.CDiptychChildSelect(water.LEntryId);
        return corpus;
    }

    private static CRepertoire TFaultRepertoireStart(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CAtelier atelier = TFaultAtelierCreate(stage, engine);
        LSituation home = TRepertoire.TRepertoireSituationSave(engine, "at home");
        LEntry hearth = TRepertoire.TRepertoireEntrySave(engine, "hearth", home);
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TFaultEnvoyCreate(stage));
        repertoire.TRepertoireSituationOpen(home.LSituationId);
        repertoire.CRepertoireDiptych.CDiptychChildSelect(hearth.LEntryId);
        return repertoire;
    }

    private static CShelf TFaultShelfStart(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CShelf shelf = TShelf.TShelfPrepare(TFaultAtelierCreate(stage, engine), TFaultEnvoyCreate(stage));
        LEntry hearth = TShelf.TShelfEntrySave(engine, "hearth");
        shelf.CShelfEntrySelect(hearth.LEntryId);
        return shelf;
    }
}
