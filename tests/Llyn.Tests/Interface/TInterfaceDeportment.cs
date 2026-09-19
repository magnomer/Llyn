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

    internal static void TPanelRowSelect(this LPanel panel, long? id) => panel.LPanelRowSelect(id);

    internal static void TPanelScribeSet(this LPanel panel, bool editing) => panel.LPanelScribeSet(editing);

    internal static void TPanelFreshStart(this LPanel panel) => panel.LPanelFreshStart();

    internal static void TPanelDelete(this LPanel panel) => panel.LPanelDelete();

    internal static IReadOnlyList<LCatalogPronunciation> TPhonologyRowsRead(this LPhonology panel) =>
        panel.LPhonologyRowsRead();

    internal static void TPhonologyQuerySet(this LPhonology panel, string query) => panel.LPhonologyQuerySet(query);
}
