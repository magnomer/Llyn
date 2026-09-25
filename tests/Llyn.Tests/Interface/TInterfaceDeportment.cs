using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.Tests;

internal static class TInterfaceDeportment
{
    internal static LEditor TEditorCreate(LEngine engine) => new(
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LPhonologyOutlet(engine),
        new LSettingsOutlet(engine),
        static () => false);

    internal static int TCustomsCardScan(IReadOnlyList<LCardDraft> cards) => LSCustoms.LSCustomsCardScan(cards);

    internal static LMarkupIntake TCustomsIntakeCreate(int index, IReadOnlyList<LEntry> candidates) =>
        LSCustoms.LSCustomsIntakeCreate(index, candidates);

    internal static LMarkupIntake TCustomsIntakeCreate(int index, LMarkupMode mode, long target) =>
        LSCustoms.LSCustomsIntakeCreate(index, mode, target);

    internal static bool TCustomsReadyCheck(LMarkupMode mode, long target) =>
        LSCustoms.LSCustomsReadyCheck(mode, target);

    internal static string TCustomsLossResolve(LWindow? window, LMarkupMode mode, long target) =>
        LSCustoms.LSCustomsLossResolve(window, mode, target, "{0}/{1}");

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

    internal static LEntryDraft? TEditorDraftRead(this LEditor editor) => editor.LEditorDraftRead();

    internal static LClip TClipCreate() => new();

    internal static void TClipStepHandle(this LClip clip, LHarvestStep step) => clip.LClipStepHandle(step);

    internal static void TEditorClipStart(this LEditor editor, string word, long target, Action<LHarvestStep> sink) =>
        editor.LEditorClipStart(word, target, sink);

    internal static string TEditorGraspFormat(this LEditor editor, int step) => editor.LEditorGraspFormat(step);

    internal static void TEditorGraspSet(this LEditor editor, int step) => editor.LEditorGraspSet(step);

    internal static void TEditorFavoriteSet(this LEditor editor, bool marked) => editor.LEditorFavoriteSet(marked);

    internal static void TEditorTagAdd(this LEditor editor, long id) => editor.LEditorTagAdd(id);

    internal static void TEditorRegisterAdd(this LEditor editor, long id) => editor.LEditorRegisterAdd(id);

    internal static void TEditorSituationAdd(this LEditor editor, long id) => editor.LEditorSituationAdd(id);

    internal static void TEditorExampleAdd(this LEditor editor, long id) => editor.LEditorExampleAdd(id);

    internal static void TEditorReferenceAdd(this LEditor editor, long id) => editor.LEditorReferenceAdd(id);

