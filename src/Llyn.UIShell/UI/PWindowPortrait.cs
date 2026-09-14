using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.UIShell;

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

    internal async Task PWindowPortraitExport(long id)
    {
        List<string> filters = new List<string>();
        foreach ((string key, string suffix, LPortraitFormat _) in PWindowPortraitKinds)
        {
            filters.Add(PLocalizationTextRead(key) + "|*" + suffix);
        }

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = PLocalizationTextRead("Export.Title"),
            Filter = string.Join("|", filters),
            FilterIndex = 2,
            AddExtension = true,
            FileName = PWindowHeadwordRead(id),
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        int chosen = Math.Clamp(dialog.FilterIndex - 1, 0, PWindowPortraitKinds.Count - 1);
        LPortraitFormat format = PWindowPortraitKinds[chosen].PWindowKind;

        try
        {
            await _lEngine.LEnginePortraitExport(id, dialog.FileName, format, PWindowLabelRead());
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Export.Failed", exception);
        }
    }

    private string PWindowHeadwordRead(long id)
    {
        string headword;
        try
        {
            headword = _lEngine.LEngineEntryRead(id)?.LEntryHeadword ?? string.Empty;
        }
        catch (Exception)
        {
            headword = string.Empty;
        }

        string trimmed = headword.Trim();
        foreach (char barred in Path.GetInvalidFileNameChars())
        {
            trimmed = trimmed.Replace(barred, '_');
        }

        return trimmed.Length == 0 ? "entry" : trimmed;
    }
}
