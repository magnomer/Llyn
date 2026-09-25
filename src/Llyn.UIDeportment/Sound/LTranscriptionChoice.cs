using System.ComponentModel;

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
