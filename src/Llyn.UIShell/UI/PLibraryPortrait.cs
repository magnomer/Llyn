using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PLibrary
{
    private static readonly IReadOnlyList<(string PLibraryKey, string PLibrarySuffix, LPortraitFormat PLibraryKind)>
        PLibraryPortraitKinds =
        [
            ("Export.Markup", ".llx", LPortraitFormat.LPortraitFormatMarkup),
            ("Export.Html", ".html", LPortraitFormat.LPortraitFormatHtml),
            ("Export.Markdown", ".md", LPortraitFormat.LPortraitFormatMarkdown),
            ("Export.Docx", ".docx", LPortraitFormat.LPortraitFormatDocx),
            ("Export.Pdf", ".pdf", LPortraitFormat.LPortraitFormatPdf),
        ];

    internal async void PLibraryPortraitHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayEntry is not long id)
        {
            return;
        }

        List<string> filters = new List<string>();
        foreach ((string key, string suffix, LPortraitFormat _) in PLibraryPortraitKinds)
        {
            filters.Add(_pLibraryHost.PLocalizationTextRead(key) + "|*" + suffix);
        }

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = _pLibraryHost.PLocalizationTextRead("Export.Title"),
            Filter = string.Join("|", filters),
            FilterIndex = 2,
            AddExtension = true,
            FileName = PLibraryNameRead(id),
        };

        if (dialog.ShowDialog(_pLibraryHost) != true)
        {
            return;
        }

        int chosen = Math.Clamp(dialog.FilterIndex - 1, 0, PLibraryPortraitKinds.Count - 1);
        LPortraitFormat format = PLibraryPortraitKinds[chosen].PLibraryKind;

        try
        {
            await _lEngine.LEnginePortraitExport(
                id, dialog.FileName, format, PLibraryLabelRead());
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("Export.Failed", exception);
        }
    }

    private LPortraitLabel PLibraryLabelRead()
    {
        return new LPortraitLabel(
            _pLibraryHost.PLocalizationTextRead("Display.Unreadable"),
            _pLibraryHost.PLocalizationTextRead("Display.MeaningSingle"),
            _pLibraryHost.PLocalizationTextRead("Display.MeaningPlural"),
            _pLibraryHost.PLocalizationTextRead("Display.CollocationSingle"),
            _pLibraryHost.PLocalizationTextRead("Display.Collocation"),
            _pLibraryHost.PLocalizationTextRead("Display.Translated"),
            _pLibraryHost.PLocalizationTextRead("Display.Note"));
    }

    private string PLibraryNameRead(long id)
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
