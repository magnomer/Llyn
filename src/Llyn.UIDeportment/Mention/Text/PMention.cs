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
        typeof(IReadOnlyList<CMention>),
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

    public IReadOnlyList<CMention>? PMentionMention
    {
        get => (IReadOnlyList<CMention>?)GetValue(PMentionMentionProperty);
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
        RaiseEvent(new PMentionArgument(PMentionClickEvent, this, offset));
    }

    internal void PMentionMarkShow(IReadOnlyList<PMentionMark> marks)
    {
        ArgumentNullException.ThrowIfNull(marks);

        List<CMention> mentions = new(marks.Count);
        foreach (PMentionMark mark in marks)
        {
            mentions.Add(new CMention(
                mark.PMentionMarkId,
                mark.PMentionMarkOffset,
                mark.PMentionMarkLength,
                mark.PMentionMarkEntry,
                mark.PMentionMarkSense));
        }

        PMentionMention = mentions;
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

            if (run.Tag is not CMentionPiece piece)
            {
                continue;
            }

            if (offset < piece.CMentionPieceOffset || offset >= piece.CMentionPieceEnd)
            {
                continue;
            }

            int unit = window.LWindowUnitRead(run.Text, offset - piece.CMentionPieceOffset);
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

        foreach (CMentionPiece piece in window.LWindowMentionDivide(text, PMentionMention ?? []))
        {
            Run run = new(piece.CMentionPieceText) { Tag = piece };

            PMentionStyleApply(run, piece.CMentionPieceLinked);

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

        if (run.Tag is not CMentionPiece piece)
        {
            return null;
        }

        if (PMentionWindow is not LWindow window)
        {
            return null;
        }

        int unit = run.ContentStart.GetOffsetToPosition(pointer);
        return PMentionOffsetRead(piece.CMentionPieceOffset, window.LWindowOffsetRead(run.Text, unit));
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
