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
        long entryId, string path, LPortraitFormat format, LPortraitLabel label)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitFormat.LPortraitFormatMarkup)
        {
            LEngineMarkupExport([entryId], path);
            return;
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
                await LEnginePressRead().LPressSave(LSheet.LSheetFormat(portrait, theme), path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    public async Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(ticket);

        LPress press = LEnginePressRead();
        LPortrait portrait = LEnginePortraitRead(entryId, label);

        await press.LPressPrint(LSheet.LSheetFormat(portrait, LTheme.LThemeLoad()), ticket);
    }

    public async Task LEnginePortraitPrint(long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);
        ArgumentNullException.ThrowIfNull(ticket);

        LPress press = LEnginePressRead();
        LPortraitPage page = LEnginePortraitRead(id, owner, legend);

        await press.LPressPrint(LSheetPage.LSheetPageFormat(page, LTheme.LThemeLoad()), ticket);
    }

    private LPress LEnginePressRead()
    {
        return _lEnginePress
            ?? throw new InvalidOperationException("No printing surface stands ready for this workspace.");
    }
}
