using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Llyn.Tests;

internal static class TInterfaceDeportment
{
    internal static LEditor TEditorCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LSettingsOutlet(engine),
        new LMediaOutlet(engine),
        TInterfaceConduct.TEnvoyCreate(false, []));

    internal static void TEditorVistaRestore(this LEditor editor, LVista vista) => editor.LEditorVistaRestore(vista);

    internal static void TEditorOpen(this LEditor editor, long? id) => editor.LEditorOpen(id);

    internal static void TEditorHeadwordSet(this LEditor editor, string text) => editor.LEditorHeadwordSet(text);

    internal static void TEditorNoteSet(this LEditor editor, string text) => editor.LEditorNoteSet(text);

    internal static void TEditorLanguageSet(this LEditor editor, string language) =>
        editor.LEditorLanguageSet(language);

    internal static void TEditorPersist(this LEditor editor) => editor.LEditorPersist();

    internal static void TEditorSave(this LEditor editor) => editor.LEditorSave();

    internal static void TEditorReset(this LEditor editor) => editor.LEditorReset();

    internal static bool TEditorFinish(this LEditor editor, bool store) => editor.LEditorFinish(store);

    internal static CEntryDraft? TEditorDraftRead(this LEditor editor) => editor.LEditorDraftRead();

    internal static LLectern TLecternCreate(LEditor editor) => new(editor.LEditorDisplay);

    internal static LClip TClipCreate(LEngine engine) =>
        new(TInterfaceConduct.TDeskCreate(engine, "Input", TInterfaceConduct.TEnvoyCreate(false, [])));

    internal static void TClipStepHandle(this LClip clip, CHarvestStep step) => clip.LClipStepHandle(step);

    internal static bool TEditorClipStart(this LEditor editor, string word, long target, Action<CHarvestStep> sink) =>
        editor.LEditorClipStart(word, target, sink);

    internal static string TEditorGraspFormat(this LEditor editor, int step) =>
        editor.LEditorEsteem.QEsteemGraspFormat(step);

    internal static void TEditorGraspSet(this LEditor editor, int step) => editor.LEditorEsteem.QEsteemGraspSet(step);

    internal static void TEditorFavoriteSet(this LEditor editor, bool marked) =>
        editor.LEditorEsteem.QEsteemFavoriteSet(marked);

    internal static void TEditorTagAdd(this LEditor editor, long id) => editor.LEditorTagAdd(id);

    internal static void TEditorRegisterAdd(this LEditor editor, long id) => editor.LEditorRegisterAdd(id);

    internal static void TEditorSituationAdd(this LEditor editor, long id) => editor.LEditorSituationAdd(id);

    internal static void TEditorExampleAdd(this LEditor editor, long id) => editor.LEditorExampleAdd(id);

    internal static void TEditorReferenceAdd(this LEditor editor, long id) => editor.LEditorReferenceAdd(id);

    internal static LPhonology TPhonologyCreate(LEngine engine, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            leaveSeam,
            deleteSeam);
    }

    internal static void TPhonologyVistaRestore(this LPhonology panel, LVista vista) =>
        panel.LPhonologyVistaRestore(vista);

    internal static LLibrary TLibraryCreate(LEngine engine, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            leaveSeam,
            deleteSeam);
    }

    internal static void TLibraryVistaRestore(this LLibrary panel, LVista vista) =>
        panel.LLibraryVistaRestore(vista);

    internal static IReadOnlyList<CVistaRow> TLibraryRowsRead(this LLibrary panel) => panel.LLibraryRowsRead();

    internal static long TLibraryVoyageRead(this LLibrary panel) => panel.LLibraryVoyageRead();

    internal static LShelf TShelfCreate(LEngine engine, Func<bool> leaveSeam)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            leaveSeam,
            _ => true,
            TInterfaceConduct.TEnvoyCreate(false, []));
    }

    internal static void TShelfVistaRestore(this LShelf shelf, LVista vista, LVista footnote) =>
        shelf.LShelfVistaRestore(vista, footnote);

    internal static IReadOnlyList<CCatalogReference> TShelfRowsRead(this LShelf shelf) => shelf.LShelfRowsRead();

    internal static void TShelfRowSelect(this LShelf shelf, long? id) => shelf.LShelfRowSelect(id);

    internal static void TShelfEntrySelect(this LShelf shelf, long? id) => shelf.LShelfEntrySelect(id);

    internal static void TShelfFreshStart(this LShelf shelf) => shelf.LShelfFreshStart();

    internal static void TShelfScribeSet(this LShelf shelf, bool editing) => shelf.LShelfScribeSet(editing);

    internal static void TShelfDelete(this LShelf shelf) => shelf.LShelfDelete();

    internal static IReadOnlyList<CCatalogOrder> TShelfOrderRead() => LShelf.LShelfOrderRead();

    internal static void TShelfOrderSet(this LShelf shelf, CCatalogOrder order) => shelf.LShelfOrderSet(order);

    internal static void TImprintOpen(this LImprint imprint, long? id) => imprint.LImprintOpen(id);

    internal static void TImprintCancel(this LImprint imprint) => imprint.LImprintCancel();

    internal static void TImprintSave(this LImprint imprint) => imprint.LImprintSave();

    internal static string TImprintTallyRead(this LImprint imprint) => imprint.LImprintTallyRead();

    internal static void TImprintTitleSet(this LImprint imprint, string text) => imprint.LImprintTitleSet(text);

    internal static void TImprintYearSet(this LImprint imprint, string text) => imprint.LImprintYearSet(text);

    internal static void TImprintKindSet(this LImprint imprint, string? tag) => imprint.LImprintKindSet(tag);

    internal static IReadOnlyList<CAuthorRow> TImprintCreditRead(this LImprint imprint) =>
        imprint.LImprintCreditRead();

    internal static void TImprintAuthorAdd(this LImprint imprint, int? position, long? id) =>
        imprint.LImprintAuthorAdd(position, id);

    internal static void TImprintAuthorRemove(this LImprint imprint, long? id) => imprint.LImprintAuthorRemove(id);

    internal static void TImprintAuthorRetreat(this LImprint imprint, int? position, long? id) =>
        imprint.LImprintAuthorRetreat(position, id);

    internal static void TImprintAuthorAdvance(this LImprint imprint, int? position, long? id) =>
        imprint.LImprintAuthorAdvance(position, id);

    internal static bool TImprintKeyApply(
        this LImprint imprint, string key, int? position, long? id, string? text, long? chosen) =>
        imprint.LImprintKeyApply(key, position, id, text, chosen);

    internal static void TBylineWordSet(this LImprint imprint, string? text, bool? focused) =>
        imprint.LBylineWordSet(text, focused);

    internal static IReadOnlyList<CAuthor> TBylineRowsRead(this LImprint imprint) => imprint.LBylineRowsRead();

    internal static void TBylineSelect(this LImprint imprint, long? id, int? position, long? held) =>
        imprint.LBylineSelect(id, position, held);

    internal static void TBylineHide(this LImprint imprint) => imprint.LBylineHide();

    internal static void TPanelRowSelect(this LPanel panel, long? id) => panel.LPanelRowSelect(id);

    internal static void TPanelScribeSet(this LPanel panel, bool editing) => panel.LPanelScribeSet(editing);

    internal static void TPanelFreshStart(this LPanel panel) => panel.LPanelFreshStart();

    internal static void TPanelDelete(this LPanel panel) => panel.LPanelDelete();

    internal static CCatalogOrder TPanelOrderRead(LCatalogOrder order) => LPanel.LPanelOrderRead(order);

    internal static LCatalogOrder? TPanelOrderRead(CCatalogOrder? order) => LPanel.LPanelOrderRead(order);

    internal static CCatalogFilter TPanelFilterRead(LCatalogFilter filter) => LPanel.LPanelFilterRead(filter);

    internal static LCatalogFilter TPanelFilterRead(CCatalogFilter filter) => LPanel.LPanelFilterRead(filter);

    internal static LSubject TPanelSubjectRead(CSubject subject) => LPanel.LPanelSubjectRead(subject);

    internal static LPortraitMedium TPanelMediumRead(CPortraitMedium medium) => QPortrait.QPortraitMediumRead(medium);

    internal static LPressTicket TPanelTicketRead(CPressTicket ticket) => QPortrait.QPortraitTicketRead(ticket);

    internal static LPortraitLabel TPanelLabelRead(CPortraitLabel label) => QPortrait.QPortraitLabelRead(label);

    internal static LTaxonomy TTaxonomyCreate(LEntryPort entries, LSettingsPort settings)
    {
        LEditor editor = new(
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            entries,
            TEngineFake.TEngineStubCreate<LPhonologyPort>(),
            settings,
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TInterfaceConduct.TEnvoyCreate(false, []));
        return new(
            entries,
            TEngineFake.TEngineStubCreate<LPortraitPort>(),
            settings,
            editor,
            TLecternCreate(editor),
            static () => true,
            static () => true,
            static () => true);
    }

    internal static IReadOnlyList<CCatalogTag> TTaxonomyRowsRead(this LTaxonomy taxonomy) =>
        taxonomy.LTaxonomyRowsRead();

    internal static long TTaxonomyTagCreate(this LTaxonomy taxonomy, string name) =>
        taxonomy.LTaxonomyTagCreate(name);

    internal static bool TTaxonomyCoinageCheck(this LTaxonomy taxonomy) => taxonomy.LTaxonomyCoinageCheck();

    internal static string TTaxonomyEmptyRead(this LTaxonomy taxonomy, string query) =>
        taxonomy.LTaxonomyEmptyRead(query);

    internal static IReadOnlyList<CCatalogTag> TTaxonomyTagRead(IReadOnlyList<LCatalogTag> rows) =>
        LTaxonomy.LTaxonomyTagRead(rows);

    internal static LCatalogTag TCatalogTagCreate(LTag tag, bool chosen) => new(tag, chosen);

    internal static IReadOnlyList<CCatalogPronunciation> TPhonologyRowsRead(this LPhonology panel) =>
        panel.LPhonologyRowsRead();

    internal static void TPhonologyQuerySet(this LPhonology panel, string query) => panel.LPhonologyQuerySet(query);

    internal static IReadOnlyList<CParadigmSlot> TSoundingParadigmRead(IReadOnlyList<LParadigmSlot> slots) =>
        LSounding.LSoundingParadigmRead(slots);

    internal static CFrequency? TSoundingFrequencyRead(IReadOnlyList<LFrequency> rows, string once) =>
        LSounding.LSoundingFrequencyRead(rows, once);

    internal static bool TDeskChangeCheck(this CDesk desk) => desk.CDeskChangeCheck();

    internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk) =>
        desk.CDeskChronicleRead();

    internal static LGuild TGuildCreate(
        LEngine engine, Func<bool> leaveSeam, Func<int, bool> removalSeam, Func<string, string, bool> unionSeam) =>
        new(
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            () => true,
            leaveSeam,
            removalSeam,
            unionSeam,
            TInterfaceConduct.TEnvoyCreate(false, []));

    internal static void TGuildVistaRestore(this LGuild guild, LVista vista, LVista oeuvre) =>
        guild.LGuildVistaRestore(vista, oeuvre);

    internal static IReadOnlyList<CCatalogAuthor> TGuildRollRead(this LGuild guild) => guild.LGuildRollRead();

    internal static CVita TGuildVitaRead(this LGuild guild) => guild.LGuildVitaRead();

    internal static IReadOnlyList<CCatalogAuthor> TGuildUnionRead(this LGuild guild, string typed) =>
        guild.LGuildUnion.QUnionRead(typed);

    internal static void TGuildQuerySet(this LGuild guild, string query) => guild.LGuildQuerySet(query);

    internal static void TGuildOrderSet(this LGuild guild, CCatalogOrder? order) => guild.LGuildOrderSet(order);

    internal static void TGuildRowSelect(this LGuild guild, long? id) => guild.LGuildRowSelect(id);

    internal static void TGuildSourceSelect(this LGuild guild, long? id) => guild.LGuildSourceSelect(id);

    internal static void TGuildScribeSet(this LGuild guild, bool editing) => guild.LGuildScribeSet(editing);

    internal static void TGuildFreshStart(this LGuild guild) => guild.LGuildFreshStart();

    internal static bool TGuildSave(this LGuild guild) => guild.LGuildSession.CSessionSave();

    internal static void TGuildUnionSelect(this LGuild guild, long? id) => guild.LGuildUnion.QUnionSelect(id);

    internal static void TGuildDelete(this LGuild guild) => guild.LGuildDelete();

    internal static IReadOnlyList<CCatalogReference> TOeuvreRowsRead(this LOeuvre oeuvre) => oeuvre.LOeuvreRowsRead();

    internal static LYunjing TYunjingCreate(LEngine engine, Func<bool> leaveSeam)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            leaveSeam,
            () => true);
    }

    internal static void TYunjingVistaRestore(this LYunjing panel, LVista shengmu, LVista yunmu, LVista xiaoyun) =>
        panel.LYunjingVistaRestore(shengmu, yunmu, xiaoyun);

    internal static IReadOnlyList<CDiwei> TYunjingShengmuRead(this LYunjing panel) => panel.LYunjingShengmuRead();

    internal static IReadOnlyList<CDiwei> TYunjingYunmuRead(this LYunjing panel) => panel.LYunjingYunmuRead();

    internal static IReadOnlyList<CVistaRow> TYunjingXiaoyunRead(this LYunjing panel) =>
        panel.LYunjingXiaoyunRead();

    internal static CDiweiPage TYunjingDiweiRead(this LYunjing panel) => panel.LYunjingDiweiRead();

    internal static void TYunjingDiweiSelect(this LYunjing panel, long? id, bool? final) =>
        panel.LYunjingDiweiSelect(id, final);

    internal static void TYunjingDiweiShow(this LYunjing panel, string language, string kind, string key) =>
        panel.LYunjingDiweiShow(language, kind, key);

    internal static void TYunjingTallySet(this LYunjing panel, bool? respelled) => panel.LYunjingTallySet(respelled);

    internal static QPosture TPostureCreate(string root) => new(() => root);

    internal static LTenor TTenorCreate(LEntryPort entries, LSettingsPort settings)
    {
        LEditor editor = new(
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            entries,
            TEngineFake.TEngineStubCreate<LPhonologyPort>(),
            settings,
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TInterfaceConduct.TEnvoyCreate(false, []));
        return new(
            entries,
            TEngineFake.TEngineStubCreate<LPortraitPort>(),
            settings,
            editor,
            TLecternCreate(editor),
            static () => true,
            static () => true,
            static () => true);
    }

    internal static IReadOnlyList<CCatalogRegister> TTenorRowsRead(this LTenor tenor) => tenor.LTenorRowsRead();

    internal static IReadOnlyList<CVistaRow> TTenorCohortRead(this LTenor tenor) => tenor.LTenorCohortRead();

    internal static long TTenorRegisterCreate(this LTenor tenor, string name) => tenor.LTenorRegisterCreate(name);

    internal static bool TTenorCoinageCheck(this LTenor tenor) => tenor.LTenorCoinageCheck();

    internal static IReadOnlyList<CCatalogRegister> TTenorRegisterRead(IReadOnlyList<LCatalogRegister> rows) =>
        LTenor.LTenorRegisterRead(rows);

    internal static LCatalogRegister TCatalogRegisterCreate(LRegister register, int usage, bool chosen) =>
        new(register, usage, chosen);

    internal static IReadOnlyList<string> TTenorLanguageRead(this LTenor tenor) => tenor.LTenorLanguageRead();

    internal static IReadOnlyList<QIndexItem> TIndexItemBuild(IReadOnlyList<CVistaRow> rows) =>
        QIndexItem.QIndexItemBuild(rows);

    internal static bool TIndexItemMatch(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemMatch(held, fresh);

    internal static void TIndexItemSync(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemSync(held, fresh);

    internal static long? TIndexNeighbourFind(IReadOnlyList<QIndexItem> items, bool down) =>
        QIndex.QIndexNeighbourFind(items, down);

    internal static bool TCaretKeyApply(
        string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place) =>
        QCaret.QCaretKeyApply(key, caret, length, selection, remove, move, place);

    internal static void TLecternFoldSet(this LLectern lectern, bool opened) => lectern.LLecternFoldSet(opened);

    internal static void TLecternReflexAttach(
        this LLectern lectern, ItemsControl reflex, UIElement loading, ToggleButton fold) =>
        lectern.LLecternSound.LLecternReflexAttach(reflex, loading, fold);

    internal static void TLecternFoldHandle(this LLectern lectern, bool opened) =>
        lectern.LLecternSound.LLecternFoldHandle(opened);

    internal static void TReflexFoldApply(IReadOnlyList<LReflexItem> rows, ToggleButton fold, bool opened) =>
        LReflexItem.LReflexFoldApply(rows, fold, opened);
}
