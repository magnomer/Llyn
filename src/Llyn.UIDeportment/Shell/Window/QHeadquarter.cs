using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QHeadquarter
{
    private readonly Window _qHeadquarterWindow;

    public QHeadquarter(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _qHeadquarterWindow = window;
    }

    private Popup QHeadquarterPopup => (Popup)_qHeadquarterWindow.FindName("PHeadquarter");

    private Grid QLogo => (Grid)_qHeadquarterWindow.FindName("PLogo");

    private TextBlock QHeadquarterAbout => (TextBlock)_qHeadquarterWindow.FindName("PHeadquarterAbout");

    private TextBlock QHeadquarterExit => (TextBlock)_qHeadquarterWindow.FindName("PHeadquarterExit");

    public void QHeadquarterIntroduce()
    {
        QLogo.MouseLeftButtonDown += QLogoRefine;
        QHeadquarterAbout.MouseLeftButtonDown += QHeadquarterAboutRefine;
        QHeadquarterExit.MouseLeftButtonDown += QHeadquarterExitRefine;
    }

    private void QLogoRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarterPopup.IsOpen = !QHeadquarterPopup.IsOpen;
        e.Handled = true;
    }

    private void QHeadquarterAboutRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarterPopup.IsOpen = false;
        e.Handled = true;

        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        string build = version is null ? string.Empty : version.ToString(3);
        string product = QLocalizationCatalog.QLocalizationTextRead("Terms.Product");
        MessageBox.Show(
            _qHeadquarterWindow,
            $"{product}\n{QLocalizationCatalog.QLocalizationTextRead(CAtelier.CAtelierAboutRead())} {build}",
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void QHeadquarterExitRefine(object sender, MouseButtonEventArgs e)
    {
        QHeadquarterPopup.IsOpen = false;
        e.Handled = true;

        SystemCommands.CloseWindow(_qHeadquarterWindow);
    }
}
