using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConduct
{
    internal static IReadOnlyList<bool> TReflexLeadRead(IReadOnlyList<string> languages) =>
        CReflex.LReflexLeadRead(languages);

    internal static LDisplaySound TDisplaySoundCreate(LEngine engine) => new(
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LMediaOutlet(engine),
        new LSettingsOutlet(engine));

    internal static void TDisplayFoldSet(this LDisplaySound sound, bool opened) => sound.LDisplayFoldSet(opened);

    internal static void TDisplaySoundShow(this LDisplaySound sound, long? id, LEntryDraft draft) =>
        sound.LDisplaySoundShow(id, draft);

    internal static void TDisplaySoundClear(this LDisplaySound sound) => sound.LDisplaySoundClear();

    internal static void TDisplayReflexLoad(this LDisplaySound sound) => sound.LDisplayReflexLoad();

    internal static IReadOnlyList<LReflexDraft> TDisplayReflexRead(this LDisplaySound sound) =>
        sound.LDisplayReflexRead();

    internal static CFrequency? TDisplayFrequencyRead(this LDisplay display, long? entry, string once) =>
        display.LDisplayFrequencyRead(entry, once);

    internal static int TDisplayGraspRead(this LDisplay display, long? entry) => display.LDisplayGraspRead(entry);

    internal static CSCustoms TCustomsCreate(
        IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets) => new(entries, targets);

    internal static IReadOnlyList<CSCustomsRow> TCustomsRowsRead(this CSCustoms customs) =>
        customs.LSCustomsRowsRead();

    internal static CSCustomsRow TCustomsRowRead(this CSCustoms customs, int row) => customs.CSCustomsRowRead(row);

    internal static bool TCustomsModeSet(this CSCustoms customs, int row, CSCustomsMode mode) =>
        customs.CSCustomsModeSet(row, mode);

    internal static bool TCustomsTargetSet(this CSCustoms customs, int row, long target) =>
        customs.CSCustomsTargetSet(row, target);

    internal static bool TCustomsReadyCheck(this CSCustoms customs) => customs.CSCustomsReadyCheck();

    internal static bool TCoinageWordingCheck(string? wording) => CSCoinage.CSCoinageWordingCheck(wording);

    internal static bool TCatalogFilterMatch(this CCatalogFilter filter, string? language) =>
        filter.CCatalogFilterMatch(language);

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
        TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ => null,
        }),
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

    internal static CAtelier TAtelierMediaCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
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
        answers["LEngineRecordingStop"] = _ => null;
        return new CAtelier(
            new LPosture(engine),
            TEngineFake.TEngineCreate<LDraftPort>(answers),
            TEngineFake.TEngineCreate<LEntryPort>(answers),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineCreate<LPhonologyPort>(answers),
            TEngineFake.TEngineCreate<LMediaPort>(answers),
            new LPortraitOutlet(engine));
    }

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
        TEnvoyFake.TEnvoyCreate(false, []));

    internal static CEditor TEditorCreate(LEngine engine, LPhonologyPort phonology) =>
        TEditorCreate(
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            phonology,
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>());

    internal static LMediaPort TMediaCreate(LEngine engine) => new LMediaOutlet(engine);

    internal static CEditor TEditorCreate(
        LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology, LSettingsPort settings, LMediaPort media) =>
        new(drafts, entries, phonology, settings, media, TEnvoyFake.TEnvoyCreate(false, []));
    internal static void TEditorVistaRestore(this CEditor editor, LVista vista) => editor.LEditorVistaRestore(vista);

    internal static CVistaRow TPanelRowRead(LVistaRow row) => CPanel.CPanelRowRead(row);

    internal static CPanel TPanelCreate(
        CEnvoy envoy, string? deleteScope, Func<bool> changeSeam, Func<bool, bool> finishSeam) =>
        new(envoy, TSettingsCreate(), "List.LoadFailed", deleteScope, changeSeam, finishSeam, static () => true);

    internal static LSettingsPort TSettingsCreate() =>
        TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
        });

    internal static void TPanelVistaRestore(this CPanel panel, LVista vista) => panel.CPanelVistaRestore(vista);

    internal static LVista? TPanelVistaRead(this CPanel panel) => panel.CPanelVista;

    internal static CCatalogOrder TPanelOrderRead(LCatalogOrder order) => CPanel.CPanelOrderRead(order);

    internal static LCatalogOrder? TPanelOrderRead(CCatalogOrder? order) => CPanel.CPanelOrderRead(order);

    internal static CCatalogFilter TPanelFilterRead(LCatalogFilter filter) => CPanel.CPanelFilterRead(filter);

    internal static LSubject TPanelSubjectRead(CSubject subject) => CPanel.CPanelSubjectRead(subject);

    internal static COeuvre TOeuvreCreate(LEngine engine) =>
        new(
            new LEntryOutlet(engine),
            new LSettingsOutlet(engine),
            TEnvoyFake.TEnvoyCreate(true, []),
            static () => true);

    internal static void TOeuvreVistaRestore(this COeuvre oeuvre, LVista roll, LVista vista) =>
        oeuvre.LOeuvreVistaRestore(roll, vista);

    internal static IReadOnlyList<CCatalogAuthor> TOeuvreAuthorRead(
        this COeuvre oeuvre, IReadOnlyList<LCatalogAuthor> rows) => oeuvre.LOeuvreAuthorRead(rows);

    internal static COccurrence TOccurrenceCreate(LEngine engine) => new(
        new LEntryOutlet(engine),
        new LPortraitOutlet(engine),
        new LSettingsOutlet(engine),
        TEnvoyFake.TEnvoyCreate(false, []),
        static () => false,
        static _ => true,
        static () => true);

    internal static void TOccurrenceVistaRestore(this COccurrence occurrence, LVista roll, LVista vista) =>
        occurrence.LOccurrenceVistaRestore(roll, vista);

    internal static void TAtlasVistaRestore(this CAtlas atlas, LVista vista) => atlas.LAtlasVistaRestore(vista);

    internal static IReadOnlyList<CCatalogSituation>? TAtlasRowsRead(this CAtlas atlas) => atlas.LAtlasRowsRead();

    internal static IReadOnlyList<CCatalogSituation>? TAtlasFailRead(LEngine engine, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineSituationFind"] = _ => throw new InvalidOperationException("no situations"),
        });
        CAtlas atlas = new(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            TDeskCreate(engine, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation),
            static () => true,
            envoy,
            static _ => true);
        atlas.LAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));
        return atlas.LAtlasRowsRead();
    }

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

    internal static IReadOnlyList<CCatalogExample> TAnthologyRowsRead(this CAnthology anthology) =>
        anthology.LAnthologyRowsRead() ?? throw new InvalidOperationException("The rows read failed.");

    internal static CExample? TAnthologyExampleRead(LExample? example, string citation, string tally) =>
        CAnthology.LAnthologyExampleRead(example, citation, tally);

    internal static void TDeskOccurrenceStart(this CDesk desk, long? situation) =>
        desk.LDeskOccurrenceStart(situation);

    internal static void TDeskQuotationStart(this CDesk desk, long? example) => desk.LDeskQuotationStart(example);

    internal static void TDeskFootnoteStart(this CDesk desk, long? reference) => desk.LDeskFootnoteStart(reference);

    internal static void TDeskMembershipStart(this CDesk desk, long? tag) => desk.LDeskMembershipStart(tag);

    internal static void TDeskCohortStart(this CDesk desk, long? register) => desk.LDeskCohortStart(register);


    internal static void TFootnoteVistaRestore(this CFootnote footnote, LVista parent, LVista vista) =>
        footnote.LFootnoteVistaRestore(parent, vista);

    internal static void TFootnoteEntryCreate(this CFootnote footnote) => footnote.LFootnoteEntryCreate();

    internal static void TImprintOpen(this CImprint imprint, long? id) => imprint.LImprintOpen(id);

    internal static void TImprintCancel(this CImprint imprint) => imprint.LImprintCancel();

    internal static void TImprintSave(this CImprint imprint) => imprint.LImprintSave();

    internal static LVistaRow TVistaRowCreate(long id, string epithet, bool chosen) =>
        new(id, "aqua", "Latin", epithet, "aqua (1)", chosen);

    internal static CCard TCardCreate(LEngine engine, CDesk desk, CEnvoy envoy) => new(
        desk, new LDraftOutlet(engine), new LEntryOutlet(engine), new LSettingsOutlet(engine), envoy);

    internal static CCardList TCardListCreate(CDesk desk) => new(desk);

    internal static CSentence TSentenceCreate(LEngine engine, CDesk desk) => new(
        desk,
        new LPhonologyOutlet(engine),
        new LDraftOutlet(engine),
        TSettingsCreate(),
        TEnvoyFake.TEnvoyCreate(false, []));

    internal static CSounding TSoundingCreate(
        CDesk desk, LPhonologyPort phonology, LDraftPort drafts, CEnvoy envoy, LSettingsPort? pack = null)
    {
        LSettingsPort settings = pack ?? TSettingsCreate();
        return new(
            desk,
            phonology,
            drafts,
            settings,
            new LDisplaySound(
                TEngineFake.TEngineStubCreate<LEntryPort>(),
                phonology,
                TEngineFake.TEngineStubCreate<LMediaPort>(),
                settings),
            envoy);
    }

    internal static CStateValue TCardStateRead(LStateValue value) => CFolio.CFolioStateRead(value);

    internal static CSentenceOrder TCatalogOrderRead(LEngine engine, string language) =>
        CCatalog.CCatalogOrderRead(new LPhonologyOutlet(engine).LEngineOrderRead(language));

    internal static CEntryDraft TCardEntryRead(
        LEntryDraft draft, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets) =>
        CFolio.CFolioEntryRead(draft, targets);

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

    internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium) =>
        CPortrait.LPortraitMediumRead(medium);

    internal static LPressTicket TPortraitTicketRead(CPressTicket ticket) => CPortrait.LPortraitTicketRead(ticket);

    internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings) => CPortrait.LPortraitLabelRead(settings);

    internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm) =>
        CPortrait.LPortraitLegendRead(settings, realm);

    internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead() => CPortrait.LPortraitChoiceRead();

    internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export) =>
        CPortrait.LPortraitFileExport(envoy, TSettingsCreate(), file, export);

    internal static Task TPortraitTicketPrint(CEnvoy envoy, Func<LPressTicket, Task> print) =>
        CPortrait.LPortraitTicketPrint(envoy, TSettingsCreate(), print);

    internal static string TFavoriteFileRead(this CFavorite favorite) => favorite.LFavoriteFileRead();

    internal static string TLibraryFileRead(this CLibrary library) => library.LLibraryFileRead();

    internal static string TPhonologyFileRead(this CPhonology phonology) => phonology.LPhonologyFileRead();

    internal static string TMembershipFileRead(this CMembership membership) => membership.LMembershipFileRead();

    internal static string TTenorFileRead(this CTenor tenor) => tenor.LTenorFileRead();

    internal static string TXieshengFileRead(this CXiesheng xiesheng) => xiesheng.LXieshengFileRead();

    internal static string TYunjingFileRead(this CYunjing yunjing) => yunjing.LYunjingFileRead();

    internal static string TQuotationFileRead(this CQuotation quotation) => quotation.LQuotationFileRead();

    internal static string TOccurrenceFileRead(this COccurrence occurrence) => occurrence.LOccurrenceFileRead();

    internal static string TFootnoteFileRead(this CFootnote footnote) => footnote.LFootnoteFileRead();

    internal static long TPanelChosenRead(this CPanel panel) => panel.LPanelChosenRead();

    internal static void TPanelStationAttach(this CPanel panel, Action record) => panel.LPanelStationAttach(record);

    internal static void TPanelScribeRestore(this CPanel panel, bool editing) => panel.LPanelScribeRestore(editing);

    internal static bool TAtelierSplitRead(CAtelier atelier) => atelier.LAtelierSplitRead();

    internal static void TGuildScribeRestore(this CGuild guild, bool editing) => guild.LGuildScribeRestore(editing);

    internal static bool TRepertoireLeaveConfirm(this CRepertoire repertoire) =>
        repertoire.LRepertoireLeaveConfirm(true);

    internal static bool TShelfLeaveConfirm(this CShelf shelf) => shelf.LShelfLeaveConfirm();

    internal static CVoyageState TVoyageRead(this CVoyage voyage) => voyage.LVoyageRead();

    internal static void TVoyageStationAdd(this CVoyage voyage, string tab, long id) =>
        voyage.LVoyageStationAdd(tab, id);

    internal static bool TVoyageUndo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageUndo(tab, id, show);

    internal static bool TVoyageRedo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageRedo(tab, id, show);

    internal static bool TSessionChangeCheck(this CSession session) => session.LSessionChangeCheck();

    internal static bool TSessionFinish(this CSession session, bool store) => session.LSessionFinish(store);

    internal static bool TShelfChangeRead(this CShelf shelf) => shelf.LShelfChangeRead();

    internal static bool TShelfDraftFinish(this CShelf shelf, bool store) => shelf.LShelfDraftFinish(store);

    internal static bool TEditorFinish(this CEditor editor, bool store) => editor.LEditorFinish(store);

    internal static bool TDeskChangeCheck(this CDesk desk) => desk.LDeskChangeCheck();

    internal static CEstablishment TAtelierEstablishmentRead(this CAtelier atelier) =>
        atelier.LAtelierEstablishmentRead();

    internal static void TWorkspaceDraftAdd(this CWorkspace workspace, Func<bool> pending, Func<bool, bool> closure) =>
        workspace.LWorkspaceDraftAdd(pending, closure);

    internal static void TWorkspaceStateAdd(this CWorkspace workspace, Action<CWorkspaceState> heard) =>
        workspace.LWorkspaceStateOpened += heard;

    internal static CWorkspaceState? TAtelierStateOpen(this CAtelier atelier)
    {
        CWorkspaceState? opened = null;
        Action<CWorkspaceState> heard = state => opened = state;
        atelier.CAtelierWorkspace.LWorkspaceStateOpened += heard;
        atelier.CAtelierOpen();
        atelier.CAtelierWorkspace.LWorkspaceStateOpened -= heard;
        return opened;
    }

    internal static CExample? TCorpusTranscriptRead(this CCorpus corpus) => corpus.LCorpusTranscriptRead();

    internal static void TCorpusEntryResonate(this CCorpus corpus) => corpus.LCorpusEntryResonate();

    internal static bool TCorpusLeaveConfirm(this CCorpus corpus) => corpus.LCorpusLeaveConfirm(true);
}
