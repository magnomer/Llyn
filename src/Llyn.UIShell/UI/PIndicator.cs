using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Llyn.UIShell;

internal static class PIndicator
{
    private const string PIndicatorVerticalKey = "Theme.ScrollBar.Vertical.Template";
    private const string PIndicatorHorizontalKey = "Theme.ScrollBar.Horizontal.Template";
    private const string PIndicatorGutterKey = "Theme.Scroll.Gutter.Template";
    private const string PIndicatorCatalogKey = "Theme.Scroll.Catalog.Template";
    private const string PIndicatorCompactKey = "Theme.ScrollBar.Catalog.Template";
    private const string PIndicatorGutterSize = "40";

    internal static void PIndicatorApply(ResourceDictionary resources)
    {
        resources[PIndicatorVerticalKey] = PIndicatorTemplateBuild(Orientation.Vertical);
        resources[PIndicatorHorizontalKey] = PIndicatorTemplateBuild(Orientation.Horizontal);
        resources[PIndicatorGutterKey] = PIndicatorGutterBuild();
        resources[PIndicatorCatalogKey] = PIndicatorGutterBuild("6");
        resources[PIndicatorCompactKey] = PIndicatorTemplateBuild(Orientation.Vertical, true);
    }

    private static ControlTemplate PIndicatorGutterBuild(string size = PIndicatorGutterSize)
    {
        string markup = $$$"""
            <ControlTemplate
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                TargetType="{x:Type ScrollViewer}">
                <Grid Background="{TemplateBinding Background}">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition x:Name="PScrollColumn" Width="{{{size}}}" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*" />
                        <RowDefinition x:Name="PScrollRow" Height="{{{size}}}" />
                    </Grid.RowDefinitions>
                    <ScrollContentPresenter
                        x:Name="PART_ScrollContentPresenter"
                        Margin="{TemplateBinding Padding}"
                        CanContentScroll="{TemplateBinding CanContentScroll}"
                        CanHorizontallyScroll="False"
                        CanVerticallyScroll="False"
                        Content="{TemplateBinding Content}"
                        ContentTemplate="{TemplateBinding ContentTemplate}" />
                    <ScrollBar
                        x:Name="PART_VerticalScrollBar"
                        Grid.Row="0"
                        Grid.Column="1"
                        Maximum="{TemplateBinding ScrollableHeight}"
                        Orientation="Vertical"
                        ViewportSize="{TemplateBinding ViewportHeight}"
                        Visibility="{TemplateBinding ComputedVerticalScrollBarVisibility}"
                        Value="{Binding VerticalOffset, Mode=OneWay, RelativeSource={RelativeSource TemplatedParent}}" />
                    <ScrollBar
                        x:Name="PART_HorizontalScrollBar"
                        Grid.Row="1"
                        Grid.Column="0"
                        Maximum="{TemplateBinding ScrollableWidth}"
                        Orientation="Horizontal"
                        ViewportSize="{TemplateBinding ViewportWidth}"
                        Visibility="{TemplateBinding ComputedHorizontalScrollBarVisibility}"
                        Value="{Binding HorizontalOffset, Mode=OneWay, RelativeSource={RelativeSource TemplatedParent}}" />
                </Grid>
                <ControlTemplate.Triggers>
                    <Trigger Property="VerticalScrollBarVisibility" Value="Disabled">
                        <Setter TargetName="PScrollColumn" Property="Width" Value="0" />
                    </Trigger>
                    <Trigger Property="HorizontalScrollBarVisibility" Value="Disabled">
                        <Setter TargetName="PScrollRow" Property="Height" Value="0" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
            """;

        return (ControlTemplate)XamlReader.Parse(markup);
    }

    private static ControlTemplate PIndicatorTemplateBuild(Orientation orientation, bool compact = false)
    {
        string direction = orientation == Orientation.Vertical ? "True" : "False";
        string decrease = orientation == Orientation.Vertical ? "PageUpCommand" : "PageLeftCommand";
        string increase = orientation == Orientation.Vertical ? "PageDownCommand" : "PageRightCommand";
        string rail = orientation == Orientation.Vertical
            ? "Width=\"16\" VerticalAlignment=\"Stretch\""
            : "Height=\"16\" HorizontalAlignment=\"Stretch\"";
        string thumb = orientation == Orientation.Vertical
            ? "MinHeight=\"40\" Margin=\"6,0\""
            : "MinWidth=\"40\" Margin=\"0,6\"";
        if (compact)
        {
            rail = "Width=\"6\" VerticalAlignment=\"Stretch\"";
            thumb = "MinHeight=\"40\" Margin=\"0\"";
        }

        string markup = $$$"""
            <ControlTemplate
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                TargetType="{x:Type ScrollBar}">
                <Grid Background="Transparent" SnapsToDevicePixels="True">
                    <Border {{{rail}}} Background="{DynamicResource Theme.Line}" CornerRadius="2" Opacity="0.34" />
                    <Track x:Name="PART_Track" IsDirectionReversed="{{{direction}}}" Orientation="{TemplateBinding Orientation}">
                        <Track.DecreaseRepeatButton>
                            <RepeatButton
                                Command="{x:Static ScrollBar.{{{decrease}}}}"
                                CommandTarget="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                                Focusable="False"
                                IsTabStop="False">
                                <RepeatButton.Template>
                                    <ControlTemplate TargetType="RepeatButton">
                                        <Border Background="Transparent" />
                                    </ControlTemplate>
                                </RepeatButton.Template>
                            </RepeatButton>
                        </Track.DecreaseRepeatButton>
                        <Track.Thumb>
                            <Thumb {{{thumb}}} Cursor="Hand">
                                <Thumb.Template>
                                    <ControlTemplate TargetType="Thumb">
                                        <Border
                                            x:Name="PSurface"
                                            Background="{DynamicResource Theme.Muted}"
                                            CornerRadius="8"
                                            Opacity="0.42" />
                                        <ControlTemplate.Triggers>
                                            <Trigger Property="IsMouseOver" Value="True">
                                                <Setter TargetName="PSurface" Property="Background" Value="{DynamicResource Theme.Accent}" />
                                                <Setter TargetName="PSurface" Property="Opacity" Value="0.62" />
                                            </Trigger>
                                            <Trigger Property="IsDragging" Value="True">
                                                <Setter TargetName="PSurface" Property="Background" Value="{DynamicResource Theme.Accent}" />
                                                <Setter TargetName="PSurface" Property="Opacity" Value="0.88" />
                                            </Trigger>
                                            <Trigger Property="IsEnabled" Value="False">
                                                <Setter TargetName="PSurface" Property="Opacity" Value="0.18" />
                                            </Trigger>
                                        </ControlTemplate.Triggers>
                                    </ControlTemplate>
                                </Thumb.Template>
                            </Thumb>
                        </Track.Thumb>
                        <Track.IncreaseRepeatButton>
                            <RepeatButton
                                Command="{x:Static ScrollBar.{{{increase}}}}"
                                CommandTarget="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                                Focusable="False"
                                IsTabStop="False">
                                <RepeatButton.Template>
                                    <ControlTemplate TargetType="RepeatButton">
                                        <Border Background="Transparent" />
                                    </ControlTemplate>
                                </RepeatButton.Template>
                            </RepeatButton>
                        </Track.IncreaseRepeatButton>
                    </Track>
                </Grid>
            </ControlTemplate>
            """;

        return (ControlTemplate)XamlReader.Parse(markup);
    }
}
