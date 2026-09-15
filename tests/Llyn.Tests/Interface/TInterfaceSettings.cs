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
        bool split = false) =>
        new(localization, window, volume, respelled, frequency, morphology, layout, linked, mode, split);

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

    internal static void TEngineOrderSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineOrderSave(order);
    }

    internal static void TEngineSequenceSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineSequenceSave(order);
    }

    internal static void TEngineSeriesSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineSeriesSave(order);
    }

    internal static void TEngineFunnelSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineFunnelSave(order);
    }

    internal static void TEngineTierSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineTierSave(order);
    }

    internal static void TEngineGradeSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineGradeSave(order);
    }

    internal static void TEngineRankSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineRankSave(order);
    }

    internal static void TEngineDegreeSave(this LEngine engine, LCatalogOrder order)
    {
        engine.LEngineDegreeSave(order);
    }

    internal static void TEngineSieveSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineSieveSave(filter);
    }

    internal static void TEngineLensSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineLensSave(filter);
    }

    internal static void TEngineStrainerSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineStrainerSave(filter);
    }

    internal static void TEngineLatticeSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineLatticeSave(filter);
    }

    internal static void TEngineGrilleSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineGrilleSave(filter);
    }

    internal static void TEngineMeshSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineMeshSave(filter);
    }

    internal static void TEngineGauzeSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineGauzeSave(filter);
    }

    internal static void TEngineTrellisSave(this LEngine engine, LCatalogFilter filter)
    {
        engine.LEngineTrellisSave(filter);
    }
}
