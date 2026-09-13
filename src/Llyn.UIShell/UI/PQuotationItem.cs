using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PQuotationItem : INotifyPropertyChanged
{
    private bool _pQuotationItemChosen;

    internal PQuotationItem(long id, string headword, string language)
    {
        PQuotationItemId = id;
        PQuotationItemHeadword = headword;
        PQuotationItemName = headword;
        PQuotationItemLanguage = language;
        PQuotationItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PQuotationItemId { get; }

    public string PQuotationItemHeadword { get; }

    public string PQuotationItemName { get; internal set; }

    public string PQuotationItemLanguage { get; }

    public ImageSource? PQuotationItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PQuotationItemChosen
    {
        get => _pQuotationItemChosen;

        set
        {
            if (_pQuotationItemChosen == value)
            {
                return;
            }

            _pQuotationItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PQuotationItemChosen)));
        }
    }
}
