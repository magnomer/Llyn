using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TDrawer
{
    private const string TDrawerSpace =
        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "
        + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";

    private const string TDrawerSurface = """
        <Style x:Key="Theme.Popup.Surface" TargetType="Border">
            <Setter Property="Padding" Value="6" />
            <Setter Property="Background" Value="{StaticResource Theme.Surface}" />
            <Setter Property="BorderBrush" Value="{StaticResource Theme.Line}" />
            <Setter Property="BorderThickness" Value="1" />
            <Setter Property="CornerRadius" Value="14" />
            <Setter Property="Effect">
                <Setter.Value>
                    <DropShadowEffect BlurRadius="10" Direction="270" Opacity="0.18" ShadowDepth="4" />
                </Setter.Value>
            </Setter>
        </Style>
        """;

    private const string TDrawerPopup = """
        <Style x:Key="Editor.Popup.Frame" TargetType="Border" BasedOn="{StaticResource Theme.Popup.Surface}">
            <Setter Property="Margin" Value="10" />
            <Setter Property="Padding" Value="4" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="CornerRadius" Value="10" />
            <Setter Property="Effect">
                <Setter.Value>
                    <DropShadowEffect BlurRadius="10" Direction="270" Opacity="0.18" ShadowDepth="4" />
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="Editor.Popup.List" TargetType="ListBox">
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="Focusable" Value="False" />
            <Setter Property="SelectionMode" Value="Single" />
            <Setter Property="ScrollViewer.HorizontalScrollBarVisibility" Value="Disabled" />
            <Setter Property="ScrollViewer.VerticalScrollBarVisibility" Value="Auto" />
        </Style>

        <Style x:Key="Editor.Popup.Row" TargetType="ListBoxItem">
            <Setter Property="Focusable" Value="False" />
            <Setter Property="HorizontalContentAlignment" Value="Stretch" />
            <Setter Property="Padding" Value="0" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="ListBoxItem">
                        <Border
                            x:Name="PSurface"
                            MinHeight="38"
                            Padding="10,0"
                            Background="Transparent"
                            CornerRadius="9">
                            <ContentPresenter VerticalAlignment="Center" />
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="Editor.Popup.Caret" TargetType="ListBoxItem" BasedOn="{StaticResource Editor.Popup.Row}">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="ListBoxItem">
                        <Border
                            x:Name="PSurface"
                            MinHeight="28"
                            Padding="8,0"
                            Background="Transparent"
                            CornerRadius="6">
                            <ContentPresenter VerticalAlignment="Center" />
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        """;

    private const string TDrawerRow = """
        <DataTemplate x:Key="Theme.Citation.Row">
            <Grid x:Name="PCitationRow" Background="Transparent">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                <TextBlock
                    VerticalAlignment="Center"
                    FontSize="13"
                    Foreground="{DynamicResource Theme.Ink}"
                    TextTrimming="CharacterEllipsis">
                    <Run x:Name="PCitationLead" /><Run
                        x:Name="PCitationMark"
                        FontWeight="SemiBold"
                        Foreground="{DynamicResource Theme.Accent}" /><Run x:Name="PCitationTail" />
                </TextBlock>
                <TextBlock
                    x:Name="PCitationCount"
                    Grid.Column="1"
                    Margin="10,0,0,0"
                    VerticalAlignment="Center"
                    FontSize="10.5"
                    Foreground="{DynamicResource Theme.Muted}" />
            </Grid>
        </DataTemplate>
        """;

    private const string TDrawerField = """
        <Grid HorizontalAlignment="Left">
            <TextBox
                x:Name="PCitationField"
                MinWidth="138" />
            <Popup
                x:Name="PCitationDrawer"
                AllowsTransparency="True"
                Placement="Custom"
                StaysOpen="False">
                <Border x:Name="PCitationSheet" Style="{StaticResource Editor.Popup.Frame}">
                    <ListBox
                        x:Name="PCitationList"
                        MaxHeight="232"
                        ItemContainerStyle="{StaticResource Editor.Popup.Caret}"
                        ItemTemplate="{DynamicResource Theme.Citation.Row}"
                        Style="{StaticResource Editor.Popup.List}" />
                </Border>
            </Popup>
        </Grid>
        """;

    private static readonly CProfferRow[] TDrawerShort =
    [
        new(1, "A", string.Empty, string.Empty),
        new(2, "Major Author (2018)", string.Empty, string.Empty),
    ];

    private static readonly CProfferRow[] TDrawerLong =
    [
        .. TDrawerShort,
        new(3, "Minor Author and Another Author (2020)", string.Empty, string.Empty),
    ];

    private static bool _tDrawerAttached;

    [Fact]
    public void DrawerMove_SecondRow_PaintsSelectedSurface()
    {
        TWindow.TWindowRun(() => TDrawerOpen(TDrawerLong, (drawer, surface) =>
        {
            Assert.True(drawer.QDrawerMove(true));
            Assert.True(drawer.QDrawerMove(true));
            TDrawerSettle();

            ListBox list = (ListBox)surface.FindName("PCitationList");
            ListBoxItem row = (ListBoxItem)list.ItemContainerGenerator.ContainerFromIndex(1);
            Border paint = (Border)row.Template.FindName("PSurface", row);
            Assert.Equal(1, list.SelectedIndex);
            Assert.Same(System.Windows.Application.Current.Resources["Theme.AccentSoft"], paint.Background);
        }));
    }

    [Fact]
    public void DrawerShow_FirstOpen_WidensToLongestRow()
    {
        TWindow.TWindowRun(() => TDrawerOpen(TDrawerLong, (_, surface) => TDrawerWidthCheck(surface)));
    }

    [Fact]
    public void DrawerShow_LongerRowsWhileOpen_WidensToLongestRow()
    {
        TWindow.TWindowRun(() => TDrawerOpen(TDrawerShort, (drawer, surface) =>
        {
            drawer.QDrawerShow(new CProffer("a", TDrawerLong, true), (TextBox)surface.FindName("PCitationField"));
            TDrawerSettle();

            TDrawerWidthCheck(surface);
        }));
    }

    private static void TDrawerWidthCheck(UserControl surface)
    {
        Border sheet = (Border)surface.FindName("PCitationSheet");
        TextBox field = (TextBox)surface.FindName("PCitationField");
        PresentationSource source = PresentationSource.FromVisual(sheet);
        FrameworkElement root = (FrameworkElement)source.RootVisual;
        double shown = root.ActualWidth;
        sheet.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double wanted = sheet.DesiredSize.Width;

        Assert.True(wanted > field.ActualWidth + 40, $"wanted {wanted}, field {field.ActualWidth}");
        Assert.Equal(wanted, shown, 0.5);
    }

    private static void TDrawerOpen(CProfferRow[] rows, Action<QDrawer, UserControl> body)
    {
        TDrawerPrepare();
        UserControl surface = (UserControl)XamlReader.Parse(
            $"<UserControl {TDrawerSpace}><UserControl.Resources><ResourceDictionary>"
            + $"<ResourceDictionary.MergedDictionaries><ResourceDictionary>{TDrawerPopup}</ResourceDictionary>"
            + $"<ResourceDictionary>{TDrawerRow}</ResourceDictionary></ResourceDictionary.MergedDictionaries>"
            + $"</ResourceDictionary></UserControl.Resources>{TDrawerField}</UserControl>");
        QLook.QLookStyleAttach(surface);
        Popup popup = (Popup)surface.FindName("PCitationDrawer");
        popup.StaysOpen = true;
        QDrawer drawer = new(
            popup,
            (Border)surface.FindName("PCitationSheet"),
            (ListBox)surface.FindName("PCitationList"),
            (_, _) => { });
        using HwndSource host = new(new HwndSourceParameters("TDrawer", 600, 400)) { RootVisual = surface };
        TDrawerSettle();
        try
        {
            drawer.QDrawerShow(new CProffer("a", rows, true), (TextBox)surface.FindName("PCitationField"));
            TDrawerSettle();
            Assert.True(popup.IsOpen);
            body(drawer, surface);
        }
        finally
        {
            drawer.QDrawerHide();
        }
    }

    private static void TDrawerSettle()
    {
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ContextIdle);
    }

    private static void TDrawerPrepare()
    {
        System.Windows.Application application =
            System.Windows.Application.Current ?? new System.Windows.Application();
        application.Resources["PIconRoot"] = "pack://application:,,,/Llyn.Tests.Windows;component/icons/";
        application.Resources["Theme.Popup.ProgressBar.Sweep"] = new DoubleAnimation().GetAsFrozen();
        application.Resources["Theme.Sound.Rebuild.Spin"] = new DoubleAnimation().GetAsFrozen();
        foreach ((string key, Color color) in new[]
        {
            ("Theme.Surface", Colors.White), ("Theme.Line", Colors.Gray), ("Theme.Ink", Colors.Black),
            ("Theme.Accent", Colors.Blue), ("Theme.Muted", Colors.DimGray), ("Theme.AccentSoft", Colors.LightBlue),
            ("Theme.SurfaceRaised", Colors.WhiteSmoke),
        })
        {
            application.Resources[key] = new SolidColorBrush(color).GetAsFrozen();
        }

        if (!application.Resources.Contains("Theme.Popup.Surface"))
        {
            ResourceDictionary surface = (ResourceDictionary)XamlReader.Parse(
                $"<ResourceDictionary {TDrawerSpace}>{TDrawerSurface}</ResourceDictionary>");
            application.Resources["Theme.Popup.Surface"] = surface["Theme.Popup.Surface"];
        }

        if (!_tDrawerAttached)
        {
            _tDrawerAttached = true;
            QLook.QLookStateAttach();
        }
    }
}
