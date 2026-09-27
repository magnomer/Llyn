using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConduct
{
    internal static string TDisplayStampFormat(string? utc) => LDisplay.LDisplayStampFormat(utc);

    internal static LDisplaySound TDisplaySoundCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LMediaOutlet(engine),
        new LSettingsOutlet(engine));

    internal static void TDisplayFoldSet(this LDisplaySound sound, bool opened) => sound.LDisplayFoldSet(opened);

    internal static HashSet<string> TDisplayFoldScan(IReadOnlyList<LReflexRule> rules) =>
        LDisplaySound.LDisplayFoldScan(
            rules.Select(static rule => new CReflexRule(rule.LReflexRuleLanguage, rule.LReflexRuleFolded)).ToList());

    internal static int TDisplayBandResolve(IReadOnlyList<LFrequency> rows) => LDisplay.LDisplayBandResolve(rows);

    internal static string TDisplayBandRead(int count) => LDisplay.LDisplayBandRead(count, string.Empty);

    internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft) =>
        sound.LDisplaySoundShow(id, draft);

    internal static void TDisplaySoundClear(this LDisplaySound sound) => sound.LDisplaySoundClear();

    internal static void TDisplayReflexLoad(this LDisplaySound sound) => sound.LDisplayReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound) =>
        sound.LDisplayReflexRead();

    internal static string TDisplaySourceFormat(IReadOnlyList<LFrequency> rows, string once) =>
        LDisplay.LDisplaySourceFormat(rows, once);

    internal static int TCustomsCardScan(IReadOnlyList<LCardDraft> cards) =>
        CSCustoms.CSCustomsCardScan(cards, card => card.LCardDraftChild);

    internal static CSCustoms TCustomsCreate(IReadOnlyList<IReadOnlyList<long>> candidates) => new(candidates);

    internal static CSCustomsRow TCustomsRowRead(this CSCustoms customs, int row) => customs.CSCustomsRowRead(row);

    internal static bool TCustomsModeSet(this CSCustoms customs, int row, CSCustomsMode mode) =>
        customs.CSCustomsModeSet(row, mode);

    internal static bool TCustomsTargetSet(this CSCustoms customs, int row, long target) =>
        customs.CSCustomsTargetSet(row, target);

    internal static bool TCustomsReadyCheck(this CSCustoms customs) => customs.CSCustomsReadyCheck();

    internal static bool TCoinageWordingCheck(string? wording) => CSCoinage.CSCoinageWordingCheck(wording);

    internal static bool TCatalogFilterMatch(this CCatalogFilter filter, string? language) =>
        filter.CCatalogFilterMatch(language);

    internal static IReadOnlyList<CCitationRow> TCitationRowFind(
        IReadOnlyList<CCatalogReference> found, string word, long? source) =>
        CCitationRow.CCitationRowFind(found, word, source);

    internal static LFont TFontCreate(string family, double size) => new(family, size);

    internal static CAtelier TAtelierCreate(LEngine engine) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LSettingsOutlet(engine),
        new LPhonologyOutlet(engine),
        TEngineFake.TEngineStubCreate<LMediaPort>(),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLeftoverSweep"] = _ => null,
        }),
        new LEntryOutlet(engine),
        new LSettingsOutlet(engine),
        new LPhonologyOutlet(engine),
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
        new LEntryOutlet(engine),
        settings,
        new LPhonologyOutlet(engine),
        TEngineFake.TEngineStubCreate<LMediaPort>(),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers["LEngineLeftoverSweep"] = _ => null;
        return new CAtelier(
            new LPosture(engine),
            TEngineFake.TEngineCreate<LDraftPort>(answers),
            TEngineFake.TEngineCreate<LEntryPort>(answers),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineCreate<LPhonologyPort>(answers),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            new LPortraitOutlet(engine));
    }

    internal static CEnvoy TEnvoyCreate(bool? answer, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyConfirm"] = args =>
            {
                asked.Add((string)args![0]!);
                return answer ?? false;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
            ["CEnvoyLeaveConfirm"] = _ =>
            {
                asked.Add("Leave");
                return answer;
            },
        });

    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy) =>
        new(new LDraftOutlet(engine), scope, envoy);

    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy, string origin, CSubject subject) =>
        new(new LDraftOutlet(engine), scope, envoy, origin, subject);

    internal static void TDeskVistaRestore(this CDesk desk, LVista vista) => desk.CDeskVistaRestore(vista);

    internal static void TDeskDefer(this CDesk desk, LRequest request) => desk.CDeskDefer(request);

    internal static LDraft? TDeskRead(this CDesk desk) => desk.CDeskRead();

    internal static CRecording? TErrandRecordingRead(LRecording? recording) => CErrand.CErrandRecordingRead(recording);

    internal static LRecording TErrandRecordingRead(CRecording recording) => CErrand.CErrandRecordingRead(recording);

    internal static CCandidate? TErrandCandidateRead(LCandidate? candidate) => CErrand.CErrandCandidateRead(candidate);

    internal static CSession TSessionCreate(
        CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam) =>
        new(
            desk, pending, null, static () => false, static _ => true, readySeam, storedSeam);

    internal static CSession TSessionCreate(CDesk desk, CDesk editor, Func<bool> shownSeam, List<string> seen) =>
        new(
            desk, [], editor, shownSeam,
            store =>
            {
                seen.Add(store ? "Finish" : "Drop");
                return true;
            },
            static () => true, static _ => { });
}
