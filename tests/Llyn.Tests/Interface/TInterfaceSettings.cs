using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LSettings TSettingsCreate(
        string localization,
        LWindowState? window = null,
        double volume = 1,
        bool respelled = false,
        bool frequency = true,
        bool morphology = true,
        IReadOnlyList<LLayout>? layout = null,
        bool linked = true,
        string? mode = null,
        bool split = false,
        bool epithet = true,
        bool tally = false) =>
        new(localization, window, volume, respelled, frequency, morphology, layout, linked, mode, split, epithet,
            tally);

    internal static LLayout TLayoutCreate(
        string tab,
        double? left = null,
        double? middle = null,
        LCatalogOrder? order = null,
        LCatalogFilter? filter = null) =>
        new(tab, left, middle, order, filter);

    internal static LCatalogFilter TCatalogFilterCreate(params string[] hidden) =>
        new(hidden);

    internal static bool TSettingsExist(string root) =>
        LSettingsLoader.LSettingsLoaderExist(root);

    internal static LSettings TSettingsLoad(string root) =>
        LSettingsLoader.LSettingsLoaderLoad(root);

    internal static void TSettingsSave(string root, LSettings settings)
    {
        LSettingsLoader.LSettingsLoaderSave(root, settings);
    }

    internal static LWindowState TWindowStateCreate(
        double left,
        double top,
        double width,
        double height,
        bool maximized) =>
        new(left, top, width, height, maximized);

    internal static void TEngineLeftSave(this LEngine engine, long? id)
    {
        engine.LEngineLeftSave(id);
    }

    internal static void TEngineRightSave(this LEngine engine, long? id)
    {
        engine.LEngineRightSave(id);
    }

    internal static void TEngineModeSave(this LEngine engine, string mode)
    {
        engine.LEngineModeSave(mode);
    }

    internal static void TEngineSplitSave(this LEngine engine, bool split)
    {
        engine.LEngineSplitSave(split);
    }
}
