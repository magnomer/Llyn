using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class QTranscriptionChoice : INotifyPropertyChanged
{
    private bool _qTranscriptionChoiceTaken;

    public QTranscriptionChoice(string scheme, string label)
    {
        QTranscriptionChoiceScheme = scheme;
        QTranscriptionChoiceLabel = label;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string QTranscriptionChoiceScheme { get; }

    public string QTranscriptionChoiceLabel { get; }

    internal static void QTranscriptionChoiceRefine(FrameworkElement container, object item, string? _)
    {
        if (item is QTranscriptionChoice choice)
        {
            container.IsEnabled = !choice.QTranscriptionChoiceTaken;
        }
    }

    public bool QTranscriptionChoiceTaken
    {
        get => _qTranscriptionChoiceTaken;
        set
        {
            if (_qTranscriptionChoiceTaken == value)
            {
                return;
            }

            _qTranscriptionChoiceTaken = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QTranscriptionChoiceTaken)));
        }
    }
}
