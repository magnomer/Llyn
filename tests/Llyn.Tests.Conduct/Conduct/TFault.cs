using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed partial class TFault
{
    private const string TFaultFlagged =
        "{ \"varieties\": { \"shown\": \"flag\", \"list\": [ { \"name\": \"British\", \"flag\": \"gb\" } ] } }";

    private static readonly IReadOnlyList<TFaultRow> TFaultRows =
    [
        new(
            "CCatalog.CCatalogEnsignLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Language.LoadFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CAtelier atelier = stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
                    engine, stage.TFaultStageMember, stage.TFaultStageThrown));
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return Task.FromResult<Func<Task>>(
                    () => atelier.CAtelierCatalog.CCatalogEnsignLoad(envoy, static (_, _) => static () => { }));
            }),
        new(
            "CDisplayAccent.CDisplayAccentLoad",
            "LLanguagePort.LEngineAccentLoad",
            "Sound.LoadFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CAtelier atelier = stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
                    engine, stage.TFaultStageMember, stage.TFaultStageThrown));
                LEntry water = engine.TEngineEntrySave(
                    TInterface.TEntryDraftCreate(
                        "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], [])
                    with
                    {
                        LEntryDraftPronunciations = [TInterface.TPronunciationDraftCreate("ˈwɔːtə", "British")],
                    });
                CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard), true);
                wing.CWingEntryOpen(water.LEntryId);
                return Task.FromResult<Func<Task>>(
                    () => wing.CWingDisplay.CDisplayAccent.CDisplayAccentLoad(static (_, _) => static () => { }));
            }),
        new(
            "CErrand.CErrandEnsignLoad",
            "LLanguageVault.LLanguageFlagRead",
            "Input.RecordingFailed",
            static async stage =>
            {
                CErrand errand = await TFaultRecordingStart(stage, TFaultFlagged);
                return () => errand.CErrandEnsignLoad(static (_, _) => static () => { });
            }),
        new(
            "CErrand.CErrandFlagLoad",
            "LLanguageVault.LLanguageFlagRead",
            "Input.TranscriptionFailed",
            static stage =>
            {
                CErrand errand = TFaultEditorStart(stage, TFaultFlagged).CEditorDesk.CDeskErrand;
                errand.CErrandTranscriptionStart(0, string.Empty);
                return Task.FromResult<Func<Task>>(() => errand.CErrandFlagLoad(static (_, _) => static () => { }));
            }),
        new(
            "CErrand.CErrandPreviewStart",
            "LRecordingVault.LRecordingPrepare",
            "Input.RecordingFailed",
            static async stage =>
            {
                CErrand errand = await TFaultRecordingStart(stage, "{}");
                CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
                errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
                return () => errand.CErrandPreviewStart(found);
            }),
        new(
            "CErrand.CErrandRecordingSave",
            "LRecordingVault.LRecordingSave",
            "Input.RecordingFailed",
            static async stage =>
            {
                CErrand errand = await TFaultRecordingStart(stage, "{}");
                CRecording found = new("Tagged", "https://example.test/gb.mp3", 0, true, "British");
                errand.TErrandHarvestResonate(new CHarvestStep("Tagged", 0, found, false));
                return () => errand.CErrandRecordingSave(found);
            }),
        new(
            "CTimbre.CTimbreFlagRead",
            "LLanguageVault.LLanguageFlagRead",
            "Sound.LoadFailed",
            static stage =>
            {
                CEditor editor = TFaultEditorStart(stage, TFaultFlagged);
                editor.CEditorDesk.TDeskDefer(TInterface.TRequestIpaCreate(editor.CEditorDesk.CDeskId, "a˥"));
                editor.CEditorDesk.TDeskVarietySet(true, 0, "British");
                return Task.FromResult<Func<Task>>(
                    () => editor.CEditorTimbre.CTimbreFlagRead(static (_, _) => static () => { }));
            }),
        new(
            "CLibrary.CLibraryMarkupImport",
            "LPortraitPort.LEngineMarkupStart",
            "List.ImportFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CAtelier atelier = stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
                    engine, stage.TFaultStageMember, stage.TFaultStageThrown));
                CLibrary library = TLibrary.TLibraryPrepare(
                    atelier, TEnvoyFake.TEnvoyMarkupCreate("fault.llx", stage.TFaultStageHeard));
                return Task.FromResult<Func<Task>>(library.CLibraryMarkupImport);
            }),
        new(
            "CCourier.CCourierSend",
            "LPortraitPort.LEngineCourierSend",
            "Courier.SendFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CAtelier atelier = stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
                    engine, stage.TFaultStageMember, stage.TFaultStageThrown));
                CCourier courier = CCourier.CCourierCreate(atelier);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return Task.FromResult<Func<Task>>(() => courier.CCourierSend(envoy));
            }),
        new(
            "CCourier.CCourierAttach",
            "LPortraitPort.LEngineCourierAttach",
            "Courier.AttachFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CAtelier atelier = stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
                    engine, stage.TFaultStageMember, stage.TFaultStageThrown));
                CCourier courier = CCourier.CCourierCreate(atelier);
                CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard);
                return Task.FromResult<Func<Task>>(() => courier.CCourierAttach(envoy));
            }),
        new(
            "CCorpus.CCorpusRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Example.LoadFailed",
            static stage =>
            {
                CCorpus corpus = CCorpus.CCorpusCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => corpus.CCorpusRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CFavorite.CFavoriteRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Favorite.LoadFailed",
            static stage =>
            {
                CFavorite favorite = CFavorite.CFavoriteCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => favorite.CFavoriteRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CFootnote.CFootnoteRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "List.LoadFailed",
            static stage =>
            {
                CShelf shelf = CShelf.CShelfCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => shelf.CShelfFootnote.CFootnoteRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CLibrary.CLibraryRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "List.LoadFailed",
            static stage =>
            {
                CLibrary library = CLibrary.CLibraryCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => library.CLibraryRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "COccurrence.COccurrenceRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Situation.LoadFailed",
            static stage =>
            {
                CRepertoire repertoire = CRepertoire.CRepertoireCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => repertoire.CRepertoireOccurrence.COccurrenceRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CPhonology.CPhonologyRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Sound.LoadFailed",
            static stage =>
            {
                CPhonology phonology = CPhonology.CPhonologyCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => phonology.CPhonologyRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CQuotation.CQuotationRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Example.LoadFailed",
            static stage =>
            {
                CCorpus corpus = CCorpus.CCorpusCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => corpus.CCorpusQuotation.CQuotationRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CRepertoire.CRepertoireRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Situation.LoadFailed",
            static stage =>
            {
                CRepertoire repertoire = CRepertoire.CRepertoireCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => repertoire.CRepertoireRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CShelf.CShelfRollLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Source.LoadFailed",
            static stage =>
            {
                CShelf shelf = CShelf.CShelfCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => shelf.CShelfRollLoad(static (_, _) => static () => { }));
            }),
        new(
            "CTaxonomy.CTaxonomyRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Tag.LoadFailed",
            static stage =>
            {
                CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => taxonomy.CTaxonomyRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CTenor.CTenorRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Register.LoadFailed",
            static stage =>
            {
                CTenor tenor = CTenor.CTenorCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => tenor.CTenorRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CEntryList.CEntryListLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Xiesheng.LoadFailed",
            static stage =>
            {
                CXiesheng xiesheng = CXiesheng.CXieshengCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => xiesheng.CXieshengKindred.CEntryListLoad(static (_, _) => static () => { }));
            }),
        new(
            "CYunjing.CYunjingXiaoyun.CEntryListLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Yunjing.LoadFailed",
            static stage =>
            {
                CYunjing yunjing = CYunjing.CYunjingCreate(
                    TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => yunjing.CYunjingXiaoyun.CEntryListLoad(static (_, _) => static () => { }));
            }),
        .. TFaultPortraitRows,
    ];

    public static TheoryData<string> TFaultGates
    {
        get
        {
            TheoryData<string> gates = new();
            foreach (TFaultRow row in TFaultRows)
            {
                gates.Add(row.TFaultRowGate);
            }

            return gates;
        }
    }

    [Theory]
    [MemberData(nameof(TFaultGates))]
    public Task EnvoyFailureShow_FaultedTask_ReceivesTheKeyOnce(string gate) => TFaultRun(gate, false);

    [Theory]
    [MemberData(nameof(TFaultGates))]
    public Task EnvoyFailureShow_ThrownFault_ReceivesTheKeyOnce(string gate) => TFaultRun(gate, true);

    [Fact]
    public void EnvoyFailureShow_EveryTaskGate_HoldsASweepRow()
    {
        HashSet<string> held = TFaultRows.Select(static row => row.TFaultRowGate).ToHashSet(StringComparer.Ordinal);

        List<string> missing = typeof(CCatalog).Assembly.GetExportedTypes()
            .SelectMany(static type => type
                .GetMethods(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(static method => !method.IsSpecialName
                    && (method.ReturnType == typeof(Task)
                        || (method.ReturnType.IsGenericType
                            && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))))
                .Select(method => type.Name + "." + method.Name))
            .Distinct(StringComparer.Ordinal)
            .Where(gate => !held.Contains(gate))
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.Empty(missing);
    }

    private static async Task TFaultRun(string gate, bool thrown)
    {
        TFaultRow row = TFaultRows.Single(found => string.Equals(found.TFaultRowGate, gate, StringComparison.Ordinal));
        using TFaultStage stage = new(row.TFaultRowMember, thrown);
        Func<Task> call = await row.TFaultRowArrange(stage);
        stage.TFaultStageHeard.Clear();

        Exception? escaped = await Record.ExceptionAsync(() => call().WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Null(escaped);
        Assert.Equal([row.TFaultRowKey], stage.TFaultStageHeard);
    }

    private static LEngine TFaultEngineStart(TFaultStage stage)
    {
        TWorkspace workspace = stage.TFaultStageAdd(TWorkspace.TWorkspacePrepare());
        return stage.TFaultStageAdd(workspace.TWorkspaceEngineStart());
    }

    private static CAtelier TFaultAtelierStart(TFaultStage stage) =>
        stage.TFaultStageAdd(TInterfaceConduct.TAtelierFaultCreate(
            TFaultEngineStart(stage), stage.TFaultStageMember, stage.TFaultStageThrown));

    private static CEditor TFaultEditorStart(TFaultStage stage, string json)
    {
        TLanguageFixture pack = stage.TFaultStageAdd(TLanguageFixture.TLanguageFixtureCreate(json));
        TWorkspace workspace = stage.TFaultStageAdd(TWorkspace.TWorkspaceCreate());
        LRig rig = workspace.TWorkspaceRigCreate();
        LRig faulted = rig with
        {
            LRigLanguages = TEngineFault.TEngineFaultCreate(
                rig.LRigLanguages, stage.TFaultStageMember, stage.TFaultStageThrown),
            LRigRecordings = TEngineFault.TEngineFaultCreate(
                rig.LRigRecordings, stage.TFaultStageMember, stage.TFaultStageThrown),
        };
        LEngine engine = stage.TFaultStageAdd(TInterface.TEngineCreate(faulted));
        engine.TEngineDelaySet(0);
        CEditor editor = TInterfaceEditor.TEditorCreate(
            engine, TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(null);
        editor.CEditorEntry.CEntryLanguageSet(pack.TLanguageFixtureName);
        editor.CEditorEntry.CEntryHeadwordSet("hill");
        return editor;
    }

    private static async Task<CErrand> TFaultRecordingStart(TFaultStage stage, string json)
    {
        CErrand errand = TFaultEditorStart(stage, json).CEditorDesk.CDeskErrand;
        TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        errand.CErrandClipChanged += roll =>
        {
            if (!roll.CClipRollSearching)
            {
                finished.TrySetResult();
            }
        };
        errand.CErrandRecordingStart(0);
        await finished.Task.WaitAsync(TimeSpan.FromSeconds(5));
        return errand;
    }

    private sealed record TFaultRow(
        string TFaultRowGate,
        string TFaultRowMember,
        string TFaultRowKey,
        Func<TFaultStage, Task<Func<Task>>> TFaultRowArrange);

    private sealed class TFaultStage : IDisposable
    {
        private readonly Stack<IDisposable> _tFaultStageHeld = new();

        internal TFaultStage(string member, bool thrown)
        {
            TFaultStageMember = member;
            TFaultStageThrown = thrown;
        }

        internal string TFaultStageMember { get; }

        internal bool TFaultStageThrown { get; }

        internal List<string> TFaultStageHeard { get; } = [];

        internal TFaultKind TFaultStageAdd<TFaultKind>(TFaultKind held) where TFaultKind : IDisposable
        {
            _tFaultStageHeld.Push(held);
            return held;
        }

        public void Dispose()
        {
            while (_tFaultStageHeld.TryPop(out IDisposable? held))
            {
                held.Dispose();
            }
        }
    }
}
