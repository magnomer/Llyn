using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPortraitClerkPress
{
    private readonly LPortraitVault _lPortraitPressPortraits;
    private readonly LPress _lPortraitPressPrinter;

    public LPortraitClerkPress(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lPortraitPressPortraits = rig.LRigAsset.LRigAssetPortrait;
        _lPortraitPressPrinter = rig.LRigPress;
    }

    public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (format == LPortraitMedium.LPortraitMediumPdf)
        {
            await _lPortraitPressPrinter.LPressSave(_lPortraitPressPortraits.LPortraitSheetFormat(portrait), path)
                .ConfigureAwait(false);
            return;
        }

        _lPortraitPressPortraits.LPortraitSave(portrait, format, path);
    }

    public Task LPortraitClerkPrint(LPortraitPage page, LPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(ticket);

        return _lPortraitPressPrinter.LPressPrint(_lPortraitPressPortraits.LPortraitSheetFormat(page), ticket);
    }

    public static LPressTicket LPortraitTicketCreate(
        string printer,
        double? width,
        double? height,
        bool landscape,
        int copies,
        bool collated,
        LPressSide side,
        LPressInk ink)
    {
        LPressPaper paper = width is > 0 and double across && height is > 0 and double down
            ? new LPressPaper(across, down)
            : LPressPaper.LPressPaperLocal;
        return new LPressTicket(printer, paper, landscape, copies, collated, side, ink);
    }

    public static IReadOnlyList<(LPortraitMedium, string, bool)> LPortraitMediumRead()
    {
        return
        [
            (LPortraitMedium.LPortraitMediumMarkup, ".llx", false),
            (LPortraitMedium.LPortraitMediumHtml, ".html", true),
            (LPortraitMedium.LPortraitMediumMarkdown, ".md", false),
            (LPortraitMedium.LPortraitMediumDocx, ".docx", false),
            (LPortraitMedium.LPortraitMediumPdf, ".pdf", false),
        ];
    }
}
