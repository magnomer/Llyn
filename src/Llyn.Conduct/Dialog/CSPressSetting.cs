using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSPressSetting(
    IReadOnlyList<CSPressPrinter> CSPressSettingPrinter,
    IReadOnlyList<CSPressPaper> CSPressSettingPaper,
    int? CSPressSettingCopies,
    bool? CSPressSettingCollation,
    IReadOnlyList<CSPressChoice<CSPressRange>> CSPressSettingRange,
    string? CSPressSettingText,
    string? CSPressSettingFailure,
    IReadOnlyList<bool> CSPressSettingIncluded,
    int CSPressSettingPage,
    CSPressOrientation CSPressSettingOrientation,
    IReadOnlyList<CSPressChoice<CSPressMargin>> CSPressSettingMargin,
    double? CSPressSettingTop,
    double? CSPressSettingBottom,
    double? CSPressSettingLeft,
    double? CSPressSettingRight,
    string CSPressSettingUnit,
    int CSPressSettingScale,
    CSPressColor? CSPressSettingColor,
    IReadOnlyList<CSPressChoice<CSPressSide>>? CSPressSettingSide,
    bool CSPressSettingBackground,
    string CSPressSettingAction,
    string? CSPressSettingHint,
    int? CSPressSettingSheets,
    string CSPressSettingTally,
    bool CSPressSettingStale,
    bool CSPressSettingIdle,
    string CSPressSettingBusy);
