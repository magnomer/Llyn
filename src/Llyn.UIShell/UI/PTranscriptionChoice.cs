using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed class PTranscriptionChoice : INotifyPropertyChanged
{
    private bool _pTranscriptionChoiceTaken;

    internal PTranscriptionChoice(string scheme, string label)
    {
        PTranscriptionChoiceScheme = scheme;
        PTranscriptionChoiceLabel = label;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PTranscriptionChoiceScheme { get; }

    public string PTranscriptionChoiceLabel { get; }

    public bool PTranscriptionChoiceTaken
    {
        get => _pTranscriptionChoiceTaken;
        set
        {
            if (_pTranscriptionChoiceTaken == value)
            {
                return;
            }

            _pTranscriptionChoiceTaken = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PTranscriptionChoiceTaken)));
        }
    }
}
