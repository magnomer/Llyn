namespace Llyn.UIDeportment;

internal sealed class QBearingItem
{
    internal QBearingItem(int direction, int front, int back, int statistics, string caption)
    {
        QBearingItemDirection = direction;
        QBearingItemFront = front;
        QBearingItemBack = back;
        QBearingItemStatistics = statistics;
        QBearingItemCaption = caption;
    }

    internal int QBearingItemDirection { get; }

    internal int QBearingItemFront { get; }

    internal int QBearingItemBack { get; }

    internal int QBearingItemStatistics { get; }

    internal string QBearingItemCaption { get; }
}
