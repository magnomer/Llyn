using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

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

            LMarkupCargo cargo = await Task.Run(() => _lEngine.LEngineMarkupRead(path));

            IReadOnlyList<LMarkupIntake>? intakes = PSCustoms.PSCustomsShow(
                _pLibraryHost, _lEngine, cargo.LMarkupCargoEntry);
            if (intakes is null)
            {
                return;
            }

            LMarkupOutcome outcome = await Task.Run(() => _lEngine.LEngineMarkupImport(cargo, intakes));

            PIndexFind(PInquiry.Text ?? string.Empty);
            PSCustoms.PSCustomsOmissionShow(_pLibraryHost, outcome.LMarkupOutcomeOmission);
        }
        catch (Exception exception)
        {
            _pLibraryHost.PWindowFailureShow("List.ImportFailed", exception);
        }
    }
}
