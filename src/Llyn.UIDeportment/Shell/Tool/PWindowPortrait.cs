using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private static readonly IReadOnlyList<(string PWindowKey, string PWindowSuffix, CPortraitMedium PWindowKind)>
        PWindowPortraitKinds =
        [
            ("Export.Markup", ".llx", CPortraitMedium.CPortraitMediumMarkup),
            ("Export.Html", ".html", CPortraitMedium.CPortraitMediumHtml),
            ("Export.Markdown", ".md", CPortraitMedium.CPortraitMediumMarkdown),
            ("Export.Docx", ".docx", CPortraitMedium.CPortraitMediumDocx),
            ("Export.Pdf", ".pdf", CPortraitMedium.CPortraitMediumPdf),
        ];

    internal Task PWindowPortraitExport(
        string file, Func<string, LPortraitMedium, LPortraitLabel, Task> export)
    {
        return PWindowPortraitExport(file, QPortrait.QPortraitPortraitCreate(export));
    }

    internal async Task PWindowPortraitExport(
        string file, Func<string, CPortraitMedium, CPortraitLabel, Task> export)
    {
        List<string> filters = new List<string>();
        foreach ((string key, string suffix, CPortraitMedium _) in PWindowPortraitKinds)
        {
            filters.Add($"{QLocalizationCatalog.QLocalizationTextRead(key)}|*{suffix}");
        }

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Export.Title"),
            Filter = string.Join("|", filters),
            FilterIndex = 2,
            AddExtension = true,
            FileName = file,
        };

        if (dialog.ShowDialog(_pWindowSurface) != true)
        {
            return;
        }

        int chosen = Math.Clamp(dialog.FilterIndex - 1, 0, PWindowPortraitKinds.Count - 1);
        CPortraitMedium format = PWindowPortraitKinds[chosen].PWindowKind;

        try
        {
            await export(dialog.FileName, format, PWindowPortraitRead());
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Export.Failed", exception);
        }
    }
}
