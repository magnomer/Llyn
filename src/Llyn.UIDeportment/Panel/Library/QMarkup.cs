using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Llyn.UIDeportment;

internal static class QMarkup
{
    internal static string? QMarkupConsult(Window owner, string key, IReadOnlyList<string> suffixes)
    {
        ArgumentNullException.ThrowIfNull(suffixes);

        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead(key),
            Filter = string.Concat(
                QLocalizationCatalog.QLocalizationTextRead("Export.Markup"),
                "|",
                string.Join(";", suffixes.Select(static suffix => "*" + suffix)),
                "|All files|*.*"),
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }
}
