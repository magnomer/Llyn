using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LAnswer(IReadOnlyList<LReading> LAnswerReadings, bool LAnswerReached)
{
    public static LAnswer LAnswerLost { get; } = new([], false);

    public static LAnswer LAnswerBlank { get; } = new([], true);

    public static LAnswer LAnswerCreate(IReadOnlyList<LReading> readings)
    {
        return new LAnswer(readings, true);
    }

    public static LAnswer LAnswerCreate(string value)
    {
        return new LAnswer([new LReading(string.Empty, value)], true);
    }

    public string? LAnswerValue => LAnswerReadings.FirstOrDefault()?.LReadingPhonetic;

    public bool LAnswerEmpty => LAnswerReadings.Count == 0;
}
