using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIVeneer;

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
        PColophonKindShow(reference);
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
        return value.LStateValueUncertain
            ? PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")
            : value.LStateValueShown;
    }

    private void PColophonTitleShow(LStateValue value)
    {
        string? text = PColophonTextRead(value);

        PColophonTitle.Text = text ?? PLocalizationCatalog.PLocalizationTextRead("Source.Untitled");
        PField.PFieldPlaceholderShow(PColophonTitle, text is null || value.LStateValueUncertain);
    }

    private void PColophonKindShow(LReference reference)
    {
        PColophonKind.Text = reference.LReferenceKindCheck()
            ? PLocalizationCatalog.PLocalizationTextRead(PReference.PReferenceKindRead(reference.LReferenceKind))
            : string.Empty;
        PColophonChip.Visibility = reference.LReferenceKindCheck() ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PColophonValueShow(TextBlock field, StackPanel section, LStateValue value)
    {
        string? text = PColophonTextRead(value);

        field.Text = text ?? string.Empty;
        PField.PFieldPlaceholderShow(field, value.LStateValueUncertain);
        section.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PColophonAuthorShow(LReference reference, IReadOnlyList<LAuthor> credits)
    {
        PColophonAuthor.Text = reference.LReferenceAuthorCheck(credits)
            ? PShelfItem.PShelfCreditRead(
                reference,
                credits,
                PLocalizationCatalog.PLocalizationTextRead("Display.Unknown"),
                string.Empty)
            : string.Empty;
        PField.PFieldPlaceholderShow(
            PColophonAuthor,
            reference.LReferenceCreditRead(credits) is null
                ? reference.LReferenceAuthorState.LStateMarkUncertain
                : false);
        PColophonAuthorSection.Visibility = reference.LReferenceAuthorCheck(credits)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
