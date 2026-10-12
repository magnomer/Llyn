using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPortraitClerkPress
{
    private readonly LPortraitVault _lPortraitPressPortraits;
    private readonly LPress _lPortraitPressPrinter;
    private readonly LPressQueue _lPortraitPressQueue;

    public LPortraitClerkPress(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lPortraitPressPortraits = rig.LRigAsset.LRigAssetPortrait;
        _lPortraitPressPrinter = rig.LRigPress;
        _lPortraitPressQueue = rig.LRigQueue;
    }

    public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (format == LPortraitMedium.LPortraitMediumPdf)
        {
            string sheet = _lPortraitPressPortraits.LPortraitSheetFormat(LPortraitPage.LPortraitPageOpen(portrait));
            await _lPortraitPressPrinter.LPressSave(sheet, path).ConfigureAwait(false);
            return;
        }

        _lPortraitPressPortraits.LPortraitSave(portrait, format, path);
    }

    public Task<LGalleyClerk> LPortraitClerkOpen(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return LPortraitClerkOpen(_lPortraitPressPortraits.LPortraitSheetFormat(LPortraitPage.LPortraitPageOpen(page)));
    }

    public async Task LPortraitClerkExport(
        LLiveryPage livery,
        LPortraitPage portrait,
        LPortraitLabel label,
        Func<string, string> lookup,
        string path,
        LPortraitMedium format)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(lookup);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        switch (format)
        {
            case LPortraitMedium.LPortraitMediumPdf:
                await _lPortraitPressPrinter.LPressSave(
                        _lPortraitPressPortraits.LPortraitSheetFormat(
                            livery, LPortraitPage.LPortraitPageOpen(portrait), label, lookup),
                        path)
                    .ConfigureAwait(false);
                return;
            case LPortraitMedium.LPortraitMediumHtml:
                _lPortraitPressPortraits.LPortraitSave(livery, portrait, label, lookup, path);
                return;
            default:
                await LPortraitClerkExport(portrait, path, format).ConfigureAwait(false);
                return;
        }
    }

    public Task<LGalleyClerk> LPortraitClerkOpen(
        LLiveryPage livery,
        LPortraitPage portrait,
        LPortraitLabel label,
        Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(lookup);

        return LPortraitClerkOpen(_lPortraitPressPortraits.LPortraitSheetFormat(
            livery, LPortraitPage.LPortraitPageOpen(portrait), label, lookup));
    }

    private async Task<LGalleyClerk> LPortraitClerkOpen(string html)
    {
        return new LGalleyClerk(await _lPortraitPressPrinter.LPressBoardOpen(html));
    }

    public async Task<IReadOnlyList<LPressPrinter>> LPortraitPrinterRead()
    {
        IReadOnlyList<LPressPrinter> printers = await _lPortraitPressQueue.LPressQueueRead();
        return printers.Select(static printer => LPortraitPrinterRead(printer)).ToList();
    }

    private static LPressPrinter LPortraitPrinterRead(LPressPrinter printer)
    {
        List<LPressPaper> papers = new();

        foreach (LPressPaper paper in printer.LPressPrinterPaper)
        {
            LPressPaper sheet = new(
                Math.Min(paper.LPressPaperWidth, paper.LPressPaperHeight),
                Math.Max(paper.LPressPaperWidth, paper.LPressPaperHeight));

            if (papers.Exists(sheet.LPressPaperMatch))
            {
                continue;
            }

            LPressPaper? standard = LPressPaper.LPressPaperStandard.FirstOrDefault(sheet.LPressPaperMatch);
            papers.Add(sheet with { LPressPaperName = standard?.LPressPaperName });
        }

        return printer with { LPressPrinterPaper = papers };
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
