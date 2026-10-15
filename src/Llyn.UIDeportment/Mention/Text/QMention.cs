using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class QMention : TextBlock
{
    public static readonly DependencyProperty QMentionHostProperty = DependencyProperty.RegisterAttached(
        "QMentionHost",
        typeof(QMentionMenu),
        typeof(QMention),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty QMentionSentenceProperty = DependencyProperty.Register(
        nameof(QMentionSentence),
        typeof(long),
        typeof(QMention),
        new FrameworkPropertyMetadata(0L));

    public static readonly DependencyProperty QMentionPiecesProperty = DependencyProperty.Register(
        nameof(QMentionPieces),
        typeof(IReadOnlyList<QMentionPiece>),
        typeof(QMention),
        new FrameworkPropertyMetadata(null, QMentionChangeRefine));

    public static readonly RoutedEvent QMentionClickEvent = EventManager.RegisterRoutedEvent(
        nameof(QMentionClick),
        RoutingStrategy.Bubble,
        typeof(EventHandler<QMentionArgument>),
        typeof(QMention));

    private Point? _qMentionPress;

    public QMention()
    {
        SetResourceReference(FontFamilyProperty, "Theme.Card.ExampleFamily");
        SetResourceReference(FontSizeProperty, "Theme.Card.ExampleSize");
        TextWrapping = TextWrapping.Wrap;
    }

    public event EventHandler<QMentionArgument> QMentionClick
    {
        add => AddHandler(QMentionClickEvent, value);
        remove => RemoveHandler(QMentionClickEvent, value);
    }

    public long QMentionSentence
    {
        get => (long)GetValue(QMentionSentenceProperty);
        set => SetValue(QMentionSentenceProperty, value);
    }

    internal IReadOnlyList<QMentionPiece>? QMentionPieces
    {
        get => (IReadOnlyList<QMentionPiece>?)GetValue(QMentionPiecesProperty);
        set => SetValue(QMentionPiecesProperty, value);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _qMentionPress = e.GetPosition(this);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        Point? press = _qMentionPress;
        _qMentionPress = null;

        Point release = e.GetPosition(this);
        if (press is null
            || Math.Abs(release.X - press.Value.X) > SystemParameters.MinimumHorizontalDragDistance
            || Math.Abs(release.Y - press.Value.Y) > SystemParameters.MinimumVerticalDragDistance
            || QMentionRunFind(release) is not QMentionArgument click)
        {
            return;
        }

        e.Handled = true;
        RaiseEvent(click);
    }

    internal Rect QMentionPlaceRead(int? unit)
    {
        int before = 0;
        foreach (Inline inline in Inlines)
        {
            if (inline is not Run run || run.Tag is not QMentionPiece)
            {
                continue;
            }

            int inside = unit is int whole ? whole - before : -1;
            before += run.Text.Length;
            if (inside < 0 || inside >= run.Text.Length)
            {
                continue;
            }

            TextPointer pointer = run.ContentStart.GetPositionAtOffset(inside) ?? run.ContentStart;
            Rect found = pointer.GetCharacterRect(LogicalDirection.Forward);
            if (!found.IsEmpty)
            {
                return found;
            }
        }

        return new Rect(0, ActualHeight, 0, 0);
    }

    private static void QMentionChangeRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is QMention mention)
        {
            mention.QMentionShow();
        }
    }

    private QMentionMenu? QMentionHost => (QMentionMenu?)GetValue(QMentionHostProperty);

    private void QMentionShow()
    {
        Inlines.Clear();
        foreach (QMentionPiece piece in QMentionPieces ?? [])
        {
            Run run = new(piece.QMentionPieceText) { Tag = piece };

            QMentionStyleApply(run, piece.QMentionPieceLinked);

            Inlines.Add(run);
        }
    }

    private QMentionArgument? QMentionRunFind(Point point)
    {
        TextPointer? pointer;
        try
        {
            pointer = GetPositionFromPoint(point, true);
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        if (pointer?.Parent is not Run run)
        {
            return null;
        }

        if (run.Tag is not QMentionPiece)
        {
            return null;
        }

        if (QMentionHost is null)
        {
            return null;
        }

        StringBuilder shown = new();
        int clicked = run.ContentStart.GetOffsetToPosition(pointer);
        foreach (Inline inline in Inlines)
        {
            if (inline is not Run drawn || drawn.Tag is not QMentionPiece)
            {
                continue;
            }

            if (ReferenceEquals(drawn, run))
            {
                clicked += shown.Length;
            }

            shown.Append(drawn.Text);
        }

        return new QMentionArgument(QMentionClickEvent, this, shown.ToString(), clicked);
    }

    private static void QMentionStyleApply(Run run, bool? linked)
    {
        if (linked is true)
        {
            run.SetResourceReference(FrameworkContentElement.StyleProperty, "Theme.Mention.Linked");
        }
        else if (linked is false)
        {
            run.SetResourceReference(FrameworkContentElement.StyleProperty, "Theme.Mention.Silent");
        }
    }
}
