using System;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PFanqieLine
{
    private const string PFanqieLineUnrounded = "開";

    private const string PFanqieLineMark = "合";

    private const string PFanqieLineSuffix = "等";

    private const string PFanqieLineKey = "Display.FanqieTone";

    private PFanqieLine(
        string reading,
        string label,
        string initial,
        string rime,
        string heading,
        string yunmu,
        string medial,
        string division,
        string tone,
        string spelling,
        string text)
    {
        PFanqieLineReading = reading;
        PFanqieLineLabel = label;
        PFanqieLineInitial = initial;
        PFanqieLineRime = rime;
        PFanqieLineHeading = heading;
        PFanqieLineYunmu = yunmu;
        PFanqieLineMedial = medial;
        PFanqieLineDivision = division;
        PFanqieLineTone = tone;
        PFanqieLineSpelling = spelling;
        PFanqieLineText = text;
    }

    public string PFanqieLineReading { get; }

    public string PFanqieLineLabel { get; }

    public string PFanqieLineInitial { get; }

    public string PFanqieLineRime { get; }

    public string PFanqieLineHeading { get; }

    public string PFanqieLineYunmu { get; }

    public string PFanqieLineMedial { get; }

    public bool PFanqieLineRounded => PFanqieLineMedial == PFanqieLineMark;

    public string PFanqieLineDivision { get; }

    public string PFanqieLineTone { get; }

    public string PFanqieLineSpelling { get; }

    public string PFanqieLineText { get; }

    internal static PFanqieLine PFanqieLineCreate(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        bool parted = row.LFanqieRowInitial.Length > 0 || row.LFanqieRowRime.Length > 0;
        string reading = row.LFanqieRowReading.Length == 0 ? string.Empty : '/' + row.LFanqieRowReading + '/';
        string label = PFanqieLabelFormat(row.LFanqieRowClass);
        string heading = row.LFanqieRowHeading.Length == 0 ? string.Empty : '[' + row.LFanqieRowHeading + ']';
        string medial = !parted ? string.Empty : row.LFanqieRowRounded ? PFanqieLineMark : PFanqieLineUnrounded;
        string division = row.LFanqieRowDivision.Length == 0
            ? string.Empty
            : row.LFanqieRowDivision + PFanqieLineSuffix;

        return new PFanqieLine(
            reading,
            label,
            row.LFanqieRowInitial,
            row.LFanqieRowRime,
            heading,
            LDiwei.LDiweiRimeFormat(row.LFanqieRowRime, row.LFanqieRowDivision, row.LFanqieRowRounded),
            medial,
            division,
            row.LFanqieRowTone,
            row.LFanqieRowSpelling,
            parted ? string.Empty : row.LFanqieRowText);
    }

    private static string PFanqieLabelFormat(string toneClass)
    {
        if (toneClass.Length == 0)
        {
            return string.Empty;
        }

        string pattern = PLocalizationCatalog.PLocalizationCatalogCurrent[PFanqieLineKey];
        return pattern.Length == 0
            ? toneClass
            : string.Format(CultureInfo.CurrentCulture, pattern, toneClass);
    }
}
