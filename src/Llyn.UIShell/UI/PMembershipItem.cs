using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PMembershipItem
{
    internal PMembershipItem(string id, string headword, string language)
    {
        PMembershipItemId = id;
        PMembershipItemHeadword = headword;
        PMembershipItemLanguage = language;
        PMembershipItemFlag = PLangcodeIndicator.PLangcodeIndicatorFind(language);
    }

    public string PMembershipItemId { get; }

    public string PMembershipItemHeadword { get; }

    public string PMembershipItemLanguage { get; }

    public ImageSource? PMembershipItemFlag { get; }
}
