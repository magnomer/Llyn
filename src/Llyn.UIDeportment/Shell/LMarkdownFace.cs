using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LMarkdownFace
{
    private static readonly FontFamily LMarkdownMono = new("Consolas");

    public static void LMarkdownShow(Panel target, string? markdown, LWindow window)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(window);

        target.Children.Clear();
        IReadOnlyList<LMarkdownBlock> blocks = window.LWindowMarkdownParse(markdown);
        int place = 0;
        foreach (LMarkdownBlock block in blocks)
        {
            FrameworkElement element = LMarkdownBlockBuild(block, window);
            element.Margin = new Thickness(0, place == 0 ? 0 : LMarkdownGapRead(block), 0, 0);
            target.Children.Add(element);
            place++;
        }
    }

    private static FrameworkElement LMarkdownBlockBuild(LMarkdownBlock block, LWindow window)
    {
        if (block.LMarkdownBlockHeaded)
        {
            return LMarkdownHeadingBuild(block, window);
        }

        if (block.LMarkdownBlockListed)
        {
            return LMarkdownItemBuild(block, window);
        }

        if (block.LMarkdownBlockQuoted)
        {
            return LMarkdownQuoteBuild(block, window);
        }

        if (block.LMarkdownBlockFenced)
        {
            return LMarkdownCodeBuild(block);
        }

        if (block.LMarkdownBlockRuled)
        {
            return LMarkdownRuleBuild();
        }

        return LMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window);
    }

    private static double LMarkdownGapRead(LMarkdownBlock block)
    {
        if (block.LMarkdownBlockHeaded)
        {
            return 14;
        }

        return block.LMarkdownBlockListed ? 3 : 10;
    }

    private static double LMarkdownIndentRead(int level)
    {
        return 22 * level;
    }

    private static TextBlock LMarkdownTextBuild(
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
            text.Inlines.Add(LMarkdownSpanBuild(span, window));
        }

        return text;
    }

    private static Inline LMarkdownSpanBuild(LMarkdownSpan span, LWindow window)
    {
        Run run = new Run(span.LMarkdownSpanText);

        if (span.LMarkdownSpanCode)
        {
            run.FontFamily = LMarkdownMono;
            run.Background = LMarkdownBrushRead("Theme.AccentSoft");
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
            Foreground = LMarkdownBrushRead("Theme.Accent"),
        };
        link.RequestNavigate += (_, e) =>
        {
            window.LWindowLocationOpen(e.Uri.AbsoluteUri);
            e.Handled = true;
        };
        return link;
    }

    private static TextBlock LMarkdownHeadingBuild(LMarkdownBlock block, LWindow window)
    {
        TextBlock text = LMarkdownTextBuild(block.LMarkdownBlockSpan, 15, window);
        text.FontWeight = FontWeights.SemiBold;
        return text;
    }

    private static Grid LMarkdownItemBuild(LMarkdownBlock block, LWindow window)
    {
        Grid row = new Grid { Margin = new Thickness(LMarkdownIndentRead(block.LMarkdownBlockLevel), 0, 0, 0) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
        row.ColumnDefinitions.Add(new ColumnDefinition());

        TextBlock lead = new TextBlock
        {
            Text = block.LMarkdownBlockMark,
            FontSize = 14,
            LineHeight = 21,
            Foreground = LMarkdownBrushRead("Theme.Muted"),
        };
        TextBlock body = LMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window);
        Grid.SetColumn(body, 1);

        row.Children.Add(lead);
        row.Children.Add(body);
        return row;
    }

    private static Border LMarkdownQuoteBuild(LMarkdownBlock block, LWindow window)
    {
        TextBlock text = LMarkdownTextBuild(block.LMarkdownBlockSpan, 14, window);
        text.Foreground = LMarkdownBrushRead("Theme.Muted");

        return new Border
        {
            BorderThickness = new Thickness(3, 0, 0, 0),
            BorderBrush = LMarkdownBrushRead("Theme.Line"),
            Padding = new Thickness(12, 0, 0, 0),
            Child = text,
        };
    }

    private static Border LMarkdownCodeBuild(LMarkdownBlock block)
    {
        return new Border
        {
            Background = LMarkdownBrushRead("Theme.AccentSoft"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 10, 12, 10),
            Child = new TextBlock
            {
                Text = block.LMarkdownBlockText,
                FontFamily = LMarkdownMono,
                FontSize = 13,
                LineHeight = 19,
                TextWrapping = TextWrapping.Wrap,
            },
        };
    }

    private static Border LMarkdownRuleBuild()
    {
        return new Border
        {
            Height = 1,
            Background = LMarkdownBrushRead("Theme.Line"),
        };
    }

    private static Brush LMarkdownBrushRead(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as Brush ?? Brushes.Gray;
    }
}
