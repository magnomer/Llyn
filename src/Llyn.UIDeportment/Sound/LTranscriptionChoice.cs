using System.ComponentModel;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class LTranscriptionChoice : INotifyPropertyChanged
{
    private bool _lTranscriptionChoiceTaken;

    public LTranscriptionChoice(string scheme, string label)
    {
        LTranscriptionChoiceScheme = scheme;
        LTranscriptionChoiceLabel = label;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string LTranscriptionChoiceScheme { get; }

    public string LTranscriptionChoiceLabel { get; }

    internal static void LTranscriptionChoiceApply(FrameworkElement container, object item, string? _)
    {
        if (item is LTranscriptionChoice choice)
        {
            container.IsEnabled = !choice.LTranscriptionChoiceTaken;
        }
    }

    public bool LTranscriptionChoiceTaken
    {
        get => _lTranscriptionChoiceTaken;
        set
        {
            if (_lTranscriptionChoiceTaken == value)
            {
                return;
            }

            _lTranscriptionChoiceTaken = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LTranscriptionChoiceTaken)));
        }
    }
}
