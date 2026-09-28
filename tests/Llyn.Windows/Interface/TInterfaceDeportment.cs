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
    internal static LEditor TEditorCreate(LEngine engine) => new(TInterfaceConduct.TEditorCreate(engine));

    internal static void TEditorVistaRestore(this LEditor editor, LVista vista) =>
        editor.LEditorStudio.TEditorVistaRestore(vista);

    internal static LLectern TLecternCreate(LEditor editor) => new(editor.LEditorStudio.CEditorDisplay);

    internal static LClip TClipCreate(LEngine engine) =>
        new(TInterfaceConduct.TDeskCreate(engine, "Input", TInterfaceConduct.TEnvoyCreate(false, [])));

    internal static void TClipStepHandle(this LClip clip, CHarvestStep step) => clip.LClipStepHandle(step);

    internal static LPhonology TPhonologyCreate(LEngine engine)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            TInterfaceConduct.TEnvoyCreate(true, []));
    }

    internal static void TPhonologyVistaRestore(this LPhonology panel, LVista vista) =>
        panel.LPhonologyVistaRestore(vista);

    internal static LLibrary TLibraryCreate(LEngine engine)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LEntryOutlet(engine),
            new LPortraitOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            TInterfaceConduct.TEnvoyCreate(true, []));
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
            TInterfaceConduct.TEnvoyCreate(true, []));
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

    internal static void TPanelRowSelect(this CPanel panel, long? id) => panel.CPanelRowSelect(id);

    internal static void TPanelScribeToggle(this CPanel panel, bool editing) => panel.CPanelScribeToggle(editing);

    internal static void TPanelDelete(this CPanel panel) => panel.CPanelEntryDelete();

    internal static LTaxonomy TTaxonomyCreate(LEntryPort entries, LSettingsPort settings)
    {
        LDraftPort drafts = TEngineFake.TEngineStubCreate<LDraftPort>();
        LPhonologyPort phonology = TEngineFake.TEngineStubCreate<LPhonologyPort>();
        LEditor editor = new(
            TInterfaceConduct.TEditorCreate(
                drafts, entries, phonology, settings, TEngineFake.TEngineStubCreate<LMediaPort>()));
        return new(
            entries,
            TEngineFake.TEngineStubCreate<LPortraitPort>(),
            settings,
            editor,
            TLecternCreate(editor),
            static () => true,
            TInterfaceConduct.TEnvoyCreate(true, []));
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

    internal static bool TDeskChangeCheck(this CDesk desk) => desk.CDeskChangeCheck();

    internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk) =>
        desk.CDeskChronicleRead();

    internal static LYunjing TYunjingCreate(LEngine engine)
    {
        LEditor editor = TEditorCreate(engine);
        return new(
            new LPhonologyOutlet(engine),
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            editor,
            TLecternCreate(editor),
            () => true,
            TInterfaceConduct.TEnvoyCreate(true, []));
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
        LDraftPort drafts = TEngineFake.TEngineStubCreate<LDraftPort>();
        LPhonologyPort phonology = TEngineFake.TEngineStubCreate<LPhonologyPort>();
        LEditor editor = new(
            TInterfaceConduct.TEditorCreate(
                drafts, entries, phonology, settings, TEngineFake.TEngineStubCreate<LMediaPort>()));
        return new(
            entries,
            TEngineFake.TEngineStubCreate<LPortraitPort>(),
            settings,
            editor,
            TLecternCreate(editor),
            static () => true,
            TInterfaceConduct.TEnvoyCreate(true, []));
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
