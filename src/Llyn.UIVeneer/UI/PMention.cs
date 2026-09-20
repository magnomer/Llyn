using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public sealed class PMention : TextBlock
{
    public static readonly DependencyProperty PMentionWindowProperty = DependencyProperty.RegisterAttached(
        "PMentionWindow",
        typeof(LWindow),
        typeof(PMention),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, PMentionChangeHandle));

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

    protected override void OnQueryCursor(QueryCursorEventArgs e)
    {
        base.OnQueryCursor(e);

        LMentionPiece? piece = PMentionOffsetRead(e.GetPosition(this)) is int offset
            ? PMentionPieceFind(offset)
            : null;

        if (piece is { LMentionPieceLinked: true })
        {
            e.Cursor = Cursors.Hand;
            e.Handled = true;
        }
    }

    internal Rect PMentionPieceRead(int offset)
    {
        if (PMentionWindow is not LWindow window)
        {
            return new Rect(0, ActualHeight, 0, 0);
        }

        foreach (Inline inline in Inlines)
        {
            if (inline is not Run run)
            {
                continue;
            }

            if (run.Tag is not LMentionPiece piece)
            {
                continue;
            }

            if (offset < piece.LMentionPieceOffset || offset >= piece.LMentionPieceEnd)
            {
                continue;
            }

            int unit = window.LWindowUnitRead(run.Text, offset - piece.LMentionPieceOffset);
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
        if (sender is PMention mention)
        {
            mention.PMentionShow();
        }
    }

    private LWindow? PMentionWindow => (LWindow?)GetValue(PMentionWindowProperty);

    private void PMentionShow()
    {
        string text = PMentionText;
        Inlines.Clear();
        if (PMentionWindow is not LWindow window)
        {
            Inlines.Add(new Run(text));
            return;
        }

        _pMentionPiece = window.LWindowMentionDivide(text, PMentionMention ?? []);
        foreach (LMentionPiece piece in _pMentionPiece)
        {
            int start = window.LWindowUnitRead(text, piece.LMentionPieceOffset);
            int end = window.LWindowUnitRead(text, piece.LMentionPieceEnd);
            Run run = new(text[start..end]) { Tag = piece };

            if (piece.LMentionPieceStored is LMention stored)
            {
                run.SetResourceReference(
                    FrameworkContentElement.StyleProperty,
                    stored.LMentionLinked ? "Theme.Mention.Linked" : "Theme.Mention.Silent");
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

        if (pointer?.Parent is not Run run)
        {
            return null;
        }

        if (run.Tag is not LMentionPiece piece)
        {
            return null;
        }

        if (PMentionWindow is not LWindow window)
        {
            return null;
        }

        int unit = run.ContentStart.GetOffsetToPosition(pointer);
        return piece.LMentionPieceOffset + window.LWindowOffsetRead(run.Text, unit);
    }

    private LMentionPiece? PMentionPieceFind(int offset)
    {
        foreach (LMentionPiece piece in _pMentionPiece)
        {
            if (offset >= piece.LMentionPieceOffset && offset < piece.LMentionPieceEnd)
            {
                return piece;
            }
        }

        return null;
    }
}
