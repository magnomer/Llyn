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

    internal static string TDisplayBandRead(int count) => LDisplay.LDisplayBandRead(count, string.Empty);

    internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft) =>
        sound.LDisplaySoundShow(id, draft);

    internal static void TDisplaySoundClear(this LDisplaySound sound) => sound.LDisplaySoundClear();

    internal static void TDisplayReflexLoad(this LDisplaySound sound) => sound.LDisplayReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound) =>
        sound.LDisplayReflexRead();

    internal static CFrequency? TDisplayFrequencyRead(this LDisplay display, long? entry, string once) =>
        display.LDisplayFrequencyRead(entry, once);

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

    internal static IReadOnlyList<CCitationRow> TCitationRowFind(IReadOnlyList<CCatalogReference> found, string word) =>
        CCitationRow.CCitationRowFind(found, word);

    internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>
        example with { LExampleMention = [mention] };

    internal static CMentionResult TMentionResultRead(int offset, LMention? stored) =>
        CMention.CMentionResultRead(new LMentionResult(offset, stored, []));

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
            ["CEnvoyUnionConfirm"] = args =>
            {
                asked.Add(string.Join(">", args!));
                return answer ?? false;
            },
        });

    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy) =>
        new(new LDraftOutlet(engine), scope, envoy);

    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy, string origin, CSubject subject) =>
        new(new LDraftOutlet(engine), scope, envoy, origin, subject);

    internal static CEditor TEditorCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        TEnvoyCreate(false, []));

    internal static CEditor TEditorCreate(
        LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media) =>
        new(drafts, entries, phonology, settings, media, TEnvoyCreate(false, []));
    internal static void TEditorVistaRestore(this CEditor editor, LVista vista) => editor.CEditorVistaRestore(vista);

    internal static CVistaRow TPanelRowRead(LVistaRow row) => CPanel.CPanelRowRead(row);

    internal static CPanel TPanelCreate(
        CEnvoy envoy, string? deleteScope, Func<bool> changeSeam, Func<bool, bool> finishSeam) =>
        new(envoy, "List.LoadFailed", deleteScope, changeSeam, finishSeam, static () => true);

    internal static void TPanelVistaRestore(this CPanel panel, LVista vista) => panel.CPanelVistaRestore(vista);

    internal static LVista? TPanelVistaRead(this CPanel panel) => panel.CPanelVista;

    internal static CCatalogOrder TPanelOrderRead(LCatalogOrder order) => CPanel.CPanelOrderRead(order);

    internal static LCatalogOrder? TPanelOrderRead(CCatalogOrder? order) => CPanel.CPanelOrderRead(order);

    internal static CCatalogFilter TPanelFilterRead(LCatalogFilter filter) => CPanel.CPanelFilterRead(filter);

    internal static LSubject TPanelSubjectRead(CSubject subject) => CPanel.CPanelSubjectRead(subject);

    internal static COeuvre TOeuvreCreate(LEngine engine) =>
        new(new LEntryOutlet(engine), TEnvoyCreate(true, []), static () => true);

    internal static void TOeuvreVistaRestore(this COeuvre oeuvre, LVista roll, LVista vista) =>
        oeuvre.LOeuvreVistaRestore(roll, vista);

    internal static IReadOnlyList<CCatalogAuthor> TOeuvreAuthorRead(
        this COeuvre oeuvre, IReadOnlyList<LCatalogAuthor> rows) => oeuvre.LOeuvreAuthorRead(rows);

    internal static COccurrence TOccurrenceCreate(LEngine engine) => new(
        new LEntryOutlet(engine),
        new LPortraitOutlet(engine),
        TEnvoyCreate(false, []),
        static () => false,
        static _ => true,
        static () => true);

    internal static void TOccurrenceVistaRestore(this COccurrence occurrence, LVista roll, LVista vista) =>
        occurrence.LOccurrenceVistaRestore(roll, vista);

    internal static void TAtlasVistaRestore(this CAtlas atlas, LVista vista) => atlas.LAtlasVistaRestore(vista);

    internal static IReadOnlyList<CCatalogSituation> TAtlasRowsRead(
        this CAtlas atlas, string unknown, string untitled) => atlas.LAtlasRowsRead(unknown, untitled);

    internal static CSituationDraft? TAtlasDraftRead(LSituation? situation) => CAtlas.LAtlasDraftRead(
        situation is null
            ? null
            : new LDraft(
                0,
                "repertoire",
                0,
                TInterface.TEntryDraftCreate(string.Empty, "English", string.Empty, string.Empty, [], []),
                DateTimeOffset.UnixEpoch,
                LDraftSituation: situation));

    internal static CAnthology TAnthologyCreate(CAtelier atelier, CDesk desk, CEnvoy envoy) =>
        CAnthology.LAnthologyCreate(atelier, desk, static () => true, envoy, static _ => true);

    internal static void TAnthologyVistaRestore(this CAnthology anthology, LVista vista) =>
        anthology.LAnthologyVistaRestore(vista);

    internal static IReadOnlyList<CCatalogExample> TAnthologyRowsRead(
        this CAnthology anthology, string unknown, string unwritten) =>
        anthology.LAnthologyRowsRead(unknown, unwritten);

    internal static CExample? TAnthologyExampleRead(LExample? example) => CAnthology.LAnthologyExampleRead(example);

    internal static void TDeskOccurrenceStart(this CDesk desk, long? situation) =>
        desk.LDeskOccurrenceStart(situation);

    internal static void TDeskQuotationStart(this CDesk desk, long? example) => desk.LDeskQuotationStart(example);

    internal static void TDeskFootnoteStart(this CDesk desk, long? reference) => desk.LDeskFootnoteStart(reference);

    internal static void TDeskMembershipStart(this CDesk desk, long? tag) => desk.LDeskMembershipStart(tag);

    internal static void TFootnoteVistaRestore(this CFootnote footnote, LVista parent, LVista vista) =>
        footnote.LFootnoteVistaRestore(parent, vista);

    internal static void TFootnoteEntryCreate(this CFootnote footnote) => footnote.LFootnoteEntryCreate();

    internal static void TImprintOpen(this CImprint imprint, long? id) => imprint.LImprintOpen(id);

    internal static void TImprintCancel(this CImprint imprint) => imprint.LImprintCancel();

    internal static void TImprintSave(this CImprint imprint) => imprint.LImprintSave();

    internal static LVistaRow TVistaRowCreate(long id, string? epithet, bool chosen) =>
        new(id, "aqua", "Latin", epithet, "aqua (1)", chosen);

    internal static CCard TCardCreate(LEngine engine, CDesk desk) => new(
        desk, new LDraftOutlet(engine), new LEntryOutlet(engine));

    internal static CSentence TSentenceCreate(LEngine engine, CDesk desk) => new(
        desk, new LPhonologyOutlet(engine));

    internal static CSounding TSoundingCreate(
        CDesk desk, LPhonologyPort phonology, LDraftPort drafts, CEnvoy envoy) => new(desk, phonology, drafts, envoy);

    internal static CStateValue TCardStateRead(LStateValue value) => CFolio.CFolioStateRead(value);

    internal static CEntryDraft TCardEntryRead(LEntryDraft draft) => CFolio.CFolioEntryRead(draft);

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
