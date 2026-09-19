using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public sealed class PFanqieLine
{
    private const string PFanqieLineMark = "合";

    private PFanqieLine(LFanqieRow row)
    {
        PFanqieLineReading = row.LFanqieRowSlashed;
        PFanqieLineLabel = row.LFanqieRowLabel;
        PFanqieLineInitial = row.LFanqieRowInitial;
        PFanqieLineRime = row.LFanqieRowRime;
        PFanqieLineHeading = row.LFanqieRowBracketed;
        PFanqieLineYunmu = row.LFanqieRowCell;
        PFanqieLineMedial = row.LFanqieRowMedial;
        PFanqieLineDivision = row.LFanqieRowGraded;
        PFanqieLineTone = row.LFanqieRowTone;
        PFanqieLineSpelling = row.LFanqieRowSpelling;
        PFanqieLineText = row.LFanqieRowRemainder;
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

        return new PFanqieLine(row);
    }
}
