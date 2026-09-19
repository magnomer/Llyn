using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.Tests;

internal static class TInterfaceDeportment
{
    internal static LPhonology TPhonologyCreate(
        LEngine engine, Func<bool> changeSeam, Func<bool> leaveSeam, Func<bool> deleteSeam) =>
        new(engine, changeSeam, () => true, leaveSeam, deleteSeam);

    internal static void TPhonologyVistaRestore(this LPhonology panel, LVista vista) =>
        panel.LPhonologyVistaRestore(vista);

    internal static LLibrary TLibraryCreate(
        LEngine engine, Func<bool> changeSeam, Func<bool> leaveSeam, Func<bool> deleteSeam) =>
        new(engine, changeSeam, () => true, leaveSeam, deleteSeam);

    internal static void TLibraryVistaRestore(this LLibrary panel, LVista vista) =>
        panel.LLibraryVistaRestore(vista);

    internal static IReadOnlyList<LVistaRow> TLibraryRowsRead(this LLibrary panel) => panel.LLibraryRowsRead();

    internal static long TLibraryVoyageRead(this LLibrary panel) => panel.LLibraryVoyageRead();

    internal static LShelf TShelfCreate(
        LEngine engine, Func<bool> sourceChangeSeam, Func<bool> entryChangeSeam, Func<bool> leaveSeam) =>
        new(engine, sourceChangeSeam, entryChangeSeam, () => true, leaveSeam, _ => true);

    internal static void TShelfVistaRestore(this LShelf shelf, LVista vista, LVista footnote) =>
        shelf.LShelfVistaRestore(vista, footnote);

    internal static IReadOnlyList<LCatalogReference> TShelfRowsRead(this LShelf shelf) => shelf.LShelfRowsRead();

    internal static void TShelfRowSelect(this LShelf shelf, long? id) => shelf.LShelfRowSelect(id);

    internal static void TShelfEntrySelect(this LShelf shelf, long? id) => shelf.LShelfEntrySelect(id);

    internal static void TShelfFreshStart(this LShelf shelf) => shelf.LShelfFreshStart();

    internal static void TShelfScribeSet(this LShelf shelf, bool editing) => shelf.LShelfScribeSet(editing);

    internal static void TShelfDelete(this LShelf shelf) => shelf.LShelfDelete();

    internal static LColophon TShelfColophonRead(this LShelf shelf, LDraft draft) => shelf.LShelfColophonRead(draft);

    internal static void TPanelRowSelect(this LPanel panel, long? id) => panel.LPanelRowSelect(id);

    internal static void TPanelScribeSet(this LPanel panel, bool editing) => panel.LPanelScribeSet(editing);

    internal static void TPanelFreshStart(this LPanel panel) => panel.LPanelFreshStart();

    internal static void TPanelDelete(this LPanel panel) => panel.LPanelDelete();

    internal static IReadOnlyList<LCatalogPronunciation> TPhonologyRowsRead(this LPhonology panel) =>
        panel.LPhonologyRowsRead();

    internal static void TPhonologyQuerySet(this LPhonology panel, string query) => panel.LPhonologyQuerySet(query);

    internal static LDesk TDeskCreate(LEngine engine, string scope, Func<bool> unreadableSeam) =>
        new(engine, scope, unreadableSeam);

    internal static void TDeskVistaRestore(this LDesk desk, LVista vista) => desk.LDeskVistaRestore(vista);

    internal static void TDeskStart(this LDesk desk, long? id) => desk.LDeskStart(id);

    internal static LDraft? TDeskRead(this LDesk desk) => desk.LDeskRead();

    internal static void TDeskDefer(this LDesk desk, LRequest request) => desk.LDeskDefer(request);

    internal static bool TDeskChangeCheck(this LDesk desk) => desk.LDeskChangeCheck();

    internal static bool TDeskFinish(this LDesk desk, bool store) => desk.LDeskFinish(store);

    internal static void TDeskCancel(this LDesk desk) => desk.LDeskCancel();

    internal static LGuild TGuildCreate(
        LEngine engine, Func<bool> leaveSeam, Func<int, bool> removalSeam, Func<string, string, bool> unionSeam) =>
        new(engine, () => true, leaveSeam, removalSeam, unionSeam, () => false);

    internal static void TGuildVistaRestore(this LGuild guild, LVista vista, LVista oeuvre) =>
        guild.LGuildVistaRestore(vista, oeuvre);

    internal static IReadOnlyList<LCatalogAuthor> TGuildRollRead(this LGuild guild) => guild.LGuildRollRead();

    internal static LVita TGuildVitaRead(this LGuild guild) => guild.LGuildVitaRead();

    internal static IReadOnlyList<LCatalogAuthor> TGuildUnionRead(this LGuild guild, string typed) =>
        guild.LGuildUnionRead(typed);

    internal static void TGuildQuerySet(this LGuild guild, string query) => guild.LGuildQuerySet(query);

    internal static void TGuildOrderSet(this LGuild guild, string? choice) => guild.LGuildOrderSet(choice);

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

    internal static LYunjing TYunjingCreate(LEngine engine, Func<bool> changeSeam, Func<bool> leaveSeam) =>
        new(engine, changeSeam, () => true, leaveSeam, () => true);

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
}
