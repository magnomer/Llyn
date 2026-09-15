using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PColophon : UserControl
{
    private PWindow _pColophonHost = null!;

    public PColophon()
    {
        InitializeComponent();
    }

    internal void PColophonAttach(PWindow host)
    {
        ArgumentNullException.ThrowIfNull(host);
        _pColophonHost = host;
    }

    internal void PColophonShow(LReference reference, IReadOnlyList<LAuthor> credits, string tally)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(credits);

        PColophonTitleShow(reference.LReferenceTitle);
        PColophonKindShow(reference.LReferenceKind);
        PColophonValueShow(PColophonYear, PColophonYearSection, reference.LReferenceYear);
        PColophonValueShow(PColophonUrl, PColophonUrlSection, reference.LReferenceUrl);
        PColophonValueShow(PColophonNote, PColophonNoteSection, reference.LReferenceNote);
        PColophonAuthorShow(reference, credits);
        PColophonTally.Text = tally;

        PColophonBody.Visibility = Visibility.Visible;
        PColophonUnselected.Visibility = Visibility.Collapsed;
    }

    internal void PColophonTallyShow(string tally)
    {
        PColophonTally.Text = tally;
    }

    internal void PColophonClear()
    {
        PColophonBody.Visibility = Visibility.Collapsed;
        PColophonUnselected.Visibility = Visibility.Visible;
    }

    private string? PColophonTextRead(LStateValue value)
    {
        return value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => _pColophonHost.PLocalizationTextRead("Display.Unknown"),
            _ => null,
        };
    }

    private void PColophonTitleShow(LStateValue value)
    {
        string? text = PColophonTextRead(value);

        PColophonTitle.Text = text ?? _pColophonHost.PLocalizationTextRead("Source.Untitled");
        PField.PFieldPlaceholderShow(PColophonTitle, text is null || value.LStateValueState == LState.LStateUnknown);
    }

    private void PColophonKindShow(LReferenceKind kind)
    {
        bool shown = kind != LReferenceKind.LReferenceKindUnspecified;

        PColophonKind.Text = shown
            ? _pColophonHost.PLocalizationTextRead(PReference.PReferenceKindRead(kind))
            : string.Empty;
        PColophonChip.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PColophonValueShow(TextBlock field, StackPanel section, LStateValue value)
    {
        string? text = PColophonTextRead(value);

        field.Text = text ?? string.Empty;
        PField.PFieldPlaceholderShow(
            field, value.LStateValueState == LState.LStateUnknown && !value.LStateValueUnreadable);
        section.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PColophonAuthorShow(LReference reference, IReadOnlyList<LAuthor> credits)
    {
        bool shown = credits.Count > 0
            || reference.LReferenceAuthorState.LStateMarkState == LState.LStateUnknown;

        PColophonAuthor.Text = shown
            ? PShelfItem.PShelfCreditRead(
                reference,
                credits,
                _pColophonHost.PLocalizationTextRead("Display.Unknown"),
                string.Empty)
            : string.Empty;
        PField.PFieldPlaceholderShow(PColophonAuthor, shown && credits.Count == 0);
        PColophonAuthorSection.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
    }
}
