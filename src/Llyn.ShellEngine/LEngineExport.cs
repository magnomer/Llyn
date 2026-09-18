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

    internal async Task LEnginePortraitExport(
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

        LPortraitPage portrait = LEnginePortraitRead(entryId, label);
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

    internal async Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(ticket);

        LPress press = LEnginePressRead();
        LPortraitPage portrait = LEnginePortraitRead(entryId, label);

        await press.LPressPrint(LSheet.LSheetFormat(portrait, LTheme.LThemeLoad()), ticket);
    }

    public Task LEnginePortraitExport(
        LVista? vista, string path, LPortraitFormat format, LPortraitLabel label)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? LEnginePortraitExport(id, path, format, label)
            : Task.CompletedTask;
    }

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? LEnginePortraitPrint(id, label, ticket)
            : Task.CompletedTask;
    }

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket)
    {
        if (vista?.LVistaChosen is not long id)
        {
            return Task.CompletedTask;
        }

        LOwner owner = vista.LVistaSubject switch
        {
            LSubject.LSubjectExample => LOwner.LOwnerExample,
            LSubject.LSubjectSituation => LOwner.LOwnerSituation,
            LSubject.LSubjectReference => LOwner.LOwnerReference,
            _ => throw new ArgumentException("The vista does not hold a printable catalog subject.", nameof(vista)),
        };
        return LEnginePortraitPrint(id, owner, legend, ticket);
    }

    internal async Task LEnginePortraitPrint(long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);
        ArgumentNullException.ThrowIfNull(ticket);

        LPress press = LEnginePressRead();
        LPortraitPage page = LEnginePortraitRead(id, owner, legend);

        await press.LPressPrint(LSheet.LSheetFormat(page, LTheme.LThemeLoad()), ticket);
    }

    private LPress LEnginePressRead()
    {
        return _lEnginePress
            ?? throw new InvalidOperationException("No printing surface stands ready for this workspace.");
    }
}
