using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductPortrait
{
    internal static LPortraitMedium TPortraitMediumRead(CPortraitMedium medium) =>
        CPortrait.LPortraitMediumRead(medium);

    internal static LPortraitLabel TPortraitLabelRead(LSettingsPort settings) => CPortrait.LPortraitLabelRead(settings);

    internal static LPortraitLegend TPortraitLegendRead(LSettingsPort settings, string realm) =>
        CPortrait.LPortraitLegendRead(settings, realm);

    internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead() => CPortrait.LPortraitChoiceRead();

    internal static IReadOnlyList<CPortraitChoice> TPortraitChoiceRead(
        IReadOnlyList<(LPortraitMedium, string, bool)> rows) =>
        CPortrait.LPortraitChoiceRead(rows);

    internal static Task TPortraitFileExport(CEnvoy envoy, string file, Func<string, LPortraitMedium, Task> export) =>
        CPortrait.LPortraitFileExport(envoy, TInterfaceConduct.TSettingsCreate(), () => file, export);

    internal static Task TPortraitFileExport(
        CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export) =>
        CPortrait.LPortraitFileExport(envoy, settings, fileRead, export);

    internal static Task TPortraitPrint(CEnvoy envoy, Func<Task<LGalleyRelay?>> open) =>
        CPortrait.LPortraitPrint(envoy, TInterfaceConduct.TSettingsCreate(), open);

    internal static async Task TPortraitPrint(
        CEnvoy envoy, LPressBoard board, Exception? failure = null, TPressVault? vault = null)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        workspace.TWorkspacePressVault = vault ?? workspace.TWorkspacePressVault;
        workspace.TWorkspacePress.TPressBoard = board;
        workspace.TWorkspaceQueue.TPressQueueFailure = failure;
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LVista vista = engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderName);
        vista.TVistaSelect(entry.LEntryId);
        LSettingsPort settings = TInterfaceConduct.TSettingsCreate();
        LPortraitPort portrait = new LPortraitOutlet(engine);

        await CPortrait.LPortraitPrint(
            envoy, settings, () => portrait.LEnginePressOpen(vista, CPortrait.LPortraitLabelRead(settings)));
    }

    internal static Task TPressWindowOpen(
        LPressBoard board,
        Func<CSPress, Task> check,
        string? path = "press.pdf",
        List<string>? asked = null,
        Exception? failure = null,
        TPressVault? vault = null)
    {
        CEnvoy envoy = TEnvoyFake.TEnvoyPressCreate(path, press =>
        {
            Task.Run(async () =>
            {
                press.TPressDelaySet(Timeout.Infinite);
                await press.TPressRestore();
                await check(press);
            }).GetAwaiter().GetResult();
            return true;
        }, asked ?? []);

        return TPortraitPrint(envoy, board, failure, vault);
    }
}
