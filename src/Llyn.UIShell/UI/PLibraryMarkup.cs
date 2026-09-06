using System;
using System.Threading.Tasks;
using System.Windows;

namespace Llyn.UIShell;

public partial class PLibrary
{
    internal async void PLibraryMarkupHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a markup file",
            Filter = "Llyn Markup|*.llx|All files|*.*",
            CheckFileExists = true,
        };

        if (dialog.ShowDialog(_pLibraryHost) != true)
        {
            return;
        }

        try
        {
            string path = dialog.FileName;

            await Task.Run(() => _lEngine.LEngineMarkupImport(path));

            PIndexFind(PInquiry.Text ?? string.Empty);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("List.ImportFailed", exception);
        }
    }
}
