using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class PCohortItem : INotifyPropertyChanged
{
    private bool _pCohortItemChosen;

    internal PCohortItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pCohortItemChosen = chosen;
        PCohortItemId = id;
        PCohortItemHeadword = headword;
        PCohortItemEpithet = epithet ?? string.Empty;
        PCohortItemLanguage = language;
        PCohortItemFlag = LEnsignImage.LEnsignFind(language);
    }

    public long PCohortItemId { get; }

    public string PCohortItemHeadword { get; }

    public string PCohortItemEpithet { get; }

    public required string PCohortItemName { get; init; }

    public string PCohortItemLanguage { get; }

    public ImageSource? PCohortItemFlag { get; }

    internal static bool PCohortItemMatch(PCohortItem held, PCohortItem fresh)
    {
        return held.PCohortItemId == fresh.PCohortItemId
            && string.Equals(held.PCohortItemHeadword, fresh.PCohortItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PCohortItemEpithet, fresh.PCohortItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PCohortItemName, fresh.PCohortItemName, StringComparison.Ordinal)
            && string.Equals(held.PCohortItemLanguage, fresh.PCohortItemLanguage, StringComparison.Ordinal);
    }

    internal static void PCohortItemSync(PCohortItem held, PCohortItem fresh)
    {
        held.PCohortItemChosen = fresh.PCohortItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PCohortItemChosen
    {
        get => _pCohortItemChosen;

        set
        {
            if (_pCohortItemChosen == value)
            {
                return;
            }

            _pCohortItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PCohortItemChosen)));
        }
    }
}
