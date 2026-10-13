using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TInterfaceConductDialog
{
    internal static CSCustoms TCustomsCreate(
        IReadOnlyList<CMarkupEntry> entries, IReadOnlyList<CMarkupTarget> targets) => new(entries, targets);

    internal static IReadOnlyList<CSCustomsRow> TCustomsRowsRead(this CSCustoms customs) =>
        customs.LSCustomsRowsRead();

    internal static CSCustomsRow TCustomsRowRead(this CSCustoms customs, int row) => customs.CSCustomsRowRead(row);

    internal static bool TCustomsModeSet(this CSCustoms customs, int row, CSCustomsMode mode) =>
        customs.CSCustomsModeSet(row, mode);

    internal static bool TCustomsTargetSet(this CSCustoms customs, int row, long target) =>
        customs.CSCustomsTargetSet(row, target);

    internal static bool TCustomsReadyCheck(this CSCustoms customs) => customs.CSCustomsReadyCheck();

    internal static bool TCoinageWordingCheck(string? wording) => CSCoinage.CSCoinageWordingCheck(wording);

    internal static CSPressSetting TPressSettingRead(this CSPress press) => press.CSPressSettingRead();

    internal static Task<CSPressSetting> TPressRestore(this CSPress press) => press.CSPressRestore();

    internal static CSPressSetting TPressPrinterSelect(this CSPress press, string? name) =>
        press.CSPressPrinterSelect(press.CSPressSettingRead().CSPressSettingPrinter
            .Select(static printer => printer.CSPressPrinterName)
            .ToList()
            .IndexOf(name));

    internal static CSPressSetting TPressPrinterSelect(this CSPress press, int index) =>
        press.CSPressPrinterSelect(index);

    internal static CSPressSetting TPressCopiesSet(this CSPress press, string text) => press.CSPressCopiesSet(text);

    internal static CSPressSetting TPressCollationSet(this CSPress press, bool collation) =>
        press.CSPressCollationSet(collation);

    internal static CSPressSetting TPressRangeSet(this CSPress press, CSPressRange range, string text) =>
        press.CSPressRangeSet(range, text);

    internal static CSPressSetting TPressRangeSet(this CSPress press, string text) => press.CSPressRangeSet(text);

    internal static CSPressSetting TPressRangeSelect(this CSPress press, int index) => press.CSPressRangeSelect(index);

    internal static CSPressSetting TPressPageSelect(this CSPress press, int page)
    {
        press.LSPressPageSelect(page);
        return press.CSPressSettingRead();
    }

    internal static CSPressSetting TPressPageSelect(this CSPress press, string text) => press.CSPressPageSelect(text);

    internal static CSPressSetting TPressOrientationSet(this CSPress press, CSPressOrientation orientation) =>
        press.CSPressOrientationSet(orientation);

    internal static CSPressSetting TPressPaperSelect(this CSPress press, int index) => press.CSPressPaperSelect(index);

    internal static CSPressSetting TPressMarginSet(this CSPress press, CSPressMargin margin) =>
        press.CSPressMarginSet(margin);

    internal static CSPressSetting TPressMarginSet(
        this CSPress press, string top, string bottom, string left, string right) =>
        press.CSPressMarginSet(top, bottom, left, right);

    internal static CSPressSetting TPressMarginSelect(this CSPress press, int index) =>
        press.CSPressMarginSelect(index);

    internal static CSPressSetting TPressTopSet(this CSPress press, string text) => press.CSPressTopSet(text);

    internal static CSPressSetting TPressBottomSet(this CSPress press, string text) => press.CSPressBottomSet(text);

    internal static CSPressSetting TPressLeftSet(this CSPress press, string text) => press.CSPressLeftSet(text);

    internal static CSPressSetting TPressRightSet(this CSPress press, string text) => press.CSPressRightSet(text);

    internal static CSPressSetting TPressScaleSet(this CSPress press, string text) => press.CSPressScaleSet(text);

    internal static CSPressSetting TPressColorSet(this CSPress press, CSPressColor color) =>
        press.CSPressColorSet(color);

    internal static CSPressSetting TPressSideSet(this CSPress press, CSPressSide side) => press.CSPressSideSet(side);

    internal static CSPressSetting TPressSideSelect(this CSPress press, int index) => press.CSPressSideSelect(index);

    internal static CSPressSetting TPressBackgroundSet(this CSPress press, bool background) =>
        press.CSPressBackgroundSet(background);

    internal static Task<CSPressState?> TPressDraw(this CSPress press) => press.CSPressGalley.CSGalleyDraw();

    internal static Task TPressSightSet(this CSPress press, int page, int first, int last, double scale) =>
        press.CSPressGalley.CSGalleySightSet(page, first, last, scale);

    internal static CSLensState TLensTextSet(this CSPress press, string text) =>
        press.CSPressGalley.CSGalleyLens.CSLensTextSet(text);

    internal static CSLensState TLensLadderMove(this CSPress press, int delta) =>
        press.CSPressGalley.CSGalleyLens.CSLensLadderMove(delta);

    internal static CSLensState TLensModeSet(this CSPress press, CSLensMode mode) =>
        press.CSPressGalley.CSGalleyLens.CSLensModeSet(mode);

    internal static CSLensState TLensAreaSet(this CSPress press, double width, double height) =>
        press.CSPressGalley.CSGalleyLens.CSLensAreaSet(width, height);

    internal static void TPressDelaySet(this CSPress press, int delay)
    {
        press.CSPressGalley.CSGalleyEditDelay.CSGalleyDelayTime = delay;
        press.CSPressGalley.CSGalleySightDelay.CSGalleyDelayTime = delay;
    }

    internal static void TPressGalleyDispose(this CSPress press) => press.CSPressGalley.Dispose();

    internal static Task<string?> TPressSystemShow(this CSPress press) => press.CSPressSystemShow();

    internal static Task<string?> TPressPrint(this CSPress press) => press.CSPressPrint();

    internal static bool TPressClose(this CSPress press) => press.CSPressClose(false);

    internal static void TPressSettingAttach(this CSPress press, Action<CSPressSetting> watch) =>
        press.CSPressSettingChanged += watch;

    internal static void TPressStateAttach(this CSPress press, Action<CSPressState> watch) =>
        press.CSPressGalley.CSGalleyStateChanged += watch;

    internal static void TPressPageAttach(this CSPress press, Action<CSPressPage> watch) =>
        press.CSPressGalley.CSGalleyPageChanged += page =>
        {
            if (page.CSPressPagePng is not null)
            {
                watch(page);
            }
        };

    internal static void TPressDropAttach(this CSPress press, Action<int> watch) =>
        press.CSPressGalley.CSGalleyPageChanged += page =>
        {
            if (page.CSPressPagePng is null)
            {
                watch(page.CSPressPageIndex);
            }
        };
}
