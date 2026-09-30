using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PFanqieLine
{
    internal PFanqieLine(CFanqieRow row)
    {
        PFanqieLineId = row.CFanqieRowId;
        PFanqieLineRank = row.CFanqieRowRepresentative;
        PFanqieLineMarked = row.CFanqieRowMarked;
        PFanqieLinePrimary = row.CFanqieRowPrimary;
        PFanqieLineOrder = row.CFanqieRowOrder;
        PFanqieLineRounded = row.CFanqieRowClosed;
        PFanqieLineReading = row.CFanqieRowSlashed;
        PFanqieLineLabel = row.CFanqieRowLabel;
        PFanqieLineInitial = row.CFanqieRowInitial;
        PFanqieLineYunmu = row.CFanqieRowCell;
        PFanqieLineHeading = row.CFanqieRowBracketed;
        PFanqieLineKnot = row.CFanqieRowKnotted;
        PFanqieLineMedial = row.CFanqieRowMedial;
        PFanqieLineDivision = row.CFanqieRowGraded;
        PFanqieLineTone = row.CFanqieRowTone;
        PFanqieLineSpelling = row.CFanqieRowSpelling;
        PFanqieLineText = row.CFanqieRowRemainder;
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

    internal static void PFanqieRowRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PFanqieLine line)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PFanqieRepresentative") is Button representative)
        {
            representative.Content = line.PFanqieLineOrder;
            representative.SetValue(
                QLook.QLookCueProperty,
                !line.PFanqieLineMarked ? QLookCue.QLookCueBase
                : line.PFanqieLinePrimary ? QLookCue.QLookCueMarked
                : QLookCue.QLookCueFaded);
        }

        PFanqieWordRefine(container, "PFanqieInitial", line.PFanqieLineInitial);
        PFanqieWordRefine(container, "PFanqieRime", line.PFanqieLineYunmu);
        PFanqieTextRefine(container, "PFanqieReading", line.PFanqieLineReading);
        PFanqieTextRefine(container, "PFanqieLabel", line.PFanqieLineLabel);
        PFanqieTextRefine(container, "PFanqieHeading", line.PFanqieLineHeading);
        PFanqieTextRefine(container, "PFanqieKnot", line.PFanqieLineKnot);
        PFanqieTextRefine(container, "PFanqieDivision", line.PFanqieLineDivision);
        PFanqieTextRefine(container, "PFanqieTone", line.PFanqieLineTone);
        PFanqieTextRefine(container, "PFanqieSpelling", line.PFanqieLineSpelling);
        PFanqieTextRefine(container, "PFanqieText", line.PFanqieLineText);
        if (QLook.QLookPartFind<Border>(container, "PFanqieMedial") is Border medial)
        {
            medial.Visibility = QLook.QLookVisibleRead(line.PFanqieLineMedial.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieMedialText") is not TextBlock text)
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

    private static void PFanqieWordRefine(FrameworkElement container, string name, string word)
    {
        if (QLook.QLookPartFind<Button>(container, name) is Button link)
        {
            link.Content = word;
        }
    }

    private static void PFanqieTextRefine(FrameworkElement container, string name, string text)
    {
        if (QLook.QLookPartFind<TextBlock>(container, name) is TextBlock block)
        {
            block.Text = text;
        }
    }
}
