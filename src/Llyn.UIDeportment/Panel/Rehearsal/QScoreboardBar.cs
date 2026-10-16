namespace Llyn.UIDeportment;

internal sealed class QScoreboardBar
{
    internal QScoreboardBar(string monday, string tally, int added, int peak)
    {
        QScoreboardBarMonday = monday;
        QScoreboardBarTally = tally;
        QScoreboardBarAdded = added;
        QScoreboardBarPeak = peak;
    }

    internal string QScoreboardBarMonday { get; }

    internal string QScoreboardBarTally { get; }

    internal int QScoreboardBarAdded { get; }

    internal int QScoreboardBarPeak { get; }
}
