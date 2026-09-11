using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAnthologyItem : INotifyPropertyChanged
{
    private bool _pAnthologyItemChosen;

    internal PAnthologyItem(LExample example, int usage, string source, string unreadable, string unwritten)
    {
        PAnthologyItemId = example.LExampleId;
        PAnthologyItemText = PAnthologyTextRead(example.LExampleText, unreadable) ?? unwritten;
        PAnthologyItemName = PAnthologyItemText;
        PAnthologyItemLanguage = example.LExampleLanguage;
        PAnthologyItemFlag = PEnsign.PEnsignFind(example.LExampleLanguage);
        PAnthologyItemSource = source;
        PAnthologyItemCount = usage.ToString(CultureInfo.CurrentCulture);
    }

    public long PAnthologyItemId { get; }

    public string PAnthologyItemText { get; }

    public string PAnthologyItemName { get; internal set; }

    public string PAnthologyItemLanguage { get; }

    public ImageSource? PAnthologyItemFlag { get; }

    public string PAnthologyItemSource { get; }

    public string PAnthologyItemCount { get; }

    private static string? PAnthologyTextRead(LStateValue value, string unreadable)
    {
        return value.LStateValueState switch
        {
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => null,
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PAnthologyItemChosen
    {
        get => _pAnthologyItemChosen;

        set
        {
            if (_pAnthologyItemChosen == value)
            {
                return;
            }

            _pAnthologyItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAnthologyItemChosen)));
        }
    }
}
