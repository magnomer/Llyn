using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QFanqieLine
{
    internal QFanqieLine(CFanqieRow row)
    {
        QFanqieLineId = row.CFanqieRowId;
        QFanqieLineRank = row.CFanqieRowRepresentative;
        QFanqieLineMarked = row.CFanqieRowMarked;
        QFanqieLinePrimary = row.CFanqieRowPrimary;
        QFanqieLineOrder = row.CFanqieRowOrder;
        QFanqieLineRounded = row.CFanqieRowClosed;
        QFanqieLineReading = row.CFanqieRowSlashed;
        QFanqieLineLabel = row.CFanqieRowLabel;
        QFanqieLineInitial = row.CFanqieRowInitial;
        QFanqieLineYunmu = row.CFanqieRowCell;
        QFanqieLineHeading = row.CFanqieRowBracketed;
        QFanqieLineKnot = row.CFanqieRowKnotted;
        QFanqieLineMedial = row.CFanqieRowMedial;
        QFanqieLineDivision = row.CFanqieRowGraded;
        QFanqieLineTone = row.CFanqieRowTone;
        QFanqieLineSpelling = row.CFanqieRowSpelling;
        QFanqieLineText = row.CFanqieRowRemainder;
    }

    public long QFanqieLineId { get; }

    public int QFanqieLineRank { get; }

    public bool QFanqieLineMarked { get; }

    public bool QFanqieLinePrimary { get; }

    public string QFanqieLineOrder { get; }

    public string QFanqieLineReading { get; }

    public string QFanqieLineLabel { get; }

    public string QFanqieLineInitial { get; }

    public string QFanqieLineYunmu { get; }

    public string QFanqieLineHeading { get; }

    public string QFanqieLineKnot { get; }

    public string QFanqieLineMedial { get; }

    public bool QFanqieLineRounded { get; }

    public string QFanqieLineDivision { get; }

    public string QFanqieLineTone { get; }

    public string QFanqieLineSpelling { get; }

    public string QFanqieLineText { get; }

    internal static void QFanqieLineRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QFanqieLine line)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PFanqieRepresentative") is Button representative)
        {
            representative.Content = line.QFanqieLineOrder;
            representative.SetValue(
                QLook.QLookCueProperty,
                !line.QFanqieLineMarked ? QLookCue.QLookCueBase
                : line.QFanqieLinePrimary ? QLookCue.QLookCueMarked
                : QLookCue.QLookCueFaded);
        }

        QFanqieWordRefine(container, "PFanqieInitial", line.QFanqieLineInitial);
        QFanqieWordRefine(container, "PFanqieRime", line.QFanqieLineYunmu);
        QFanqieTextRefine(container, "PFanqieReading", line.QFanqieLineReading);
        QFanqieTextRefine(container, "PFanqieLabel", line.QFanqieLineLabel);
        QFanqieTextRefine(container, "PFanqieHeading", line.QFanqieLineHeading);
        QFanqieTextRefine(container, "PFanqieKnot", line.QFanqieLineKnot);
        QFanqieTextRefine(container, "PFanqieDivision", line.QFanqieLineDivision);
        QFanqieTextRefine(container, "PFanqieTone", line.QFanqieLineTone);
        QFanqieTextRefine(container, "PFanqieSpelling", line.QFanqieLineSpelling);
        QFanqieTextRefine(container, "PFanqieText", line.QFanqieLineText);
        if (QLook.QLookPartFind<Border>(container, "PFanqieMedial") is Border medial)
        {
            medial.Visibility = QLook.QLookVisibleRead(line.QFanqieLineMedial.Length > 0);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFanqieMedialText") is not TextBlock text)
        {
            return;
        }

        text.Text = line.QFanqieLineMedial;
        if (line.QFanqieLineRounded)
        {
            text.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Warning");
            text.FontWeight = FontWeights.SemiBold;
            return;
        }

        text.ClearValue(TextBlock.ForegroundProperty);
        text.ClearValue(TextBlock.FontWeightProperty);
    }

    private static void QFanqieWordRefine(FrameworkElement container, string name, string word)
    {
        if (QLook.QLookPartFind<Button>(container, name) is Button link)
        {
            link.Content = word;
        }
    }

    private static void QFanqieTextRefine(FrameworkElement container, string name, string text)
    {
        if (QLook.QLookPartFind<TextBlock>(container, name) is TextBlock block)
        {
            block.Text = text;
        }
    }
}
