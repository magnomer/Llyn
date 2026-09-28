using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class LHeadquarter
{
    public static void LHeadquarterMenuRefine(Popup menu, MouseButtonEventArgs e)
    {
        menu.IsOpen = !menu.IsOpen;
        e.Handled = true;
    }

    public static void LHeadquarterAboutRefine(
        Window window, Popup menu, MouseButtonEventArgs e, string key)
    {
        menu.IsOpen = false;
        e.Handled = true;

        string product = QLocalizationCatalog.QLocalizationTextRead("Terms.Product");
        MessageBox.Show(
            window,
            $"{product}\n{QLocalizationCatalog.QLocalizationTextRead(key)} {LHeadquarterVersionRead()}",
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void LHeadquarterExitRefine(Window window, Popup menu, MouseButtonEventArgs e)
    {
        menu.IsOpen = false;
        e.Handled = true;

        SystemCommands.CloseWindow(window);
    }

    private static string LHeadquarterVersionRead()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;

        return version is null ? string.Empty : version.ToString(3);
    }
}
