using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PMention : TextBlock
{
    public static readonly DependencyProperty PMentionHostProperty = DependencyProperty.RegisterAttached(
        "PMentionHost",
        typeof(QWindow),
        typeof(PMention),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty PMentionSentenceProperty = DependencyProperty.Register(
        nameof(PMentionSentence),
        typeof(long),
        typeof(PMention),
        new FrameworkPropertyMetadata(0L));

    public static readonly DependencyProperty PMentionPieceProperty = DependencyProperty.Register(
        nameof(PMentionPiece),
        typeof(IReadOnlyList<QMentionPiece>),
        typeof(PMention),
        new FrameworkPropertyMetadata(null, PMentionChangeRefine));

    public static readonly RoutedEvent PMentionClickEvent = EventManager.RegisterRoutedEvent(
        nameof(PMentionClick),
        RoutingStrategy.Bubble,
        typeof(EventHandler<PMentionArgument>),
        typeof(PMention));

    private Point? _pMentionPress;

    public PMention()
    {
        SetResourceReference(FontFamilyProperty, "Theme.Card.ExampleFamily");
        SetResourceReference(FontSizeProperty, "Theme.Card.ExampleSize");
        TextWrapping = TextWrapping.Wrap;
    }

    public event EventHandler<PMentionArgument> PMentionClick
    {
        add => AddHandler(PMentionClickEvent, value);
        remove => RemoveHandler(PMentionClickEvent, value);
    }

    public long PMentionSentence
    {
        get => (long)GetValue(PMentionSentenceProperty);
        set => SetValue(PMentionSentenceProperty, value);
    }

    internal IReadOnlyList<QMentionPiece>? PMentionPiece
    {
        get => (IReadOnlyList<QMentionPiece>?)GetValue(PMentionPieceProperty);
        set => SetValue(PMentionPieceProperty, value);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _pMentionPress = e.GetPosition(this);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        Point? press = _pMentionPress;
        _pMentionPress = null;

        Point release = e.GetPosition(this);
        int? clicked = PMentionOffsetRead(release);
        if (press is null
            || Math.Abs(release.X - press.Value.X) > SystemParameters.MinimumHorizontalDragDistance
            || Math.Abs(release.Y - press.Value.Y) > SystemParameters.MinimumVerticalDragDistance
            || clicked is not int offset)
        {
            return;
        }

        e.Handled = true;
        RaiseEvent(new PMentionArgument(PMentionClickEvent, this, offset));
    }

    internal Rect PMentionPieceRead(int offset)
    {
        if (PMentionHost is not QWindow host)
        {
            return new Rect(0, ActualHeight, 0, 0);
        }

        foreach (Inline inline in Inlines)
        {
            if (inline is not Run run)
            {
                continue;
            }

            if (run.Tag is not QMentionPiece piece)
            {
                continue;
            }

            if (host.QWindowAtelier.CAtelierMention.CMentionUnitRead(
                    run.Text, piece.QMentionPieceOffset, offset) is not int unit)
            {
                continue;
            }

            TextPointer pointer = run.ContentStart.GetPositionAtOffset(unit) ?? run.ContentStart;
            Rect found = pointer.GetCharacterRect(LogicalDirection.Forward);
            if (!found.IsEmpty)
            {
                return found;
            }
        }

        return new Rect(0, ActualHeight, 0, 0);
    }

    private static void PMentionChangeRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is PMention mention)
        {
            mention.PMentionShow();
        }
    }

    private QWindow? PMentionHost => (QWindow?)GetValue(PMentionHostProperty);

    private void PMentionShow()
    {
        Inlines.Clear();
        foreach (QMentionPiece piece in PMentionPiece ?? [])
        {
            Run run = new(piece.QMentionPieceText) { Tag = piece };

            PMentionStyleApply(run, piece.QMentionPieceLinked);

            Inlines.Add(run);
        }
    }

    private int? PMentionOffsetRead(Point point)
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

        if (run.Tag is not QMentionPiece piece)
        {
            return null;
        }

        if (PMentionHost is not QWindow host)
        {
            return null;
        }

        int unit = run.ContentStart.GetOffsetToPosition(pointer);
        return PMentionOffsetRead(
            piece.QMentionPieceOffset, host.QWindowAtelier.CAtelierMention.CMentionOffsetRead(run.Text, unit));
    }

    private static void PMentionStyleApply(Run run, bool? linked)
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

    private static int PMentionOffsetRead(int start, int offset)
    {
        return start + offset;
    }
}
