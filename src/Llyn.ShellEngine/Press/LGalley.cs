using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LGalley
{
    private readonly LGalleyClerk _lGalleyClerk;

    private readonly LPortraitClerkPress _lGalleyPortrait;

    private readonly LPressClerk _lGalleyPress;

    internal LGalley(LGalleyClerk clerk, LPortraitClerkPress portrait, LPressClerk press, string file)
    {
        ArgumentNullException.ThrowIfNull(clerk);
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(press);
        ArgumentNullException.ThrowIfNull(file);

        _lGalleyClerk = clerk;
        _lGalleyPortrait = portrait;
        _lGalleyPress = press;
        LGalleyPrinter = [];
        LGalleyFile = file;
        LGalleySetting = press.LPressClerkCreate(Array.Empty<LPressPrinter>());
    }

    public LPressSetting LGalleySetting { get; private set; }

    public IReadOnlyList<LPressPrinter> LGalleyPrinter { get; private set; }

    public string LGalleyRange { get; private set; } = string.Empty;

    public LPressScope LGalleyScope { get; private set; } = LPressScope.LPressScopeAll;

    public string LGalleyFile { get; }

    public bool LGalleyStale => !_lGalleyClerk.LGalleyClerkCheck(LGalleySetting);

    internal int? LGalleyCount => _lGalleyClerk.LGalleyClerkCount;

    public bool LGalleyMetric { get; } = RegionInfo.CurrentRegion.IsMetric;

    public bool LGalleyRangeValid =>
        _lGalleyPress.LPressClerkParse(LGalleyRange, LGalleyCount ?? int.MaxValue) is not null;

    internal LPressDigest LGalleyDigest =>
        _lGalleyPress.LPressClerkRead(LGalleySetting, LGalleyPrinter, LGalleyCount);

    public LPressMargin LGalleyMargin
    {
        get
        {
            LPressMargin margin = LGalleySetting.LPressSettingMargin;
            double unit = LGalleyMetric ? 25.4 : 1.0;
            return margin with
            {
                LPressMarginTop = margin.LPressMarginTop * unit,
                LPressMarginBottom = margin.LPressMarginBottom * unit,
                LPressMarginLeft = margin.LPressMarginLeft * unit,
                LPressMarginRight = margin.LPressMarginRight * unit,
            };
        }
    }

    public void LGalleySave()
    {
        _lGalleyPress.LPressClerkSave(LGalleySetting);
    }

    public async Task<LGalleyView> LGalleyRestore()
    {
        try
        {
            LGalleyPrinter = await _lGalleyPortrait.LPortraitPrinterRead();
        }
        finally
        {
            LGalleyRestore(_lGalleyPress.LPressClerkRestore(LGalleyPrinter));
        }

        return LGalleyView.LGalleyViewRead(this);
    }

    private void LGalleyRestore(LPressSetting? saved)
    {
        if (saved is null)
        {
            if (LGalleySetting == _lGalleyPress.LPressClerkCreate(Array.Empty<LPressPrinter>()))
            {
                LGalleySetting = _lGalleyPress.LPressClerkCreate(LGalleyPrinter);
            }

            return;
        }

        LGalleyScope = LPressScope.LPressScopeAll;
        LGalleyRange = string.Empty;
        LGalleySetting = saved;
    }

    public LPressSetting LGalleyPrinterSelect(string? name)
    {
        LGalleySetting = _lGalleyPress.LPressClerkSelect(LGalleySetting, LGalleyPrinter, name);
        return LGalleySetting;
    }
    public LPressSetting LGalleyCopiesSet(string text)
    {
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int copies))
        {
            return LGalleySetting;
        }

        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingCopies = copies }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyCollationSet(bool collation)
    {
        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingCollation = collation }, LGalleyPrinter);
        return LGalleySetting;
    }

    public bool LGalleyRangeSet(LPressScope scope, string text, int page)
    {
        ArgumentNullException.ThrowIfNull(text);

        LGalleyScope = scope;
        return scope switch
        {
            LPressScope.LPressScopeAll => LGalleyRangeSet(string.Empty),
            LPressScope.LPressScopeCurrent => LGalleyRangeSet(page),
            LPressScope.LPressScopeCustom => LGalleyRangeSet(text),
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null),
        };
    }

    public LPressSetting LGalleyPageSelect(int page)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(page);

        if (LGalleyScope == LPressScope.LPressScopeCurrent)
        {
            LGalleyRangeSet(page);
        }

        return LGalleySetting;
    }

    public int LGalleyPageSelect(string text, int page)
    {
        ArgumentNullException.ThrowIfNull(text);

        int count = LGalleyCount ?? 0;
        if (count == 0 || !int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int number))
        {
            return page;
        }

        int chosen = Math.Clamp(number, 1, count) - 1;
        LGalleyPageSelect(chosen);
        return chosen;
    }

    private bool LGalleyRangeSet(string text)
    {
        LGalleyRange = text;
        LPressRange? range = _lGalleyPress.LPressClerkParse(text, LGalleyCount ?? int.MaxValue);
        if (range is null)
        {
            return false;
        }

        LGalleySetting = LGalleySetting with { LPressSettingRange = range };
        return true;
    }

    private bool LGalleyRangeSet(int page)
    {
        return LGalleyRangeSet((page + 1).ToString(CultureInfo.InvariantCulture));
    }

    public LPressSetting LGalleyOrientationSet(LPressOrientation orientation)
    {
        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingOrientation = orientation }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyPaperSelect(int index)
    {
        IReadOnlyList<LPressPaper> papers = LGalleyDigest.LPressDigestPaper;
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, papers.Count);

        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingPaper = papers[index] }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyMarginSet(LPressMargin margin)
    {
        ArgumentNullException.ThrowIfNull(margin);

        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingMargin = margin }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyMarginSet(string? name)
    {
        return LGalleyMarginSet(_lGalleyPress.LPressClerkResolve(LGalleySetting.LPressSettingMargin, name));
    }

    public LPressSetting LGalleyMarginSet(string top, string bottom, string left, string right)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        return double.TryParse(top, NumberStyles.Float, culture, out double up)
            && double.TryParse(bottom, NumberStyles.Float, culture, out double down)
            && double.TryParse(left, NumberStyles.Float, culture, out double start)
            && double.TryParse(right, NumberStyles.Float, culture, out double end)
                ? LGalleyMarginSet(_lGalleyPress.LPressClerkResolve(up, down, start, end, LGalleyMetric))
                : LGalleySetting;
    }

    public LPressSetting LGalleyScaleSet(string text)
    {
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int scale))
        {
            return LGalleySetting;
        }

        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingScale = scale }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyColorSet(LPressColor color)
    {
        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingColor = color }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleySideSet(LPressSide side)
    {
        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingSide = side }, LGalleyPrinter);
        return LGalleySetting;
    }

    public LPressSetting LGalleyBackgroundSet(bool background)
    {
        LGalleySetting = _lGalleyPress.LPressClerkClamp(
            LGalleySetting with { LPressSettingBackground = background }, LGalleyPrinter);
        return LGalleySetting;
    }

    internal void LGalleyRangeSet()
    {
        if (LGalleyCount is int count && _lGalleyPress.LPressClerkParse(LGalleyRange, count) is { } range)
        {
            LGalleySetting = LGalleySetting with { LPressSettingRange = range };
        }
    }
}
