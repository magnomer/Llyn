using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

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

    internal static void PFanqieRowApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PFanqieLine line)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PFanqieRepresentative") is Button representative)
        {
            representative.Content = line.PFanqieLineOrder;
            representative.Tag = !line.PFanqieLineMarked ? null : line.PFanqieLinePrimary ? "Marked" : "Faded";
        }

        PFanqieWordApply(container, "PFanqieInitial", line.PFanqieLineInitial);
        PFanqieWordApply(container, "PFanqieRime", line.PFanqieLineYunmu);
        PFanqieTextApply(container, "PFanqieReading", line.PFanqieLineReading);
        PFanqieTextApply(container, "PFanqieLabel", line.PFanqieLineLabel);
        PFanqieTextApply(container, "PFanqieHeading", line.PFanqieLineHeading);
        PFanqieTextApply(container, "PFanqieKnot", line.PFanqieLineKnot);
        PFanqieTextApply(container, "PFanqieDivision", line.PFanqieLineDivision);
        PFanqieTextApply(container, "PFanqieTone", line.PFanqieLineTone);
        PFanqieTextApply(container, "PFanqieSpelling", line.PFanqieLineSpelling);
        PFanqieTextApply(container, "PFanqieText", line.PFanqieLineText);
        if (PLook.PLookPartFind<Border>(container, "PFanqieMedial") is Border medial)
        {
            medial.Visibility = PLook.PLookVisibleRead(line.PFanqieLineMedial.Length > 0);
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PFanqieMedialText") is not TextBlock text)
        {
            return;
        }

        text.Text = line.PFanqieLineMedial;
        if (line.PFanqieLineRounded)
        {
            text.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Warning");
            text.FontWeight = FontWeights.SemiBold;
            return;
        }

        text.ClearValue(TextBlock.ForegroundProperty);
        text.ClearValue(TextBlock.FontWeightProperty);
    }

    private static void PFanqieWordApply(FrameworkElement container, string name, string word)
    {
        if (PLook.PLookPartFind<Button>(container, name) is Button link)
        {
            link.Content = word;
        }
    }

    private static void PFanqieTextApply(FrameworkElement container, string name, string text)
    {
        if (PLook.PLookPartFind<TextBlock>(container, name) is TextBlock block)
        {
            block.Text = text;
        }
    }

    internal void PFanqieLineApply(bool raise, Action<long, int>? notice)
    {
        notice?.Invoke(PFanqieLineId, LFanqieRow.LFanqieRankResolve(PFanqieLineRank, raise));
    }
}
