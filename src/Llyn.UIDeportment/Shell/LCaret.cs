using System;

namespace Llyn.UIDeportment;

public static class LCaret
{
    public static bool LCaretKeyApply(
        string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(remove);
        ArgumentNullException.ThrowIfNull(move);
        ArgumentNullException.ThrowIfNull(place);

        if (selection != 0)
        {
            return false;
        }

        if (key == "Back" && caret == 0)
        {
            remove(-1);
            return true;
        }

        if (key == "Delete" && caret == length)
        {
            remove(1);
            return true;
        }

        int step = key == "Left" ? -1 : key == "Right" ? 1 : 0;
        if (step == 0 || length > 0 || !move(step))
        {
            return false;
        }

        place();
        return true;
    }
}
