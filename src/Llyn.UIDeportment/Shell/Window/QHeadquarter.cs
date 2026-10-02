using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    private Popup QHeadquarter => (Popup)_qWindowSurface.FindName("PHeadquarter");

    private void QLogoRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarter.IsOpen = !QHeadquarter.IsOpen;
        e.Handled = true;
    }

    private void QHeadquarterAboutRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarter.IsOpen = false;
        e.Handled = true;

        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        string build = version is null ? string.Empty : version.ToString(3);
        string product = QLocalizationCatalog.QLocalizationTextRead("Terms.Product");
        MessageBox.Show(
            _qWindowSurface,
            $"{product}\n{QLocalizationCatalog.QLocalizationTextRead(CAtelier.CAtelierAboutRead())} {build}",
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void QHeadquarterExitRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarter.IsOpen = false;
        e.Handled = true;

        SystemCommands.CloseWindow(_qWindowSurface);
    }
}
