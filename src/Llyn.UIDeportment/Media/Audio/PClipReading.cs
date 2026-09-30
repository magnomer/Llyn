using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PClipReading
{
    internal PClipReading(CClipReading reading, string label, ImageSource? flag, string action)
    {
        PClipReadingModel = reading.CClipReadingRecording;
        PClipReadingLabel = label;
        PClipReadingFlag = flag;
        PClipReadingAction = action;
        PClipReadingReady = reading.CClipReadingReady;
        PClipReadingFetching = reading.CClipReadingFetching;
        PClipReadingPlaying = reading.CClipReadingPlaying;
        PClipReadingRefused = reading.CClipReadingRefused;
    }

    public string PClipReadingLabel { get; }

    public ImageSource? PClipReadingFlag { get; }

    public string PClipReadingAction { get; }

    public bool PClipReadingReady { get; }

    public bool PClipReadingFetching { get; }

    public bool PClipReadingPlaying { get; }

    public bool PClipReadingRefused { get; }

    internal CRecording PClipReadingModel { get; }
}
