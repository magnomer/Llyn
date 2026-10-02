namespace Llyn.Conduct;

public static class CLantern
{
    public static int? CLanternMove(int chosen, int count, int step)
    {
        if (count <= 0)
        {
            return null;
        }

        int start = chosen < 0 && step < 0 ? 0 : chosen;
        return ((start + step) % count + count) % count;
    }
}
