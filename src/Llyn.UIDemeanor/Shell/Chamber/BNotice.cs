using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal sealed class BNotice
{
    private readonly BBlueprintPart _bNoticeBlueprint;
    private readonly BBlueprintPart _bNoticeText;

    internal BNotice(BBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(blueprint);

        _bNoticeBlueprint = BContract.BContractBlueprintFind(blueprint, "ANotice");
        _bNoticeText = BContract.BContractFind(_bNoticeBlueprint, "ANoticeText");
    }

    internal IEnumerable<BCanvasText> BNoticeBuild(
        string notice,
        int width,
        int height,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs,
        IEnumerable<BCanvasText> legend)
    {
        const int noticeWidth = 44;
        const int noticeHeight = 8;
        BCanvasWidth measure = new(false);
        if (width < noticeWidth || height < noticeHeight)
        {
            IReadOnlyList<string> bare = measure.BCanvasWidthDivide(notice, width);
            for (int row = 0; row < bare.Count; row++)
            {
                yield return new BCanvasText(row, 0, width, bare[row], "text");
            }

            yield break;
        }

        BBlueprintPart text = _bNoticeText;
        string theme = text.BBlueprintPartTheme;
        int across = noticeWidth - 2;
        BBlueprint.BBlueprintPlace(_bNoticeBlueprint, 0, 0, across, height - 1);
        IReadOnlyList<string> lines = measure.BCanvasWidthDivide(notice, text.BBlueprintPartWidth);
        int down = Math.Min(lines.Count + (height - 1 - text.BBlueprintPartHeight), height - 1);
        (int top, int left) = ((height - 1 - down) / 2, (width - across) / 2);
        BBlueprint.BBlueprintPlace(_bNoticeBlueprint, top, left, across, down);
        foreach (BCanvasText scaffold in BBlueprint.BBlueprintDraw(_bNoticeBlueprint, glyphs))
        {
            yield return scaffold;
        }

        const string title = " Llyn ";
        yield return new BCanvasText(top, left + ((across - title.Length) / 2), title.Length, title, theme);
        for (int line = 0; line < Math.Min(lines.Count, text.BBlueprintPartHeight); line++)
        {
            yield return new BCanvasText(
                text.BBlueprintPartRow + line, text.BBlueprintPartColumn, text.BBlueprintPartWidth, lines[line], theme);
        }

        foreach (BCanvasText key in legend)
        {
            yield return key;
        }
    }
}
