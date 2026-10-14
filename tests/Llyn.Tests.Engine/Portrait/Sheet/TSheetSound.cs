using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSheetSound
{
    [Fact]
    public void SheetEntryFormat_TonedContour_CentresPlotUnderReading()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "好", "Mandarin", "xau214", string.Empty, [TInterface.TCardCreate("good", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        page = page with
        {
            LLiveryPageAccent = page.LLiveryPageAccent with
            {
                LAccentSheetContour = [TInterface.TContourCreate("xau214", [2, 1, 4])],
            },
        };

        string sheet = TInterface.TSheetEntryFormat(
            page, engine.TEnginePortraitRead(entry.LEntryId, TInterface.TPortraitLabelRead()), static key => key);

        int axis = sheet.IndexOf("<td><span class=\"llyn-axis\"><span>", StringComparison.Ordinal);
        Assert.True(axis >= 0);
        string cell = sheet[axis..(sheet.IndexOf("</td>", axis, StringComparison.Ordinal) + 5)];
        int reading = cell.IndexOf("<span class=\"text\">", StringComparison.Ordinal);
        int contour = cell.IndexOf("</span><span class=\"llyn-contour\">", StringComparison.Ordinal);
        Assert.True(reading >= 0 && reading < contour);
        Assert.EndsWith("\" class=\"llyn-contour-chart\"></span></span></td>", cell, StringComparison.Ordinal);
        Assert.DoesNotContain("<tr class=\"contour\">", sheet, StringComparison.Ordinal);
        Assert.Contains(
            ".llyn-axis{display:inline-flex;flex-direction:column;align-items:center;}",
            sheet,
            StringComparison.Ordinal);
    }
}
