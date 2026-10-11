using System;
using System.Globalization;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CSLens
{
    private static readonly int[] CSLensLadder =
        [10, 25, 33, 50, 67, 75, 90, 100, 110, 125, 150, 175, 200, 250, 300, 400];

    private CSLensGrade _csLensGrade = new(CSLensMode.CSLensModeWidth, 1.0);

    private CSLensArea _csLensArea = new(0.0, 0.0);

    private CSLensPaper _csLensPaper = new(0.0, 0.0, 0);

    internal CSLens()
    {
    }

    public CSLensState CSLensRead()
    {
        return new CSLensState(
            _csLensGrade.CSLensGradeFactor,
            _csLensGrade.CSLensGradeMode,
            _csLensGrade.CSLensGradeFactor.ToString("P0", CultureInfo.CurrentCulture));
    }

    public CSLensState CSLensTextSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string digits = text.Replace(
            NumberFormatInfo.CurrentInfo.PercentSymbol, string.Empty, StringComparison.Ordinal);
        return int.TryParse(digits, NumberStyles.Integer, CultureInfo.CurrentCulture, out int zoom)
            ? LSLensFactorSet(CSLensMode.CSLensModeFree, zoom / 100.0)
            : LSLensFactorSet(_csLensGrade.CSLensGradeMode, LSLensFitRead(_csLensGrade.CSLensGradeMode));
    }

    public CSLensState CSLensLadderMove(int delta)
    {
        int percent = (int)Math.Round(_csLensGrade.CSLensGradeFactor * 100);
        int step = delta > 0
            ? CSLensLadder.FirstOrDefault(rung => rung > percent, CSLensLadder[^1])
            : CSLensLadder.LastOrDefault(rung => rung < percent, CSLensLadder[0]);
        return LSLensFactorSet(CSLensMode.CSLensModeFree, step / 100.0);
    }

    public CSLensState CSLensModeSet(CSLensMode mode)
    {
        return LSLensFactorSet(mode, LSLensFitRead(mode));
    }

    public CSLensState CSLensAreaSet(double width, double height)
    {
        _csLensArea = new CSLensArea(width, height);
        return LSLensFactorSet(_csLensGrade.CSLensGradeMode, LSLensFitRead(_csLensGrade.CSLensGradeMode));
    }

    internal CSLensState LSLensPaperSet(double width, double height, int resolution)
    {
        _csLensPaper = new CSLensPaper(width, height, resolution);
        return LSLensFactorSet(_csLensGrade.CSLensGradeMode, LSLensFitRead(_csLensGrade.CSLensGradeMode));
    }

    internal int? LSLensResolutionRead(double scale)
    {
        CSLensPaper paper = _csLensPaper;
        if (paper.CSLensPaperWidth <= 0.0)
        {
            return null;
        }

        int width = (int)Math.Round(
            Math.Min(
                paper.CSLensPaperWidth * _csLensGrade.CSLensGradeFactor,
                _csLensArea.CSLensAreaWidth * 2) * scale);
        double needed = width * 96.0 / paper.CSLensPaperWidth;
        return needed > paper.CSLensPaperResolution ? (int)Math.Ceiling(needed) : null;
    }

    private double LSLensFitRead(CSLensMode mode)
    {
        CSLensPaper paper = _csLensPaper;
        CSLensArea area = _csLensArea;
        if (mode != CSLensMode.CSLensModeFree
            && paper.CSLensPaperWidth > 0
            && paper.CSLensPaperHeight > 0
            && area.CSLensAreaWidth > 0
            && area.CSLensAreaHeight > 0)
        {
            double across = area.CSLensAreaWidth / paper.CSLensPaperWidth;
            return mode == CSLensMode.CSLensModeWidth
                ? across
                : Math.Min(across, area.CSLensAreaHeight / paper.CSLensPaperHeight);
        }

        return _csLensGrade.CSLensGradeFactor;
    }

    private CSLensState LSLensFactorSet(CSLensMode mode, double factor)
    {
        _csLensGrade = new CSLensGrade(
            mode,
            Math.Clamp(factor, CSLensLadder[0] / 100.0, CSLensLadder[^1] / 100.0));
        return CSLensRead();
    }

    private sealed record CSLensGrade(CSLensMode CSLensGradeMode, double CSLensGradeFactor);

    private sealed record CSLensArea(double CSLensAreaWidth, double CSLensAreaHeight);

    private sealed record CSLensPaper(double CSLensPaperWidth, double CSLensPaperHeight, int CSLensPaperResolution);
}
