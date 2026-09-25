using System;
using Llyn.Core;

namespace Llyn.UIVeneer;

public sealed class PFanqieLine
{
    private PFanqieLine(LFanqieRow row)
    {
        PFanqieLineId = row.LFanqieRowId;
        PFanqieLineRank = row.LFanqieRowRepresentative;
        PFanqieLineMarked = row.LFanqieRowMarked;
        PFanqieLinePrimary = row.LFanqieRowPrimary;
        PFanqieLineOrder = row.LFanqieRowOrder;
        PFanqieLineRounded = row.LFanqieRowClosed;
        PFanqieLineReading = row.LFanqieRowSlashed;
        PFanqieLineLabel = row.LFanqieRowLabel;
        PFanqieLineInitial = row.LFanqieRowInitial;
        PFanqieLineYunmu = row.LFanqieRowCell;
        PFanqieLineHeading = row.LFanqieRowBracketed;
        PFanqieLineKnot = row.LFanqieRowKnotted;
        PFanqieLineMedial = row.LFanqieRowMedial;
        PFanqieLineDivision = row.LFanqieRowGraded;
        PFanqieLineTone = row.LFanqieRowTone;
        PFanqieLineSpelling = row.LFanqieRowSpelling;
        PFanqieLineText = row.LFanqieRowRemainder;
    }

    public long PFanqieLineId { get; }

    public int PFanqieLineRank { get; }

    public bool PFanqieLineMarked { get; }

    public bool PFanqieLinePrimary { get; }

    public string PFanqieLineOrder { get; }

    public string PFanqieLineReading { get; }

    public string PFanqieLineLabel { get; }

    public string PFanqieLineInitial { get; }

    public string PFanqieLineYunmu { get; }

    public string PFanqieLineHeading { get; }

    public string PFanqieLineKnot { get; }

    public string PFanqieLineMedial { get; }

    public bool PFanqieLineRounded { get; }

    public string PFanqieLineDivision { get; }

    public string PFanqieLineTone { get; }

    public string PFanqieLineSpelling { get; }

    public string PFanqieLineText { get; }

    internal static PFanqieLine PFanqieLineCreate(LFanqieRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new PFanqieLine(row);
    }

    internal void PFanqieLineApply(bool raise, Action<long, int>? notice)
    {
        notice?.Invoke(PFanqieLineId, LFanqieRow.LFanqieRankResolve(PFanqieLineRank, raise));
    }
}
