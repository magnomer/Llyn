using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Llyn.UIShell;

public partial class PList
{
    internal async void PMarkupImportHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = "Choose a markup file",
            Filter = "Llyn Markup|*.llx|All files|*.*",
            CheckFileExists = true,
        };

        if (dialog.ShowDialog(_pListHost) != true)
        {
            return;
        }

        try
        {
            string text = await File.ReadAllTextAsync(dialog.FileName);

            await Task.Run(() => _lEngine.LEngineMarkupImport(text));
        }
        catch (Exception exception)
        {
            _pListHost.PWindowFailureShow("List.ImportFailed", exception);
            return;
        }

        PIndexFind(PInquiry.Text ?? string.Empty);
    }
}
