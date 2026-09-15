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

    public string PFanqieLineYunmu { get; }

    public string PFanqieLineMedial { get; }

    public bool PFanqieLineRounded => PFanqieLineMedial == PFanqieLineMark;

    public string PFanqieLineDivision { get; }

    public string PFanqieLineTone { get; }

    public string PFanqieLineSpelling { get; }

    public string PFanqieLineText { get; }

    internal static PFanqieLine PFanqieLineCreate(LFanqieRow row, LHypothesis? hypothesis)
    {
        ArgumentNullException.ThrowIfNull(row);

        bool parted = row.LFanqieRowInitial.Length > 0 || row.LFanqieRowRime.Length > 0;
        LHypothesisSound? sound = hypothesis?.LHypothesisResolve(row);
        string reading = sound is null ? string.Empty : '/' + sound.LHypothesisSoundText + '/';
        string label = PFanqieLabelFormat(sound);
        string rime = row.LFanqieRowHeading.Length == 0
            ? row.LFanqieRowRime
            : row.LFanqieRowRime + '[' + row.LFanqieRowHeading + ']';
        string medial = !parted ? string.Empty : row.LFanqieRowRounded ? PFanqieLineMark : PFanqieLineUnrounded;
        string division = row.LFanqieRowDivision.Length == 0
            ? string.Empty
            : row.LFanqieRowDivision + PFanqieLineSuffix;

        return new PFanqieLine(
            reading,
            label,
            row.LFanqieRowInitial,
            rime,
            LDiwei.LDiweiRimeNormalize(row.LFanqieRowRime),
            medial,
            division,
            row.LFanqieRowTone,
            row.LFanqieRowSpelling,
            parted ? string.Empty : row.LFanqieRowText);
    }

    private static string PFanqieLabelFormat(LHypothesisSound? sound)
    {
        if (sound is null || sound.LHypothesisSoundClass.Length == 0)
        {
            return string.Empty;
        }

        string pattern = PLocalizationCatalog.PLocalizationCatalogCurrent[PFanqieLineKey];
        return pattern.Length == 0
            ? sound.LHypothesisSoundClass
            : string.Format(CultureInfo.CurrentCulture, pattern, sound.LHypothesisSoundClass);
    }
}
