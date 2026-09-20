using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal static class PMarkdown
{
    private static readonly FontFamily PMarkdownMono = new("Consolas");

    internal static void PMarkdownShow(Panel target, string? markdown, LWindow window)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(window);

        target.Children.Clear();
        IReadOnlyList<LMarkdownBlock> blocks = window.LWindowMarkdownParse(markdown);
        int place = 0;
        foreach (LMarkdownBlock block in blocks)
        {
            FrameworkElement element = block.LMarkdownBlockKind switch
            {
                LMarkdownKind.LMarkdownKindHeading => PMarkdownHeadingBuild(block, window),
                LMarkdownKind.LMarkdownKindBullet => PMarkdownItemBuild(block, "•", window),
                LMarkdownKind.LMarkdownKindNumber => PMarkdownItemBuild(
                    block, block.LMarkdownBlockOrdinal.ToString(CultureInfo.InvariantCulture) + ".", window),
                LMarkdownKind.LMarkdownKindQuote => PMarkdownQuoteBuild(block, window),
                LMarkdownKind.LMarkdownKindCode => PMarkdownCodeBuild(block),
                LMarkdownKind.LMarkdownKindRule => PMarkdownRuleBuild(),
                _ => PMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window),
            };

            element.Margin = new Thickness(0, place == 0 ? 0 : PMarkdownGapRead(block), 0, 0);
            target.Children.Add(element);
            place++;
        }
    }

    private static double PMarkdownGapRead(LMarkdownBlock block)
    {
        return block.LMarkdownBlockKind switch
        {
            LMarkdownKind.LMarkdownKindHeading => 14,
            LMarkdownKind.LMarkdownKindBullet or LMarkdownKind.LMarkdownKindNumber => 3,
            _ => 10,
        };
    }

    private static TextBlock PMarkdownTextBuild(
        IReadOnlyList<LMarkdownSpan> spans, double size, LWindow window)
    {
        TextBlock text = new TextBlock
        {
            FontSize = size,
            LineHeight = size * 1.5,
            TextWrapping = TextWrapping.Wrap,
        };

        foreach (LMarkdownSpan span in spans)
        {
            text.Inlines.Add(PMarkdownSpanBuild(span, window));
        }

        return text;
    }

    private static Inline PMarkdownSpanBuild(LMarkdownSpan span, LWindow window)
    {
        Run run = new Run(span.LMarkdownSpanText);

        if (span.LMarkdownSpanCode)
        {
            run.FontFamily = PMarkdownMono;
            run.Background = PMarkdownBrushRead("Theme.AccentSoft");
            return run;
        }

        if (span.LMarkdownSpanBold)
        {
            run.FontWeight = FontWeights.SemiBold;
        }

        if (span.LMarkdownSpanItalic)
        {
            run.FontStyle = FontStyles.Italic;
        }

        if (span.LMarkdownSpanAddress is not Uri target)
        {
            return run;
        }

        Hyperlink link = new Hyperlink(run)
        {
            NavigateUri = target,
            Foreground = PMarkdownBrushRead("Theme.Accent"),
        };
        link.RequestNavigate += (_, e) =>
        {
            window.LWindowLocationOpen(e.Uri.AbsoluteUri);
            e.Handled = true;
        };
        return link;
    }

    private static TextBlock PMarkdownHeadingBuild(LMarkdownBlock block, LWindow window)
    {
        TextBlock text = PMarkdownTextBuild(block.LMarkdownBlockSpan, 15, window);
        text.FontWeight = FontWeights.SemiBold;
        return text;
    }

    private static Grid PMarkdownItemBuild(LMarkdownBlock block, string mark, LWindow window)
    {
        Grid row = new Grid { Margin = new Thickness(22 * block.LMarkdownBlockLevel, 0, 0, 0) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
        row.ColumnDefinitions.Add(new ColumnDefinition());

        TextBlock lead = new TextBlock
        {
            Text = mark,
            FontSize = 14,
            LineHeight = 21,
            Foreground = PMarkdownBrushRead("Theme.Muted"),
        };
        TextBlock body = PMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window);
        Grid.SetColumn(body, 1);

        row.Children.Add(lead);
        row.Children.Add(body);
        return row;
    }

    private static Border PMarkdownQuoteBuild(LMarkdownBlock block, LWindow window)
    {
        TextBlock text = PMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window);
        text.Foreground = PMarkdownBrushRead("Theme.Muted");

        return new Border
        {
            BorderThickness = new Thickness(3, 0, 0, 0),
            BorderBrush = PMarkdownBrushRead("Theme.Line"),
            Padding = new Thickness(12, 0, 0, 0),
            Child = text,
        };
    }

    private static Border PMarkdownCodeBuild(LMarkdownBlock block)
    {
        return new Border
        {
            Background = PMarkdownBrushRead("Theme.AccentSoft"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 10, 12, 10),
            Child = new TextBlock
            {
                Text = block.LMarkdownBlockText,
                FontFamily = PMarkdownMono,
                FontSize = 13,
                LineHeight = 19,
                TextWrapping = TextWrapping.Wrap,
            },
        };
    }

    private static Border PMarkdownRuleBuild()
    {
        return new Border
        {
            Height = 1,
            Background = PMarkdownBrushRead("Theme.Line"),
        };
    }

    private static Brush PMarkdownBrushRead(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as Brush ?? Brushes.Gray;
    }
}
