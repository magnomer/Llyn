using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private static readonly IReadOnlyList<(string PWindowKey, string PWindowSuffix, LPortraitMedium PWindowKind)>
        PWindowPortraitKinds =
        [
            ("Export.Markup", ".llx", LPortraitMedium.LPortraitMediumMarkup),
            ("Export.Html", ".html", LPortraitMedium.LPortraitMediumHtml),
            ("Export.Markdown", ".md", LPortraitMedium.LPortraitMediumMarkdown),
            ("Export.Docx", ".docx", LPortraitMedium.LPortraitMediumDocx),
            ("Export.Pdf", ".pdf", LPortraitMedium.LPortraitMediumPdf),
        ];

    internal async Task PWindowPortraitExport(
        string file, Func<string, LPortraitMedium, LPortraitLabel, Task> export)
    {
        List<string> filters = new List<string>();
        foreach ((string key, string suffix, LPortraitMedium _) in PWindowPortraitKinds)
        {
            filters.Add($"{PLocalizationCatalog.PLocalizationTextRead(key)}|*{suffix}");
        }

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = PLocalizationCatalog.PLocalizationTextRead("Export.Title"),
            Filter = string.Join("|", filters),
            FilterIndex = 2,
            AddExtension = true,
            FileName = file,
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        int chosen = Math.Clamp(dialog.FilterIndex - 1, 0, PWindowPortraitKinds.Count - 1);
        LPortraitMedium format = PWindowPortraitKinds[chosen].PWindowKind;

        try
        {
            await export(dialog.FileName, format, PWindowLabelRead());
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Export.Failed", exception);
        }
    }
}
