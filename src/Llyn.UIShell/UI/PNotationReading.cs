using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PNotationReading
{
    internal PNotationReading(
        string variety, string label, ImageSource? flag, string phonetic, string text, string opener, string closer)
    {
        PNotationReadingVariety = variety;
        PNotationReadingLabel = label;
        PNotationReadingFlag = flag;
        PNotationReadingPhonetic = phonetic;
        PNotationReadingText = text;
        PNotationReadingOpener = opener;
        PNotationReadingCloser = closer;
    }

    public string PNotationReadingVariety { get; }

    public string PNotationReadingLabel { get; }

    public ImageSource? PNotationReadingFlag { get; }

    public string PNotationReadingPhonetic { get; }

    public string PNotationReadingText { get; }

    public string PNotationReadingOpener { get; }

    public string PNotationReadingCloser { get; }
}