    internal static LPhonology TPhonologyCreate(LEngine engine, Func<bool> leaveSeam, Func<bool> deleteSeam) =>
        new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            TEditorCreate(engine),
            () => true,
            leaveSeam,
            deleteSeam);

    internal static void TPhonologyVistaRestore(this LPhonology panel, LVista vista) =>
        panel.LPhonologyVistaRestore(vista);

    internal static LLibrary TLibraryCreate(LEngine engine, Func<bool> leaveSeam, Func<bool> deleteSeam) =>
        new(
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            TEditorCreate(engine),
            () => true,
            leaveSeam,
            deleteSeam);

    internal static void TLibraryVistaRestore(this LLibrary panel, LVista vista) =>
        panel.LLibraryVistaRestore(vista);

    internal static IReadOnlyList<LVistaRow> TLibraryRowsRead(this LLibrary panel) => panel.LLibraryRowsRead();

    internal static long TLibraryVoyageRead(this LLibrary panel) => panel.LLibraryVoyageRead();

    internal static LShelf TShelfCreate(LEngine engine, Func<bool> leaveSeam) =>
        new(
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            TEditorCreate(engine),
            () => true,
            leaveSeam,
            _ => true,
            () => false);

    internal static void TShelfVistaRestore(this LShelf shelf, LVista vista, LVista footnote) =>
        shelf.LShelfVistaRestore(vista, footnote);

    internal static IReadOnlyList<LCatalogReference> TShelfRowsRead(this LShelf shelf) => shelf.LShelfRowsRead();

    internal static void TShelfRowSelect(this LShelf shelf, long? id) => shelf.LShelfRowSelect(id);

    internal static void TShelfEntrySelect(this LShelf shelf, long? id) => shelf.LShelfEntrySelect(id);

    internal static void TShelfFreshStart(this LShelf shelf) => shelf.LShelfFreshStart();

    internal static void TShelfScribeSet(this LShelf shelf, bool editing) => shelf.LShelfScribeSet(editing);

    internal static void TShelfDelete(this LShelf shelf) => shelf.LShelfDelete();

    internal static LColophon TShelfColophonRead(this LShelf shelf, LDraft draft) => shelf.LShelfColophonRead(draft);

    internal static void TImprintOpen(this LImprint imprint, long? id) => imprint.LImprintOpen(id);

    internal static void TImprintCancel(this LImprint imprint) => imprint.LImprintCancel();

    internal static void TImprintSave(this LImprint imprint) => imprint.LImprintSave();

    internal static LReference TImprintReferenceRead(this LImprint imprint, LDraft draft) =>
        imprint.LImprintReferenceRead(draft);

    internal static string TImprintTallyRead(this LImprint imprint) => imprint.LImprintTallyRead();

    internal static void TImprintTitleSet(this LImprint imprint, string text) => imprint.LImprintTitleSet(text);

    internal static void TImprintYearSet(this LImprint imprint, string text) => imprint.LImprintYearSet(text);

    internal static void TImprintKindSet(this LImprint imprint, string? tag) => imprint.LImprintKindSet(tag);

    internal static IReadOnlyList<LAuthorRow> TImprintCreditRead(this LImprint imprint) =>
        imprint.LImprintCreditRead();

    internal static void TImprintCreditApply(this LImprint imprint, string? action, int? position, long? id) =>
        imprint.LImprintCreditApply(action, position, id);

    internal static bool TImprintKeyApply(
        this LImprint imprint, string key, int? position, long? id, string? text, long? chosen) =>
        imprint.LImprintKeyApply(key, position, id, text, chosen);

    internal static void TBylineWordSet(this LImprint imprint, string? text, bool? focused) =>
        imprint.LBylineWordSet(text, focused);

    internal static IReadOnlyList<LAuthor> TBylineRowsRead(this LImprint imprint) => imprint.LBylineRowsRead();

    internal static void TBylineSelect(this LImprint imprint, long? id, int? position, long? held) =>
        imprint.LBylineSelect(id, position, held);

    internal static void TBylineHide(this LImprint imprint) => imprint.LBylineHide();

    internal static void TPanelRowSelect(this LPanel panel, long? id) => panel.LPanelRowSelect(id);

    internal static void TPanelScribeSet(this LPanel panel, bool editing) => panel.LPanelScribeSet(editing);

    internal static void TPanelFreshStart(this LPanel panel) => panel.LPanelFreshStart();

    internal static void TPanelDelete(this LPanel panel) => panel.LPanelDelete();

    internal static IReadOnlyList<LCatalogPronunciation> TPhonologyRowsRead(this LPhonology panel) =>
        panel.LPhonologyRowsRead();

    internal static void TPhonologyQuerySet(this LPhonology panel, string query) => panel.LPhonologyQuerySet(query);

    internal static LDesk TDeskCreate(LEngine engine, string scope, Func<bool> unreadableSeam) =>
        new(new LDraftOutlet(engine), scope, unreadableSeam);

    internal static void TDeskVistaRestore(this LDesk desk, LVista vista) => desk.LDeskVistaRestore(vista);

    internal static void TDeskStart(this LDesk desk, long? id) => desk.LDeskStart(id);

    internal static LDraft? TDeskRead(this LDesk desk) => desk.LDeskRead();

    internal static void TDeskDefer(this LDesk desk, LRequest request) => desk.LDeskDefer(request);

    internal static bool TDeskChangeCheck(this LDesk desk) => desk.LDeskChangeCheck();

    internal static (bool LDeskBackward, bool LDeskForward) TDeskChronicleRead(this LDesk desk) =>
        desk.LDeskChronicleRead();

    internal static bool TDeskFinish(this LDesk desk, bool store) => desk.LDeskFinish(store);

    internal static void TDeskCancel(this LDesk desk) => desk.LDeskCancel();

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
            () => false);

    internal static void TGuildVistaRestore(this LGuild guild, LVista vista, LVista oeuvre) =>
        guild.LGuildVistaRestore(vista, oeuvre);

    internal static IReadOnlyList<LCatalogAuthor> TGuildRollRead(this LGuild guild) => guild.LGuildRollRead();

    internal static LVita TGuildVitaRead(this LGuild guild) => guild.LGuildVitaRead();

    internal static IReadOnlyList<LCatalogAuthor> TGuildUnionRead(this LGuild guild, string typed) =>
        guild.LGuildUnionRead(typed);

    internal static void TGuildQuerySet(this LGuild guild, string query) => guild.LGuildQuerySet(query);

    internal static void TGuildOrderSet(this LGuild guild, LCatalogOrder? order) => guild.LGuildOrderSet(order);

    internal static void TGuildRowSelect(this LGuild guild, long? id) => guild.LGuildRowSelect(id);

    internal static void TGuildSourceSelect(this LGuild guild, long? id) => guild.LGuildSourceSelect(id);

    internal static void TGuildScribeSet(this LGuild guild, bool editing) => guild.LGuildScribeSet(editing);

    internal static void TGuildFreshStart(this LGuild guild) => guild.LGuildFreshStart();

    internal static bool TGuildSave(this LGuild guild) => guild.LGuildSave();

    internal static void TGuildUnionSelect(this LGuild guild, long? id) => guild.LGuildUnionSelect(id);

    internal static void TGuildDelete(this LGuild guild) => guild.LGuildDelete();

    internal static IReadOnlyList<LCatalogReference> TOeuvreRowsRead(this LOeuvre oeuvre) => oeuvre.LOeuvreRowsRead();

    internal static LColophon TOeuvreColophonRead(this LOeuvre oeuvre, LDraft draft) =>
        oeuvre.LOeuvreColophonRead(draft);

    internal static LYunjing TYunjingCreate(LEngine engine, Func<bool> leaveSeam) =>
        new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            TEditorCreate(engine),
            () => true,
            leaveSeam,
            () => true);

    internal static void TYunjingVistaRestore(this LYunjing panel, LVista shengmu, LVista yunmu, LVista xiaoyun) =>
        panel.LYunjingVistaRestore(shengmu, yunmu, xiaoyun);

    internal static IReadOnlyList<LDiwei> TYunjingShengmuRead(this LYunjing panel) => panel.LYunjingShengmuRead();

    internal static IReadOnlyList<LDiwei> TYunjingYunmuRead(this LYunjing panel) => panel.LYunjingYunmuRead();

    internal static IReadOnlyList<LVistaRow> TYunjingXiaoyunRead(this LYunjing panel) =>
        panel.LYunjingXiaoyunRead();

    internal static LDiweiPage TYunjingDiweiRead(this LYunjing panel) => panel.LYunjingDiweiRead();

    internal static void TYunjingDiweiSelect(this LYunjing panel, long? id, bool? final) =>
        panel.LYunjingDiweiSelect(id, final);

    internal static void TYunjingDiweiShow(this LYunjing panel, string language, string kind, string key) =>
        panel.LYunjingDiweiShow(language, kind, key);

    internal static void TYunjingTallySet(this LYunjing panel, bool? respelled) => panel.LYunjingTallySet(respelled);
    internal static LWindow TWindowCreate(LSettingsPort settings) =>
        TWindowCreate(TEngineFake.TEngineStubCreate<LDraftPort>(), settings);

    internal static LWindow TWindowCreate(LDraftPort drafts) =>
        TWindowCreate(drafts, TEngineFake.TEngineStubCreate<LSettingsPort>());

    internal static LWindow TWindowCreate(LDraftPort drafts, LSettingsPort settings) =>
        new(
            new LPosture(new LEngine(TRigFake.TRigFakeBuild())),
            drafts,
            TEngineFake.TEngineStubCreate<LEntryPort>(),
            settings,
            TEngineFake.TEngineStubCreate<LPhonologyPort>(),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TEngineFake.TEngineStubCreate<LPortraitPort>());

    internal static LWindow TWindowCreate(LEntryPort entries) =>
        new(
            new LPosture(new LEngine(TRigFake.TRigFakeBuild())),
            TEngineFake.TEngineStubCreate<LDraftPort>(),
            entries,
            TEngineFake.TEngineStubCreate<LSettingsPort>(),
            TEngineFake.TEngineStubCreate<LPhonologyPort>(),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            TEngineFake.TEngineStubCreate<LPortraitPort>());

    internal static LWindow TWindowCreate(LEngine engine) =>
        new(
            new LPosture(engine),
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            new LSettingsOutlet(engine),
            new LPhonologyOutlet(engine),
            new LMediaOutlet(engine),
            new LPortraitOutlet(engine));

    internal static LFont TFontCreate(string family, double size) => new(family, size);

    internal static LFont TWindowFontRead(this LWindow window, string language, LFontRole role) =>
        window.LWindowFontRead(language, role);

    internal static string TWindowLocalizationRead(this LWindow window) => window.LWindowLocalizationRead();

    internal static void TWindowEpithetSave(this LWindow window, bool epithet) => window.LWindowEpithetSave(epithet);

    internal static LPostureState TWindowPostureRead(this LWindow window) => window.LWindowPostureRead();

    internal static void TWindowLayoutSave(this LWindow window, params LLayout[] layout) =>
        window.LWindowLayoutSave(layout);

    internal static void TWindowLayoutReset(this LWindow window) => window.LWindowLayoutReset();

    internal static void TWindowModeSave(this LWindow window, string mode) => window.LWindowModeSave(mode);

    internal static bool TWindowModeMatch(this LWindow window, string mode) => window.LWindowModeMatch(mode);

    internal static IReadOnlyList<LMentionPiece> TWindowMentionDivide(
        this LWindow window, string text, IReadOnlyList<LMention> mentions) =>
        window.LWindowMentionDivide(text, mentions);

    internal static LTenor TTenorCreate(LEntryPort entries, LSettingsPort settings) =>
        new(
            entries,
            TEngineFake.TEngineStubCreate<LPortraitPort>(),
            settings,
            new LEditor(
                TEngineFake.TEngineStubCreate<LDraftPort>(),
                entries,
                TEngineFake.TEngineStubCreate<LPhonologyPort>(),
                settings,
                static () => false),
            static () => true,
            static () => true,
            static () => true);

    internal static IReadOnlyList<LCatalogRegister> TTenorRowsRead(this LTenor tenor) => tenor.LTenorRowsRead();

    internal static IReadOnlyList<LVistaRow> TTenorCohortRead(this LTenor tenor) => tenor.LTenorCohortRead();

    internal static LRegister TTenorRegisterCreate(this LTenor tenor, string name) => tenor.LTenorRegisterCreate(name);

    internal static IReadOnlyList<string> TTenorLanguageRead(this LTenor tenor) => tenor.LTenorLanguageRead();

    internal static IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> TWindowMeaningSort(
        IReadOnlyList<LMeaning> meanings, string unknown) => LWindow.LWindowMeaningSort(meanings, unknown);

    internal static IReadOnlyList<LIndexItem> TIndexItemBuild(IReadOnlyList<LVistaRow> rows) =>
        LIndexItem.LIndexItemBuild(rows, static _ => null);

    internal static bool TIndexItemMatch(LIndexItem held, LIndexItem fresh) => LIndexItem.LIndexItemMatch(held, fresh);

    internal static void TIndexItemSync(LIndexItem held, LIndexItem fresh) => LIndexItem.LIndexItemSync(held, fresh);

    internal static long? TIndexNeighbourFind(IReadOnlyList<LIndexItem> items, bool down) =>
        LIndex.LIndexNeighbourFind(items, down);

    internal static bool TCaretKeyApply(
        string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place) =>
        LCaret.LCaretKeyApply(key, caret, length, selection, remove, move, place);
}
