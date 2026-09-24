using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public static class LHeadquarter
{
    public static void LHeadquarterToggle(Popup menu, MouseButtonEventArgs e)
    {
        menu.IsOpen = !menu.IsOpen;
        e.Handled = true;
    }

    public static void LHeadquarterAboutHandle(
        Window window, Popup menu, MouseButtonEventArgs e, string product, string label)
    {
        menu.IsOpen = false;
        e.Handled = true;

        MessageBox.Show(
            window,
            $"{product}\n{label} " + LHeadquarterVersionRead(),
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void LHeadquarterExitHandle(Window window, Popup menu, MouseButtonEventArgs e)
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
