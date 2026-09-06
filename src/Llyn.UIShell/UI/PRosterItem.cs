using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PRosterItem
{
    internal PRosterItem(string id, string headword, string language)
    {
        PRosterItemId = id;
        PRosterItemHeadword = headword;
        PRosterItemLanguage = language;
        PRosterItemFlag = PEnsign.PEnsignFind(language);
    }

    public string PRosterItemId { get; }

    public string PRosterItemHeadword { get; }

    public string PRosterItemLanguage { get; }

    public ImageSource? PRosterItemFlag { get; }
}
