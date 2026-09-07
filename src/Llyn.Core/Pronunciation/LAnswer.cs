namespace Llyn.Core;

public sealed record LAnswer(string? LAnswerValue, bool LAnswerReached)
{
    public static LAnswer LAnswerLost { get; } = new(null, false);

    public static LAnswer LAnswerBlank { get; } = new(null, true);

    public static LAnswer LAnswerCreate(string value)
    {
        return new LAnswer(value, true);
    }

    public bool LAnswerEmpty => string.IsNullOrEmpty(LAnswerValue);
}
