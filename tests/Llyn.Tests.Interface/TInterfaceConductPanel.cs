using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductPanel
{
    internal static bool TCatalogFilterMatch(this CCatalogFilter filter, string? language) =>
        filter.CCatalogFilterMatch(language);

    internal static CVistaRow TPanelRowRead(LVistaRow row) => CCatalog.LCatalogRowRead(row);

    internal static CArticulation TCatalogArticulationRead(
        IReadOnlyList<string> headers,
        IReadOnlyList<string> sides,
        IReadOnlyList<IReadOnlyList<IReadOnlyList<string>>> cells) =>
        CCatalog.LCatalogArticulationRead(new LArticulation(headers, sides, cells));

    internal static CPanel TPanelCreate(
        LEngine engine, CEnvoy envoy, string? deleteScope, Func<bool> changeSeam, Func<bool, bool> finishSeam) =>
        new(
            envoy,
            TInterfaceConduct.TSettingsCreate(),
            engine.LEngineVista,
            "List.LoadFailed",
            deleteScope,
            changeSeam,
            finishSeam,
            static () => true);

    internal static void TPanelVistaRestore(this CPanel panel, LVista vista) => panel.CPanelVistaRestore(vista);

    internal static LVista? TPanelVistaRead(this CPanel panel) => panel.CPanelVista;

    internal static CCatalogOrder TPanelOrderRead(LCatalogOrder order) => CCatalog.LCatalogOrderRead(order);

    internal static LCatalogOrder? TPanelOrderRead(CCatalogOrder? order) => CCatalog.LCatalogOrderRead(order);

    internal static CCatalogFilter TPanelFilterRead(LCatalogFilter filter) => CCatalog.LCatalogFilterRead(filter);

    internal static LSubject TPanelSubjectRead(CSubject subject) => CCatalog.LCatalogSubjectRead(subject);

    internal static COeuvre TOeuvreCreate(LEngine engine) =>
        new(
            engine.LEngineEntry,
            engine.LEngineAuthor,
            engine.LEngineReference,
            new LSettingsOutlet(engine),
            engine.LEngineVista,
            TEnvoyFake.TEnvoyCreate(true, []),
            static () => true);

    internal static void TOeuvreVistaRestore(this COeuvre oeuvre, LVista roll, LVista vista) =>
        oeuvre.LOeuvreVistaRestore(roll, vista);

    internal static IReadOnlyList<CCatalogAuthor> TOeuvreAuthorRead(
        this COeuvre oeuvre, IReadOnlyList<LCatalogAuthor> rows) => oeuvre.LOeuvreAuthorRead(rows);

    internal static COccurrence TOccurrenceCreate(LEngine engine) => new(
        engine.LEngineVista,
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
        LSituationPort situations = TEngineFake.TEngineCreate<LSituationPort>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineSituationFind"] = _ => throw new InvalidOperationException("no situations"),
            });
        CAtlas atlas = new(
            situations,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            engine.LEngineVista,
            TInterfaceConductDesk.TDeskCreate(engine, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation),
            static () => true,
            envoy,
            static _ => true);
        atlas.LAtlasVistaRestore(engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName));
        return atlas.LAtlasRowsRead();
    }

    internal static CSituationDraft? TAtlasDraftRead(LSituation? situation) =>
        CAtlas.LAtlasDraftRead(TAtlasDraftCreate(situation), TInterfaceConduct.TMediaCreate());

    internal static CSituation? TAtlasSituationRead(LSituation? situation) =>
        CAtlas.LAtlasSituationRead(
            TAtlasDraftCreate(situation), TInterfaceConduct.TMediaCreate(), TInterfaceMarkdown.TMarkdownPortCreate());

    private static LDraft? TAtlasDraftCreate(LSituation? situation) =>
        situation is null
            ? null
            : new LDraft(
                0,
                "repertoire",
                0,
                TInterface.TEntryDraftCreate(string.Empty, "English", string.Empty, string.Empty, [], []),
                DateTimeOffset.UnixEpoch,
                LDraftSituation: situation);

    internal static CAnthology TAnthologyCreate(CAtelier atelier, CDesk desk, CEnvoy envoy) =>
        CAnthology.LAnthologyCreate(atelier, desk, static () => true, envoy, static _ => true);

    internal static void TAnthologyVistaRestore(this CAnthology anthology, LVista vista) =>
        anthology.LAnthologyVistaRestore(vista);

    internal static IReadOnlyList<CCatalogExample> TAnthologyRowsRead(this CAnthology anthology) =>
        anthology.LAnthologyRowsRead() ?? throw new InvalidOperationException("The rows read failed.");

    internal static bool TAnthologyTextCheck(this CAnthology anthology, string text, CStateValue value) =>
        anthology.LAnthologyTextCheck(text, value);

    internal static CExample? TAnthologyExampleRead(LExample? example, string citation, string tally) =>
        CAnthology.LAnthologyExampleRead(example, citation, tally);

    internal static void TFootnoteVistaRestore(this CFootnote footnote, LVista parent, LVista vista) =>
        footnote.LFootnoteVistaRestore(parent, vista);

    internal static void TFootnoteEntryCreate(this CFootnote footnote) => footnote.LFootnoteEntryCreate();

    internal static void TImprintOpen(this CImprint imprint, long? id) => imprint.LImprintOpen(id);

    internal static void TImprintCancel(this CImprint imprint) => imprint.LImprintCancel();

    internal static void TImprintSave(this CImprint imprint) => imprint.LImprintSave();

    internal static IReadOnlyList<CReferenceKind> TImprintKindRead(
        IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> rows) =>
        CImprint.LImprintKindRead(rows);

    internal static LVistaRow TVistaRowCreate(long id, string epithet, bool chosen) =>
        new(id, "aqua", "Latin", epithet, "aqua (1)", chosen);

    internal static string TFavoriteFileRead(this CFavorite favorite) => favorite.LFavoriteFileRead();

    internal static string TLibraryFileRead(this CLibrary library) => library.LLibraryFileRead();

    internal static string TPhonologyFileRead(this CPhonology phonology) => phonology.LPhonologyFileRead();

    internal static string TMembershipFileRead(this CMembership membership) => membership.LMembershipFileRead();

    internal static string TCohortFileRead(this CCohort cohort) => cohort.LCohortFileRead();

    internal static string TXieshengFileRead(this CXiesheng xiesheng) => xiesheng.LXieshengFileRead();

    internal static string TYunjingFileRead(this CYunjing yunjing) => yunjing.LYunjingFileRead();

    internal static string TQuotationFileRead(this CQuotation quotation) => quotation.LQuotationFileRead();

    internal static string TOccurrenceFileRead(this COccurrence occurrence) => occurrence.LOccurrenceFileRead();

    internal static string TFootnoteFileRead(this CFootnote footnote) => footnote.LFootnoteFileRead();

    internal static long TPanelChosenRead(this CPanel panel) => panel.LPanelChosenRead();

    internal static void TPanelStationAttach(this CPanel panel, Action record) => panel.LPanelStationAttach(record);

    internal static void TPanelScribeRestore(this CPanel panel, bool editing) => panel.LPanelScribeRestore(editing);

    internal static void TGuildScribeRestore(this CGuild guild, bool editing) => guild.LGuildScribeRestore(editing);

    internal static bool TRepertoireLeaveConfirm(this CRepertoire repertoire) =>
        repertoire.LRepertoireLeaveConfirm(true);

    internal static bool TShelfLeaveConfirm(this CShelf shelf) => shelf.LShelfLeaveConfirm();

    internal static bool TShelfChangeRead(this CShelf shelf) => shelf.LShelfChangeRead();

    internal static bool TShelfDraftFinish(this CShelf shelf, bool store) => shelf.LShelfDraftFinish(store);

    internal static CExample? TCorpusTranscriptRead(this CCorpus corpus) => corpus.CCorpusTranscript.LTranscriptRead();

    internal static void TCorpusEntryResonate(this CCorpus corpus) => corpus.LCorpusEntryResonate();

    internal static bool TCorpusLeaveConfirm(this CCorpus corpus) => corpus.LCorpusLeaveConfirm(true);
}
