using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private static readonly IReadOnlyList<(string PWindowKey, string PWindowSuffix, LPortraitFormat PWindowKind)>
        PWindowPortraitKinds =
        [
            ("Export.Markup", ".llx", LPortraitFormat.LPortraitFormatMarkup),
            ("Export.Html", ".html", LPortraitFormat.LPortraitFormatHtml),
            ("Export.Markdown", ".md", LPortraitFormat.LPortraitFormatMarkdown),
            ("Export.Docx", ".docx", LPortraitFormat.LPortraitFormatDocx),
            ("Export.Pdf", ".pdf", LPortraitFormat.LPortraitFormatPdf),
        ];

    internal async Task PWindowPortraitExport(
        string file, Func<string, LPortraitFormat, LPortraitLabel, Task> export)
    {
        List<string> filters = new List<string>();
        foreach ((string key, string suffix, LPortraitFormat _) in PWindowPortraitKinds)
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
        LPortraitFormat format = PWindowPortraitKinds[chosen].PWindowKind;

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
