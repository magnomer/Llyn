using System;
using System.IO;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LPortraitFile : LPortraitVault
{
    private readonly LTheme _lPortraitFileTheme;

    public LPortraitFile(LTheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        _lPortraitFileTheme = theme;
    }

    public void LPortraitSave(LPortraitPage page, LPortraitMedium format, string path)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        switch (format)
        {
            case LPortraitMedium.LPortraitMediumHtml:
                File.WriteAllText(path, LPortraitSheetFormat(page), new UTF8Encoding(false));
                return;
            case LPortraitMedium.LPortraitMediumMarkdown:
                File.WriteAllText(path, LOutline.LOutlineFormat(page), new UTF8Encoding(false));
                return;
            case LPortraitMedium.LPortraitMediumDocx:
                LFolio.LFolioSave(page, _lPortraitFileTheme, path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, "This format is not written as a file.");
        }
    }

    public string LPortraitSheetFormat(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return LSheet.LSheetFormat(page, _lPortraitFileTheme);
    }
}
