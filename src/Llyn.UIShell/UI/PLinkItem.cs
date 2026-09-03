using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PLinkItem
{
    internal PLinkItem(string id, string headword, string language, bool fresh)
    {
        PLinkItemId = id;
        PLinkItemHeadword = headword;
        PLinkItemLanguage = language;
        PLinkItemFlag = PEnsign.PEnsignFind(language);
        PLinkItemFresh = fresh;
    }

    public string PLinkItemId { get; }

    public string PLinkItemHeadword { get; }

    public string PLinkItemLanguage { get; }

    public ImageSource? PLinkItemFlag { get; }

    public bool PLinkItemFresh { get; }
}
