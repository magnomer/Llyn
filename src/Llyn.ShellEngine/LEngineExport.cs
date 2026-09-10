using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LPress? _lEnginePress;

    public void LEnginePressApply(LPress press)
    {
        ArgumentNullException.ThrowIfNull(press);

        _lEnginePress = press;
    }

    public async Task LEnginePortraitExport(
        string entryId, string path, LPortraitFormat format, LPortraitLabel label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitFormat.LPortraitFormatMarkup)
        {
            throw new NotSupportedException("No markup writer stands in this workspace.");
        }

        LPortrait portrait = LEnginePortraitRead(entryId, label);
        LTheme theme = LTheme.LThemeLoad();

        switch (format)
        {
            case LPortraitFormat.LPortraitFormatHtml:
                File.WriteAllText(
                    path, LSheet.LSheetFormat(portrait, theme), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatMarkdown:
                File.WriteAllText(
                    path, LOutline.LOutlineFormat(portrait), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatDocx:
                LFolio.LFolioSave(portrait, theme, path);
                return;
            case LPortraitFormat.LPortraitFormatPdf:
                if (_lEnginePress is not LPress press)
                {
                    throw new InvalidOperationException(
                        "No printing surface stands ready for this workspace.");
                }

                await press.LPressSave(LSheet.LSheetFormat(portrait, theme), path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }
}
