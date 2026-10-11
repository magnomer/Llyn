using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LPressClerk
{
    private readonly LPressVault _lPressClerkVault;

    public LPressClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lPressClerkVault = rig.LRigPressVault;
    }

    public LPressSetting LPressClerkCreate(IReadOnlyList<LPressPrinter> printers)
    {
        ArgumentNullException.ThrowIfNull(printers);

        return LPressSetting.LPressSettingCreate(LPressClerkFind(printers));
    }

    public LPressSetting LPressClerkSelect(LPressSetting setting, IReadOnlyList<LPressPrinter> printers, string? name)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentNullException.ThrowIfNull(printers);

        LPressPrinter? printer = name is null
            ? null
            : LPressClerkFind(printers, name)
                ?? throw new ArgumentException("The printer is not in the galley's list.", nameof(name));
        return setting.LPressSettingClamp(printer);
    }

    public LPressSetting LPressClerkClamp(LPressSetting setting, IReadOnlyList<LPressPrinter> printers)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentNullException.ThrowIfNull(printers);

        return setting.LPressSettingClamp(LPressClerkFind(printers, setting.LPressSettingPrinter));
    }

    public LPressSetting? LPressClerkRestore(IReadOnlyList<LPressPrinter> printers)
    {
        ArgumentNullException.ThrowIfNull(printers);

        LPressSetting? saved = _lPressClerkVault.LPressVaultRead();
        if (saved is null)
        {
            return null;
        }

        LPressPrinter? printer = saved.LPressSettingPrinter is null
            ? null
            : LPressClerkFind(printers, saved.LPressSettingPrinter) ?? LPressClerkFind(printers);
        return (saved with { LPressSettingRange = new LPressRange(string.Empty) }).LPressSettingClamp(printer);
    }

    public void LPressClerkSave(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        _lPressClerkVault.LPressVaultSave(setting with { LPressSettingRange = new LPressRange(string.Empty) });
    }

    public LPressRange? LPressClerkParse(string text, int count)
    {
        return LPressRange.LPressRangeParse(text, count);
    }

    public LPressMargin LPressClerkResolve(double top, double bottom, double left, double right, bool metric)
    {
        return LPressMargin.LPressMarginResolve(top, bottom, left, right, metric);
    }

    public LPressMargin LPressClerkResolve(LPressMargin margin, string? name)
    {
        ArgumentNullException.ThrowIfNull(margin);

        return name is null
            ? margin with { LPressMarginName = null }
            : LPressMargin.LPressMarginPreset.FirstOrDefault(preset => preset.LPressMarginName == name)
                ?? throw new ArgumentException("The margin is not a preset.", nameof(name));
    }

    public LPressDigest LPressClerkRead(LPressSetting setting, IReadOnlyList<LPressPrinter> printers, int? count)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentNullException.ThrowIfNull(printers);

        LPressPrinter? printer = LPressClerkFind(printers, setting.LPressSettingPrinter);
        IReadOnlyList<LPressPaper> papers = LPressPaper.LPressPaperRead(printer);
        int index = papers.ToList().FindIndex(paper => paper.LPressPaperMatch(setting.LPressSettingPaper));

        LPressRange range = setting.LPressSettingRange;
        IReadOnlyList<bool> included = count is int pages
            ? Enumerable.Range(0, pages).Select(range.LPressRangeCheck).ToList()
            : Array.Empty<bool>();

        int chosen = included.Count(flag => flag);
        int sheets = setting.LPressSettingSide is LPressSide.LPressSideLong or LPressSide.LPressSideShort
            ? (chosen + 1) / 2
            : chosen;

        return new LPressDigest(
            printer,
            papers,
            index < 0 ? null : index,
            included,
            sheets * setting.LPressSettingCopies);
    }

    private static LPressPrinter? LPressClerkFind(IReadOnlyList<LPressPrinter> printers, string? name)
    {
        return name is null ? null : printers.FirstOrDefault(printer => printer.LPressPrinterName == name);
    }

    private static LPressPrinter? LPressClerkFind(IReadOnlyList<LPressPrinter> printers)
    {
        return printers.FirstOrDefault(printer => printer.LPressPrinterDefault) ?? printers.FirstOrDefault();
    }
}
