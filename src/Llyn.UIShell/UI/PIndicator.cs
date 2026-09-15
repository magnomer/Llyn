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
    private const string PIndicatorLaneSize = "14";
    private const string PIndicatorRailSize = "10";
    private const string PIndicatorRailRadius = "5";
    private const string PIndicatorThumbLength = "40";

    internal static void PIndicatorApply(ResourceDictionary resources)
    {
        resources[PIndicatorVerticalKey] = PIndicatorTemplateBuild(Orientation.Vertical);
        resources[PIndicatorHorizontalKey] = PIndicatorTemplateBuild(Orientation.Horizontal);
        resources[PIndicatorGutterKey] = PIndicatorGutterBuild();
    }

    private static ControlTemplate PIndicatorGutterBuild()
    {
        string markup = $$$"""
            <ControlTemplate
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                TargetType="{x:Type ScrollViewer}">
                <Grid Background="{TemplateBinding Background}">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition x:Name="PScrollColumn" Width="0" />
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="*" />
                        <RowDefinition x:Name="PScrollRow" Height="0" />
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
                        Width="{{{PIndicatorLaneSize}}}"
                        MinWidth="{{{PIndicatorLaneSize}}}"
                        Margin="0"
                        HorizontalAlignment="Right"
                        Template="{DynamicResource {{{PIndicatorVerticalKey}}}}"
                        Maximum="{TemplateBinding ScrollableHeight}"
                        Orientation="Vertical"
                        ViewportSize="{TemplateBinding ViewportHeight}"
                        Visibility="{TemplateBinding ComputedVerticalScrollBarVisibility}"
                        Value="{Binding VerticalOffset, Mode=OneWay,
                                        RelativeSource={RelativeSource TemplatedParent}}" />
                    <ScrollBar
                        x:Name="PART_HorizontalScrollBar"
                        Grid.Row="1"
                        Grid.Column="0"
                        Height="{{{PIndicatorLaneSize}}}"
                        MinHeight="{{{PIndicatorLaneSize}}}"
                        Margin="0"
                        VerticalAlignment="Bottom"
                        Template="{DynamicResource {{{PIndicatorHorizontalKey}}}}"
                        Maximum="{TemplateBinding ScrollableWidth}"
                        Orientation="Horizontal"
                        ViewportSize="{TemplateBinding ViewportWidth}"
                        Visibility="{TemplateBinding ComputedHorizontalScrollBarVisibility}"
                        Value="{Binding HorizontalOffset, Mode=OneWay,
                                        RelativeSource={RelativeSource TemplatedParent}}" />
                </Grid>
                <ControlTemplate.Triggers>
                    <Trigger Property="ComputedVerticalScrollBarVisibility" Value="Visible">
                        <Setter TargetName="PScrollColumn" Property="Width" Value="{{{PIndicatorLaneSize}}}" />
                    </Trigger>
                    <Trigger Property="ComputedHorizontalScrollBarVisibility" Value="Visible">
                        <Setter TargetName="PScrollRow" Property="Height" Value="{{{PIndicatorLaneSize}}}" />
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
            """;

        return (ControlTemplate)XamlReader.Parse(markup);
    }

    private static ControlTemplate PIndicatorTemplateBuild(Orientation orientation)
    {
        string direction = orientation == Orientation.Vertical ? "True" : "False";
        string decrease = orientation == Orientation.Vertical ? "PageUpCommand" : "PageLeftCommand";
        string increase = orientation == Orientation.Vertical ? "PageDownCommand" : "PageRightCommand";
        string inset = orientation == Orientation.Vertical ? "4,0,0,0" : "0,4,0,0";
        string thumb = orientation == Orientation.Vertical
            ? $"MinHeight=\"{PIndicatorThumbLength}\""
            : $"MinWidth=\"{PIndicatorThumbLength}\"";
        string rail = orientation == Orientation.Vertical
            ? $"Width=\"{PIndicatorRailSize}\" HorizontalAlignment=\"Right\" VerticalAlignment=\"Stretch\""
            : $"Height=\"{PIndicatorRailSize}\" VerticalAlignment=\"Bottom\" HorizontalAlignment=\"Stretch\"";

        string markup = $$$"""
            <ControlTemplate
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                TargetType="{x:Type ScrollBar}">
                <Grid Margin="{{{inset}}}" Background="Transparent" SnapsToDevicePixels="True">
                    <Border
                        {{{rail}}}
                        Background="{DynamicResource Theme.Line}"
                        CornerRadius="{{{PIndicatorRailRadius}}}"
                        Opacity="0.34" />
                    <Track
                        x:Name="PART_Track"
                        IsDirectionReversed="{{{direction}}}"
                        Orientation="{TemplateBinding Orientation}">
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
                            <Thumb {{{thumb}}} Margin="0" Cursor="Hand">
                                <Thumb.Template>
                                    <ControlTemplate TargetType="Thumb">
                                        <Border
                                            x:Name="PSurface"
                                            {{{rail}}}
                                            Background="{DynamicResource Theme.Muted}"
                                            CornerRadius="{{{PIndicatorRailRadius}}}"
                                            Opacity="0.42" />
                                        <ControlTemplate.Triggers>
                                            <Trigger Property="IsMouseOver" Value="True">
                                                <Setter
                                                    TargetName="PSurface"
                                                    Property="Background"
                                                    Value="{DynamicResource Theme.Accent}" />
                                                <Setter TargetName="PSurface" Property="Opacity" Value="0.62" />
                                            </Trigger>
                                            <Trigger Property="IsDragging" Value="True">
                                                <Setter
                                                    TargetName="PSurface"
                                                    Property="Background"
                                                    Value="{DynamicResource Theme.Accent}" />
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
