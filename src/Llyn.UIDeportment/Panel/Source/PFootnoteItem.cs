using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PFootnoteItem : INotifyPropertyChanged
{
    private bool _pFootnoteItemChosen;

    internal PFootnoteItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pFootnoteItemChosen = chosen;
        PFootnoteItemId = id;
        PFootnoteItemHeadword = headword;
        PFootnoteItemEpithet = epithet ?? string.Empty;
        PFootnoteItemLanguage = language;
        PFootnoteItemFlag = LEnsignImage.LEnsignFind(language);
    }

    public long PFootnoteItemId { get; }

    public string PFootnoteItemHeadword { get; }

    public string PFootnoteItemEpithet { get; }

    public required string PFootnoteItemName { get; init; }

    public string PFootnoteItemLanguage { get; }

    public ImageSource? PFootnoteItemFlag { get; }

    internal static IReadOnlyList<PFootnoteItem> PFootnoteItemBuild(IReadOnlyList<LVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PFootnoteItem> built = new(rows.Count);
        foreach (LVistaRow row in rows)
        {
            built.Add(new PFootnoteItem(
                row.LVistaRowId,
                row.LVistaRowHeadword,
                row.LVistaRowLanguage,
                row.LVistaRowEpithet ?? string.Empty,
                row.LVistaRowChosen)
            {
                PFootnoteItemName = row.LVistaRowName,
            });
        }

        return built;
    }

    internal static bool PFootnoteItemMatch(PFootnoteItem held, PFootnoteItem fresh)
    {
        return held.PFootnoteItemId == fresh.PFootnoteItemId
            && string.Equals(held.PFootnoteItemHeadword, fresh.PFootnoteItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemEpithet, fresh.PFootnoteItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemName, fresh.PFootnoteItemName, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemLanguage, fresh.PFootnoteItemLanguage, StringComparison.Ordinal);
    }

    internal static void PFootnoteItemSync(PFootnoteItem held, PFootnoteItem fresh)
    {
        held.PFootnoteItemChosen = fresh.PFootnoteItemChosen;
    }

    internal static void PFootnoteItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PFootnoteItem footnote)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PFootnoteRow") is Button row)
        {
            if (footnote.PFootnoteItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<Image>(container, "PFootnoteFlag") is Image flag)
        {
            flag.Source = footnote.PFootnoteItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PFootnoteName") is Run name)
        {
            name.Text = footnote.PFootnoteItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PFootnoteEpithet") is Run epithet)
        {
            epithet.Text = " " + footnote.PFootnoteItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFootnoteLanguage") is TextBlock language)
        {
            language.Text = footnote.PFootnoteItemLanguage;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PFootnoteItemChosen
    {
        get => _pFootnoteItemChosen;

        set
        {
            if (_pFootnoteItemChosen == value)
            {
                return;
            }

            _pFootnoteItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PFootnoteItemChosen)));
        }
    }
}
