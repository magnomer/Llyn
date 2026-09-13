using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAnthologyItem : INotifyPropertyChanged
{
    private bool _pAnthologyItemChosen;

    internal PAnthologyItem(LExample example, int usage, string unknown, string unwritten)
    {
        PAnthologyItemId = example.LExampleId;
        PAnthologyItemText = PAnthologyTextRead(example.LExampleText, unknown) ?? unwritten;
        PAnthologyItemName = PAnthologyItemText;
        PAnthologyItemLanguage = example.LExampleLanguage;
        PAnthologyItemFlag = PEnsign.PEnsignFind(example.LExampleLanguage);
        PAnthologyItemCount = usage.ToString(CultureInfo.CurrentCulture);
    }

    public long PAnthologyItemId { get; }

    public string PAnthologyItemText { get; }

    public string PAnthologyItemName { get; internal set; }

    public string PAnthologyItemLanguage { get; }

    public ImageSource? PAnthologyItemFlag { get; }

    public string PAnthologyItemCount { get; }

    private static string? PAnthologyTextRead(LStateValue value, string unknown)
    {
        return value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => unknown,
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
