namespace Llyn.Core;

public sealed record LDeckSchedule(
    LAlmanacOrder LDeckScheduleOrder = LAlmanacOrder.LAlmanacOrderSpaced,
    int LDeckScheduleGoal = 10,
    int LDeckScheduleSpaced = 20,
    int LDeckSchedulePool = 10,
    int LDeckScheduleStreak = 10,
    int LDeckScheduleTarget = 2160,
    int? LDeckScheduleQuorum = null)
{
    public const int LDeckScheduleFloor = 0;
}
