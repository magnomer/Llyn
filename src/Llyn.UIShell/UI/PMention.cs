using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PMention : TextBlock
{
    public static readonly DependencyProperty PMentionTextProperty = DependencyProperty.Register(
        nameof(PMentionText),
        typeof(string),
        typeof(PMention),
        new FrameworkPropertyMetadata(string.Empty, PMentionChangeHandle));

    public static readonly DependencyProperty PMentionMentionProperty = DependencyProperty.Register(
        nameof(PMentionMention),
        typeof(IReadOnlyList<LMention>),
        typeof(PMention),
        new FrameworkPropertyMetadata(null, PMentionChangeHandle));

    public static readonly DependencyProperty PMentionLanguageProperty = DependencyProperty.Register(
        nameof(PMentionLanguage),
        typeof(string),
        typeof(PMention),
        new FrameworkPropertyMetadata(string.Empty));

    public static readonly RoutedEvent PMentionClickEvent = EventManager.RegisterRoutedEvent(
        nameof(PMentionClick),
        RoutingStrategy.Bubble,
        typeof(EventHandler<PMentionArgument>),
        typeof(PMention));

    private IReadOnlyList<LMentionPiece> _pMentionPiece = [];

    private Point? _pMentionPress;

    public PMention()
    {
        SetResourceReference(FontFamilyProperty, "Theme.Card.ExampleFamily");
        SetResourceReference(FontSizeProperty, "Theme.Card.ExampleSize");
        TextWrapping = TextWrapping.Wrap;

        AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(PMentionPressHandle), true);
        AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(PMentionReleaseHandle), true);
        AddHandler(QueryCursorEvent, new QueryCursorEventHandler(PMentionCursorHandle), true);
    }

    public event EventHandler<PMentionArgument> PMentionClick
    {
        add => AddHandler(PMentionClickEvent, value);
        remove => RemoveHandler(PMentionClickEvent, value);
    }

    public string PMentionText
    {
        get => (string?)GetValue(PMentionTextProperty) ?? string.Empty;
        set => SetValue(PMentionTextProperty, value);
    }

    public IReadOnlyList<LMention>? PMentionMention
    {
        get => (IReadOnlyList<LMention>?)GetValue(PMentionMentionProperty);
        set => SetValue(PMentionMentionProperty, value);
    }

    public string PMentionLanguage
    {
        get => (string?)GetValue(PMentionLanguageProperty) ?? string.Empty;
        set => SetValue(PMentionLanguageProperty, value);
    }

    private void PMentionPressHandle(object sender, MouseButtonEventArgs e)
    {
        _pMentionPress = e.GetPosition(this);
    }

    private void PMentionReleaseHandle(object sender, MouseButtonEventArgs e)
    {
        Point? press = _pMentionPress;
        _pMentionPress = null;

        Point release = e.GetPosition(this);
        if (press is null
            || Math.Abs(release.X - press.Value.X) > SystemParameters.MinimumHorizontalDragDistance
            || Math.Abs(release.Y - press.Value.Y) > SystemParameters.MinimumVerticalDragDistance
            || PMentionOffsetRead(release) is not int offset)
        {
            return;
        }

        e.Handled = true;
        RaiseEvent(new PMentionArgument(PMentionClickEvent, this, offset, PMentionPieceFind(offset)));
    }

    private void PMentionCursorHandle(object sender, QueryCursorEventArgs e)
    {
        if (IsMouseCaptured)
        {
            return;
        }

        LMentionPiece? piece = PMentionOffsetRead(e.GetPosition(this)) is int offset
            ? PMentionPieceFind(offset)
            : null;

        if (piece?.LMentionPieceStored is { LMentionEntryId: not 0 })
        {
            e.Cursor = Cursors.Hand;
            e.Handled = true;
        }
    }

    internal Rect PMentionPieceRead(int offset)
    {
        foreach (Inline inline in Inlines)
        {
            if (inline is not Run { Tag: LMentionPiece piece } run
                || offset < piece.LMentionPieceOffset
                || offset >= piece.LMentionPieceOffset + piece.LMentionPieceLength)
            {
                continue;
            }

            int unit = LMentionSpan.LMentionUnitRead(run.Text, offset - piece.LMentionPieceOffset);
            TextPointer pointer = run.ContentStart.GetPositionAtOffset(unit) ?? run.ContentStart;
            Rect found = pointer.GetCharacterRect(LogicalDirection.Forward);
            if (!found.IsEmpty)
            {
                return found;
            }
        }

        return new Rect(0, ActualHeight, 0, 0);
    }

    private static void PMentionChangeHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ((PMention)sender).PMentionShow();
    }

    private void PMentionShow()
    {
        string text = PMentionText;
        _pMentionPiece = LMentionSpan.LMentionSpanDivide(text, PMentionMention ?? []);

        Inlines.Clear();
        foreach (LMentionPiece piece in _pMentionPiece)
        {
            int start = LMentionSpan.LMentionUnitRead(text, piece.LMentionPieceOffset);
            int end = LMentionSpan.LMentionUnitRead(text, piece.LMentionPieceOffset + piece.LMentionPieceLength);
            Run run = new(text[start..end]) { Tag = piece };

            if (piece.LMentionPieceStored is LMention stored)
            {
                run.SetResourceReference(
                    FrameworkContentElement.StyleProperty,
                    stored.LMentionEntryId == 0 ? "Theme.Mention.Silent" : "Theme.Mention.Linked");
            }

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

        if (pointer?.Parent is not Run run || run.Tag is not LMentionPiece piece)
        {
            return null;
        }

        int unit = run.ContentStart.GetOffsetToPosition(pointer);
        return piece.LMentionPieceOffset + LMentionSpan.LMentionOffsetRead(run.Text, unit);
    }

    private LMentionPiece? PMentionPieceFind(int offset)
    {
        foreach (LMentionPiece piece in _pMentionPiece)
        {
            if (offset >= piece.LMentionPieceOffset && offset < piece.LMentionPieceOffset + piece.LMentionPieceLength)
            {
                return piece;
            }
        }

        return null;
    }
}
