using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPressQueue : LPressQueue
{
    internal Exception? TPressQueueFailure { get; set; }

    public Task<IReadOnlyList<LPressPrinter>> LPressQueueRead()
    {
        if (TPressQueueFailure is not null)
        {
            return Task.FromException<IReadOnlyList<LPressPrinter>>(TPressQueueFailure);
        }

        return Task.FromResult<IReadOnlyList<LPressPrinter>>(
        [
            new LPressPrinter(
                "Office Laser",
                true,
                [LPressPaper.LPressPaperMetric, LPressPaper.LPressPaperLetter],
                [LPressSide.LPressSideSingle, LPressSide.LPressSideLong],
                [LPressColor.LPressColorFull, LPressColor.LPressColorGray],
                true,
                5,
                true),
            new LPressPrinter("Plotter", false, [], [], [], false, null, false),
        ]);
    }
}
