using System.Windows;

namespace Llyn.UIDeportment;

internal static class QMarkup
{
    internal static string? QMarkupConsult(Window owner)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a markup file",
            Filter = "Llyn Markup|*.llx|All files|*.*",
            CheckFileExists = true,
        };

        return dialog.ShowDialog(owner) == true ? dialog.FileName : null;
    }
}
