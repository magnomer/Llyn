using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LCueMarkup
{
    public static string LCueMarkupFormat(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.Replace('|', '∣');
    }

    public static LCueSide LCueMarkupParse(LCueFace face, int side)
    {
        ArgumentNullException.ThrowIfNull(face);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(side);

        List<string> lines = [];
        List<string> pictures = [];
        List<string> sounds = [];
        foreach (LCueCell cell in face.LCueFaceCells)
        {
            if (cell.LCueCellIndex != side)
            {
                continue;
            }

            if (cell.LCueCellColumn == LCueColumn.LCueColumnText && cell.LCueCellValue.Length > 0)
            {
                lines.Add(cell.LCueCellValue.Replace('|', '\n'));
            }
            else if (cell.LCueCellColumn is LCueColumn.LCueColumnPicture or LCueColumn.LCueColumnSound)
            {
                List<string> files = cell.LCueCellColumn == LCueColumn.LCueColumnPicture ? pictures : sounds;
                files.AddRange(cell.LCueCellValue.Split(
                    '|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return new LCueSide(string.Join('\n', lines), pictures, sounds);
    }
}
