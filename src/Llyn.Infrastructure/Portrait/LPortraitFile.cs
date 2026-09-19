using System;
using System.IO;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LPortraitFile : LPortraitVault
{
    public void LPortraitSave(LPortraitPage page, LPortraitFormat format, string path)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        switch (format)
        {
            case LPortraitFormat.LPortraitFormatHtml:
                File.WriteAllText(path, LPortraitSheetFormat(page), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatMarkdown:
                File.WriteAllText(path, LOutline.LOutlineFormat(page), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatDocx:
                LFolio.LFolioSave(page, LThemeLoader.LThemeLoaderLoad(), path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, "This format is not written as a file.");
        }
    }

    public string LPortraitSheetFormat(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return LSheet.LSheetFormat(page, LThemeLoader.LThemeLoaderLoad());
    }
}
