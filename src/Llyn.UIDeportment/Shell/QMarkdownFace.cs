using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public static class QMarkdownFace
{
    private static readonly FontFamily QMarkdownMono = new("Consolas");

    public static void QMarkdownRefine(Panel target, IReadOnlyList<CMarkdownBlock> blocks, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(atelier);

        target.Children.Clear();
        int place = 0;
        foreach (CMarkdownBlock block in blocks)
        {
            FrameworkElement element = QMarkdownBlockBuild(block, atelier);
            element.Margin = new Thickness(0, place == 0 ? 0 : QMarkdownGapRead(block), 0, 0);
            target.Children.Add(element);
            place++;
        }
    }

    public static void QMarkdownRefine(Panel target, string text)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.Children.Clear();
        TextBlock plain = QMarkdownPlainBuild(14);
        plain.Text = text;
        target.Children.Add(plain);
    }

    private static FrameworkElement QMarkdownBlockBuild(CMarkdownBlock block, CAtelier atelier)
    {
        if (block.CMarkdownBlockHeaded)
        {
            return QMarkdownHeadingBuild(block, atelier);
        }

        if (block.CMarkdownBlockListed)
        {
            return QMarkdownItemBuild(block, atelier);
        }

        if (block.CMarkdownBlockQuoted)
        {
            return QMarkdownQuoteBuild(block, atelier);
        }

        if (block.CMarkdownBlockFenced)
        {
            return QMarkdownCodeBuild(block);
        }

        if (block.CMarkdownBlockRuled)
        {
            return QMarkdownRuleBuild();
        }

        return QMarkdownTextBuild(block.CMarkdownBlockSpan, 14, atelier);
    }

    private static double QMarkdownGapRead(CMarkdownBlock block)
    {
        if (block.CMarkdownBlockHeaded)
        {
            return 14;
        }

        return block.CMarkdownBlockListed ? 3 : 10;
    }

    private static double QMarkdownIndentRead(int level)
    {
        return 22 * level;
    }

    private static TextBlock QMarkdownPlainBuild(double size)
    {
        return new TextBlock
        {
            FontSize = size,
            LineHeight = size * 1.5,
            TextWrapping = TextWrapping.Wrap,
        };
    }

    private static TextBlock QMarkdownTextBuild(
        IReadOnlyList<CMarkdownSpan> spans, double size, CAtelier atelier)
    {
        TextBlock text = QMarkdownPlainBuild(size);
        foreach (CMarkdownSpan span in spans)
        {
            text.Inlines.Add(QMarkdownSpanBuild(span, atelier));
        }

        return text;
    }

    private static Inline QMarkdownSpanBuild(CMarkdownSpan span, CAtelier atelier)
    {
        Run run = new Run(span.CMarkdownSpanText);

        if (span.CMarkdownSpanCode)
        {
            run.FontFamily = QMarkdownMono;
            run.Background = QMarkdownBrushRead("Theme.AccentSoft");
            return run;
        }

        if (span.CMarkdownSpanBold)
        {
            run.FontWeight = FontWeights.SemiBold;
        }

        if (span.CMarkdownSpanItalic)
        {
            run.FontStyle = FontStyles.Italic;
        }

        if (span.CMarkdownSpanAddress is not Uri target)
        {
            return run;
        }

        Hyperlink link = new Hyperlink(run)
        {
            NavigateUri = target,
            Foreground = QMarkdownBrushRead("Theme.Accent"),
        };
        link.RequestNavigate += (_, e) => QMarkdownLinkObserve(e, atelier);
        return link;
    }

    private static void QMarkdownLinkObserve(RequestNavigateEventArgs e, CAtelier atelier)
    {
        atelier.CAtelierLocationOpen(e.Uri.AbsoluteUri);
        e.Handled = true;
    }

    private static TextBlock QMarkdownHeadingBuild(CMarkdownBlock block, CAtelier atelier)
    {
        TextBlock text = QMarkdownTextBuild(block.CMarkdownBlockSpan, 15, atelier);
        text.FontWeight = FontWeights.SemiBold;
        return text;
    }

    private static Grid QMarkdownItemBuild(CMarkdownBlock block, CAtelier atelier)
    {
        Grid row = new Grid { Margin = new Thickness(QMarkdownIndentRead(block.CMarkdownBlockLevel), 0, 0, 0) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
        row.ColumnDefinitions.Add(new ColumnDefinition());

        TextBlock lead = new TextBlock
        {
            Text = block.CMarkdownBlockMark,
            FontSize = 14,
            LineHeight = 21,
            Foreground = QMarkdownBrushRead("Theme.Muted"),
        };
        TextBlock body = QMarkdownTextBuild(block.CMarkdownBlockSpan, 14, atelier);
        Grid.SetColumn(body, 1);

        row.Children.Add(lead);
        row.Children.Add(body);
        return row;
    }

    private static Border QMarkdownQuoteBuild(CMarkdownBlock block, CAtelier atelier)
    {
        TextBlock text = QMarkdownTextBuild(block.CMarkdownBlockSpan, 14, atelier);
        text.Foreground = QMarkdownBrushRead("Theme.Muted");

        return new Border
        {
            BorderThickness = new Thickness(3, 0, 0, 0),
            BorderBrush = QMarkdownBrushRead("Theme.Line"),
            Padding = new Thickness(12, 0, 0, 0),
            Child = text,
        };
    }

    private static Border QMarkdownCodeBuild(CMarkdownBlock block)
    {
        return new Border
        {
            Background = QMarkdownBrushRead("Theme.AccentSoft"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 10, 12, 10),
            Child = new TextBlock
            {
                Text = block.CMarkdownBlockText,
                FontFamily = QMarkdownMono,
                FontSize = 13,
                LineHeight = 19,
                TextWrapping = TextWrapping.Wrap,
            },
        };
    }

    private static Border QMarkdownRuleBuild()
    {
        return new Border
        {
            Height = 1,
            Background = QMarkdownBrushRead("Theme.Line"),
        };
    }

    private static Brush QMarkdownBrushRead(string key)
    {
        return System.Windows.Application.Current?.TryFindResource(key) as Brush ?? Brushes.Gray;
    }
}
