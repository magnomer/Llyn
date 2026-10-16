namespace Llyn.UIDeportment;

internal sealed class QScoreboardItem
{
    internal QScoreboardItem(long? id, string text, bool folder, bool chosen)
    {
        QScoreboardItemId = id;
        QScoreboardItemText = text;
        QScoreboardItemFolder = folder;
        QScoreboardItemChosen = chosen;
    }

    internal long? QScoreboardItemId { get; }

    internal string QScoreboardItemText { get; }

    internal bool QScoreboardItemFolder { get; }

    internal bool QScoreboardItemChosen { get; }
}
