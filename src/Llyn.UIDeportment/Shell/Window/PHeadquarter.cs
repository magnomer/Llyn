using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private Popup PHeadquarter => (Popup)_pWindowSurface.FindName(nameof(PHeadquarter));

    private void PLogoRefine(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = !PHeadquarter.IsOpen;
        e.Handled = true;
    }

    private void PHeadquarterAboutRefine(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = false;
        e.Handled = true;

        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        string build = version is null ? string.Empty : version.ToString(3);
        string product = QLocalizationCatalog.QLocalizationTextRead("Terms.Product");
        MessageBox.Show(
            _pWindowSurface,
            $"{product}\n{QLocalizationCatalog.QLocalizationTextRead(CAtelier.CAtelierAboutRead())} {build}",
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void PHeadquarterExitRefine(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = false;
        e.Handled = true;

        SystemCommands.CloseWindow(_pWindowSurface);
    }
}
