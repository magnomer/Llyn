using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PAnthologyItem : INotifyPropertyChanged
{
    private bool _pAnthologyItemChosen;

    internal PAnthologyItem(LExample example, int usage, string unknown, string unwritten, bool chosen)
    {
        _pAnthologyItemChosen = chosen;
        PAnthologyItemId = example.LExampleId;
        PAnthologyItemText = PAnthologyTextRead(example.LExampleText, unknown) ?? unwritten;
        PAnthologyItemLanguage = example.LExampleLanguage;
        PAnthologyItemFlag = LEnsignImage.LEnsignFind(example.LExampleLanguage);
        PAnthologyItemCount = usage.ToString(CultureInfo.CurrentCulture);
    }

    public long PAnthologyItemId { get; }

    public string PAnthologyItemText { get; }

    public required string PAnthologyItemName { get; init; }

    public string PAnthologyItemLanguage { get; }

    public ImageSource? PAnthologyItemFlag { get; }

    public string PAnthologyItemCount { get; }

    private static string? PAnthologyTextRead(LStateValue value, string unknown)
    {
        return value.LStateValueUncertain ? unknown : value.LStateValueShown;
    }

    internal static bool PAnthologyItemMatch(PAnthologyItem held, PAnthologyItem fresh)
    {
        return held.PAnthologyItemId == fresh.PAnthologyItemId
            && string.Equals(held.PAnthologyItemText, fresh.PAnthologyItemText, StringComparison.Ordinal)
            && string.Equals(held.PAnthologyItemName, fresh.PAnthologyItemName, StringComparison.Ordinal)
            && string.Equals(held.PAnthologyItemLanguage, fresh.PAnthologyItemLanguage, StringComparison.Ordinal)
            && string.Equals(held.PAnthologyItemCount, fresh.PAnthologyItemCount, StringComparison.Ordinal);
    }

    internal static void PAnthologyItemSync(PAnthologyItem held, PAnthologyItem fresh)
    {
        held.PAnthologyItemChosen = fresh.PAnthologyItemChosen;
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
