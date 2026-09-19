using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LSettings TSettingsCreate(
        string localization,
        bool respelled = false,
        bool frequency = true,
        bool morphology = true,
        bool epithet = true,
        bool tally = false) =>
        new(localization, respelled, frequency, morphology, epithet, tally);

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
        new LSettingsLoader(root).LSettingsExist();

    internal static LSettings TSettingsLoad(string root) =>
        new LSettingsLoader(root).LSettingsRead();

    internal static void TSettingsSave(string root, LSettings settings)
    {
        new LSettingsLoader(root).LSettingsSave(settings);
    }

    internal static LWindowState TWindowStateCreate(
        double left,
        double top,
        double width,
        double height,
        bool maximized) =>
        new(left, top, width, height, maximized);

    internal static LPostureState TPostureStateCreate(
        LWindowState? window = null,
        IReadOnlyList<LLayout>? layout = null,
        bool linked = true,
        string? mode = null,
        bool split = false,
        double volume = 1) =>
        new(window, layout, linked, mode, split, volume);

    internal static LPostureState TPostureLoaderRead(string? text) => LPostureLoader.LPostureLoaderRead(text);

    internal static string TPostureLoaderFormat(LPostureState state) => LPostureLoader.LPostureLoaderFormat(state);

    internal static LPosture TPostureStart(this LEngine engine) => new(engine);

    internal static LPostureState TPostureRead(this LPosture posture) => posture.LPostureRead();

    internal static void TPostureWindowSave(this LPosture posture, LWindowState window)
    {
        posture.LPostureWindowSave(window);
    }

    internal static void TPostureVolumeSave(this LPosture posture, double volume)
    {
        posture.LPostureVolumeSave(volume);
    }

    internal static void TPostureModeSave(this LPosture posture, string mode)
    {
        posture.LPostureModeSave(mode);
    }

    internal static bool TPostureLinkedSave(this LPosture posture, bool linked) =>
        posture.LPostureLinkedSave(linked);

    internal static void TPostureLayoutSave(this LPosture posture, params LLayout[] layout)
    {
        posture.LPostureLayoutSave(layout);
    }

    internal static void TPostureLayoutReset(this LPosture posture)
    {
        posture.LPostureLayoutReset();
    }

    internal static void TEngineLeftSave(this LEngine engine, long? id)
    {
        engine.LEngineLeftSave(id);
    }

    internal static void TEngineRightSave(this LEngine engine, long? id)
    {
        engine.LEngineRightSave(id);
    }
}
