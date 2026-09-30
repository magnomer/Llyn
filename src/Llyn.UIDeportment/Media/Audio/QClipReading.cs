using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QClipReading
{
    internal QClipReading(CClipReading reading, string label, ImageSource? flag, string action)
    {
        QClipReadingModel = reading.CClipReadingRecording;
        QClipReadingLabel = label;
        QClipReadingFlag = flag;
        QClipReadingAction = action;
        QClipReadingReady = reading.CClipReadingReady;
        QClipReadingFetching = reading.CClipReadingFetching;
        QClipReadingPlaying = reading.CClipReadingPlaying;
        QClipReadingRefused = reading.CClipReadingRefused;
    }

    public string QClipReadingLabel { get; }

    public ImageSource? QClipReadingFlag { get; }

    public string QClipReadingAction { get; }

    public bool QClipReadingReady { get; }

    public bool QClipReadingFetching { get; }

    public bool QClipReadingPlaying { get; }

    public bool QClipReadingRefused { get; }

    internal CRecording QClipReadingModel { get; }
}
