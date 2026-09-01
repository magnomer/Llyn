using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Llyn.UIShell;

internal static class PIndicator
{
    private const string PIndicatorVerticalKey = "Theme.ScrollBar.Vertical.Template";
    private const string PIndicatorHorizontalKey = "Theme.ScrollBar.Horizontal.Template";

    internal static void PIndicatorApply(ResourceDictionary resources)
    {
        resources[PIndicatorVerticalKey] = PIndicatorTemplateBuild(Orientation.Vertical);
        resources[PIndicatorHorizontalKey] = PIndicatorTemplateBuild(Orientation.Horizontal);
    }

    private static ControlTemplate PIndicatorTemplateBuild(Orientation orientation)
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
