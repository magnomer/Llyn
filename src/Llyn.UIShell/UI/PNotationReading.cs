using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PNotationReading
{
    internal PNotationReading(string variety, string label, ImageSource? flag, string phonetic)
    {
        PNotationReadingVariety = variety;
        PNotationReadingLabel = label;
        PNotationReadingFlag = flag;
        PNotationReadingPhonetic = phonetic;
    }

    public string PNotationReadingVariety { get; }

    public string PNotationReadingLabel { get; }

    public ImageSource? PNotationReadingFlag { get; }

    public string PNotationReadingPhonetic { get; }
}
