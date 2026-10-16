namespace Llyn.UIDeportment;

internal sealed class QStudyItem
{
    internal QStudyItem(long id, string text, string tally, bool folder, bool chosen)
    {
        QStudyItemId = id;
        QStudyItemText = text;
        QStudyItemTally = tally;
        QStudyItemFolder = folder;
        QStudyItemChosen = chosen;
    }

    internal long QStudyItemId { get; }

    public string QStudyItemText { get; }

    public string QStudyItemTally { get; }

    internal bool QStudyItemFolder { get; }

    public bool QStudyItemChosen { get; }
}
