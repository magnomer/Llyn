namespace Llyn.UIDeportment;

internal sealed class QLeafSee
{
    internal QLeafSee(long id, string text)
    {
        QLeafSeeId = id;
        QLeafSeeText = text;
    }

    internal long QLeafSeeId { get; }

    internal string QLeafSeeText { get; }
}
