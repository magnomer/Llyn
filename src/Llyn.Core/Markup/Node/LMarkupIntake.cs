namespace Llyn.Core;

public sealed record LMarkupIntake(
    int LMarkupIntakeIndex,
    LMarkupMode LMarkupIntakeMode,
    long LMarkupIntakeTarget)
{
    public static LMarkupIntake LMarkupIntakeCreate(int index, LMarkupMode mode, long target)
    {
        return new LMarkupIntake(index, mode, mode == LMarkupMode.LMarkupModeNew ? 0 : target);
    }
}
