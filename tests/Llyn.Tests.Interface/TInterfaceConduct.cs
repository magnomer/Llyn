using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConduct
{
    internal static CAtelier TAtelierCreate(LEngine engine) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
        TEntryBundleCreate(engine),
        new LSettingsOutlet(engine),
        TPhonologyBundleCreate(engine),
        TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ => null,
            ["LEngineLocationRead"] = _ => null,
            ["LEngineScreenRead"] = _ => null,
        }),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLeftoverSweep"] = _ => null,
        }),
        TEntryBundleCreate(engine),
        new LSettingsOutlet(engine),
        TPhonologyBundleCreate(engine),
        media,
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierFaultCreate(LEngine engine, string member, bool thrown) => new(
        new LPosture(engine),
        TEngineFault.TEngineFaultCreate<LDraftPort>(new LDraftOutlet(engine), member, thrown),
        TEntryBundleCreate(engine, member, thrown),
        TEngineFault.TEngineFaultCreate<LSettingsPort>(new LSettingsOutlet(engine), member, thrown),
        TPhonologyBundleCreate(engine, member, thrown),
        TEngineFault.TEngineFaultCreate<LMediaPort>(new LMediaOutlet(engine), member, thrown),
        TEngineFault.TEngineFaultCreate<LPortraitPort>(new LPortraitOutlet(engine), member, thrown));

    internal static CEntryBundle TEntryBundleCreate(LEngine engine) => TEntryBundleCreate(engine, new object());

    internal static CEntryBundle TEntryBundleCreate(LEngine engine, object swap) => new(
        swap as LEntryPort ?? engine.LEngineEntry,
        swap as LGraspPort ?? engine.LEngineEntry,
        swap as LFavoritePort ?? engine.LEngineVista,
        swap as LVistaPort ?? engine.LEngineVista,
        swap as LCardPort ?? engine.LEngineCard,
        swap as LTagPort ?? engine.LEngineCard,
        swap as LRegisterPort ?? engine.LEngineCard,
        swap as LMentionPort ?? engine.LEngineMention,
        swap as LGlyphPort ?? engine.LEngineLanguage,
        swap as LSituationPort ?? engine.LEngineSituation,
        swap as LExamplePort ?? engine.LEngineExample,
        swap as LReferencePort ?? engine.LEngineReference,
        swap as LAuthorPort ?? engine.LEngineAuthor,
        swap as LMarkdownPort ?? engine.LEngineDraft,
        swap as LPronunciationPort ?? engine.LEnginePronunciation);

    internal static CEntryBundle TEntryBundleCreate(LEngine engine, string member, bool thrown) => new(
        TEngineFault.TEngineFaultCreate<LEntryPort>(engine.LEngineEntry, member, thrown),
        TEngineFault.TEngineFaultCreate<LGraspPort>(engine.LEngineEntry, member, thrown),
        TEngineFault.TEngineFaultCreate<LFavoritePort>(engine.LEngineVista, member, thrown),
        TEngineFault.TEngineFaultCreate<LVistaPort>(engine.LEngineVista, member, thrown),
        TEngineFault.TEngineFaultCreate<LCardPort>(engine.LEngineCard, member, thrown),
        TEngineFault.TEngineFaultCreate<LTagPort>(engine.LEngineCard, member, thrown),
        TEngineFault.TEngineFaultCreate<LRegisterPort>(engine.LEngineCard, member, thrown),
        TEngineFault.TEngineFaultCreate<LMentionPort>(engine.LEngineMention, member, thrown),
        TEngineFault.TEngineFaultCreate<LGlyphPort>(engine.LEngineLanguage, member, thrown),
        TEngineFault.TEngineFaultCreate<LSituationPort>(engine.LEngineSituation, member, thrown),
        TEngineFault.TEngineFaultCreate<LExamplePort>(engine.LEngineExample, member, thrown),
        TEngineFault.TEngineFaultCreate<LReferencePort>(engine.LEngineReference, member, thrown),
        TEngineFault.TEngineFaultCreate<LAuthorPort>(engine.LEngineAuthor, member, thrown),
        TEngineFault.TEngineFaultCreate<LMarkdownPort>(engine.LEngineDraft, member, thrown),
        TEngineFault.TEngineFaultCreate<LPronunciationPort>(engine.LEnginePronunciation, member, thrown));

    internal static CEntryBundle TEntryBundleCreate(Dictionary<string, Func<object?[]?, object?>> answers) => new(
        TEngineFake.TEngineCreate<LEntryPort>(answers),
        TEngineFake.TEngineCreate<LGraspPort>(answers),
        TEngineFake.TEngineCreate<LFavoritePort>(answers),
        TEngineFake.TEngineCreate<LVistaPort>(answers),
        TEngineFake.TEngineCreate<LCardPort>(answers),
        TEngineFake.TEngineCreate<LTagPort>(answers),
        TEngineFake.TEngineCreate<LRegisterPort>(answers),
        TEngineFake.TEngineCreate<LMentionPort>(answers),
        TEngineFake.TEngineCreate<LGlyphPort>(answers),
        TEngineFake.TEngineCreate<LSituationPort>(answers),
        TEngineFake.TEngineCreate<LExamplePort>(answers),
        TEngineFake.TEngineCreate<LReferencePort>(answers),
        TEngineFake.TEngineCreate<LAuthorPort>(answers),
        TEngineFake.TEngineCreate<LMarkdownPort>(answers),
        TEngineFake.TEngineCreate<LPronunciationPort>(answers));

    internal static CPhonologyBundle TPhonologyBundleCreate(LEngine engine) => new(
        engine.LEngineFanqie,
        engine.LEngineFanqie,
        engine.LEngineLanguage,
        engine.LEngineLanguage,
        engine.LEngineReflex,
        engine.LEngineVocabulary,
        engine.LEngineVocabulary,
        engine.LEngineStem);

    internal static CPhonologyBundle TPhonologyBundleCreate(LEngine engine, string member, bool thrown) => new(
        TEngineFault.TEngineFaultCreate<LFanqiePort>(engine.LEngineFanqie, member, thrown),
        TEngineFault.TEngineFaultCreate<LDiweiPort>(engine.LEngineFanqie, member, thrown),
        TEngineFault.TEngineFaultCreate<LScriptPort>(engine.LEngineLanguage, member, thrown),
        TEngineFault.TEngineFaultCreate<LLanguagePort>(engine.LEngineLanguage, member, thrown),
        TEngineFault.TEngineFaultCreate<LReflexPort>(engine.LEngineReflex, member, thrown),
        TEngineFault.TEngineFaultCreate<LParadigmPort>(engine.LEngineVocabulary, member, thrown),
        TEngineFault.TEngineFaultCreate<LSentencePort>(engine.LEngineVocabulary, member, thrown),
        TEngineFault.TEngineFaultCreate<LStemPort>(engine.LEngineStem, member, thrown));

    internal static CPhonologyBundle TPhonologyBundleCreate(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers.TryAdd("get_LEngineContourScale", _ => LLanguageClerk.LLanguageContourScale);
        answers.TryAdd("LEngineDiweiRead", args => LDiweiClerk.LDiweiKindRead(!(bool)args![0]!, (string)args[1]!));
        answers.TryAdd(
            "LEngineParadigmCheck",
            args => LParadigmClerk.LParadigmClerkCheck((LParadigmRow)args![0]!, (bool)args[1]!, (bool)args[2]!));
        return new(
            TEngineFake.TEngineCreate<LFanqiePort>(answers),
            TEngineFake.TEngineCreate<LDiweiPort>(answers),
            TEngineFake.TEngineCreate<LScriptPort>(answers),
            TEngineFake.TEngineCreate<LLanguagePort>(answers),
            TEngineFake.TEngineCreate<LReflexPort>(answers),
            TEngineFake.TEngineCreate<LParadigmPort>(answers),
            TEngineFake.TEngineCreate<LSentencePort>(answers),
            TEngineFake.TEngineCreate<LStemPort>(answers));
    }

    internal static LSettingsPort TSettingsOutletCreate(LEngine engine) => new LSettingsOutlet(engine);

    internal static CAtelier TAtelierMediaCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
        TEntryBundleCreate(engine),
        new LSettingsOutlet(engine),
        TPhonologyBundleCreate(engine),
        media,
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings) => new(
        new LPosture(engine),
        TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLeftoverSweep"] = _ => null,
            ["LEngineObserverAttach"] = _ => null,
            ["LEngineObserverDetach"] = _ => null,
        }),
        TEntryBundleCreate(engine),
        settings,
        TPhonologyBundleCreate(engine),
        TEngineFake.TEngineStubCreate<LMediaPort>(),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings, LDraftPort drafts) => new(
        new LPosture(engine),
        drafts,
        TEntryBundleCreate(engine),
        settings,
        TPhonologyBundleCreate(engine),
        TEngineFake.TEngineStubCreate<LMediaPort>(),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers["LEngineLeftoverSweep"] = _ => null;
        answers["LEngineRecordingStop"] = _ => null;
        return new CAtelier(
            new LPosture(engine),
            TEngineFake.TEngineCreate<LDraftPort>(answers),
            TEntryBundleCreate(answers),
            new LSettingsOutlet(engine),
            TPhonologyBundleCreate(answers),
            TEngineFake.TEngineCreate<LMediaPort>(answers),
            new LPortraitOutlet(engine));
    }

    internal static LMediaPort TMediaCreate(LEngine engine) => new LMediaOutlet(engine);

    internal static LMediaPort TMediaCreate() =>
        TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLocationRead"] = _ => null,
            ["LEngineScreenRead"] = _ => null,
        });

    internal static LSettingsPort TSettingsCreate() =>
        TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
            ["LEngineTextRead"] = args => (string)args![0]!,
            ["LEngineFontRead"] = _ => new LFont(null, 0),
        });

    internal static CVoyageState TVoyageRead(this CVoyage voyage) => voyage.LVoyageRead();

    internal static void TVoyageStationAdd(this CVoyage voyage, string tab, long id) =>
        voyage.LVoyageStationAdd(tab, id);

    internal static bool TVoyageUndo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageUndo(tab, id, show);

    internal static bool TVoyageRedo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageRedo(tab, id, show);

    internal static bool TAtelierSplitRead(CAtelier atelier) => atelier.LAtelierSplitRead();

    internal static CEstablishment TAtelierEstablishmentRead(this CAtelier atelier) =>
        atelier.LAtelierEstablishmentRead();

    internal static void TWorkspaceDraftAdd(this CWorkspace workspace, Func<bool> pending, Func<bool, bool> closure) =>
        workspace.LWorkspaceDraftAdd(pending, closure);

    internal static void TWorkspaceObserverAttach(this CWorkspace workspace, Action<Action> marshal) =>
        workspace.LWorkspaceObserverAttach(marshal);

    internal static void TWorkspaceStateAdd(this CWorkspace workspace, Action<CWorkspaceState> heard) =>
        workspace.LWorkspaceStateOpened += heard;

    internal static void TLedgerFailureShow(this CAtelier atelier, CEnvoy envoy, string key, Exception exception) =>
        CLedger.LLedgerFailureShow(envoy, atelier.CAtelierSettingsPort, key, exception);

    internal static void TLedgerRepaintShow(this CAtelier atelier, CEnvoy envoy, string key, Exception exception) =>
        atelier.CAtelierLedger.LLedgerRepaint.LLedgerRepaintShow(envoy, atelier.CAtelierSettingsPort, key, exception);

    internal static CWorkspaceState? TAtelierStateOpen(this CAtelier atelier)
    {
        CWorkspaceState? opened = null;
        Action<CWorkspaceState> heard = state => opened = state;
        atelier.CAtelierWorkspace.LWorkspaceStateOpened += heard;
        atelier.TAtelierStubOpen();
        atelier.CAtelierWorkspace.LWorkspaceStateOpened -= heard;
        return opened;
    }

    internal static void TAtelierStubOpen(this CAtelier atelier) =>
        atelier.CAtelierOpen(TEngineFake.TEngineStubCreate<CEnvoy>(), static run => run());

    internal static void TAtelierOpen(this CAtelier atelier, CEnvoy envoy) =>
        atelier.CAtelierOpen(envoy, static run => run());
}
