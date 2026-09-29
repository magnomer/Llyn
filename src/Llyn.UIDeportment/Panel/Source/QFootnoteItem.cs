using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QFootnoteItem : INotifyPropertyChanged
{
    private bool _qFootnoteItemChosen;

    internal QFootnoteItem(CVistaRow row, bool chosen)
    {
        _qFootnoteItemChosen = chosen;
        QFootnoteItemId = row.CVistaRowId;
        QFootnoteItemHeadword = row.CVistaRowHeadword;
        QFootnoteItemEpithet = row.CVistaRowEpithet;
        QFootnoteItemName = row.CVistaRowName;
        QFootnoteItemLanguage = row.CVistaRowLanguage;
        QFootnoteItemFlag = LEnsignImage.LEnsignFind(row.CVistaRowLanguage);
    }

    public long QFootnoteItemId { get; }

    public string QFootnoteItemHeadword { get; }

    public string QFootnoteItemEpithet { get; }

    public string QFootnoteItemName { get; }

    public string QFootnoteItemLanguage { get; }

    public ImageSource? QFootnoteItemFlag { get; }

    internal static IReadOnlyList<QFootnoteItem> QFootnoteItemBuild(IReadOnlyList<CVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QFootnoteItem> built = new(rows.Count);
        foreach (CVistaRow row in rows)
        {
            built.Add(new QFootnoteItem(row, row.CVistaRowChosen));
        }

        return built;
    }

    internal static bool QFootnoteItemMatch(QFootnoteItem held, QFootnoteItem fresh)
    {
        return held.QFootnoteItemId == fresh.QFootnoteItemId
            && string.Equals(held.QFootnoteItemHeadword, fresh.QFootnoteItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QFootnoteItemEpithet, fresh.QFootnoteItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QFootnoteItemName, fresh.QFootnoteItemName, StringComparison.Ordinal)
            && string.Equals(held.QFootnoteItemLanguage, fresh.QFootnoteItemLanguage, StringComparison.Ordinal);
    }

    internal static void QFootnoteItemSync(QFootnoteItem held, QFootnoteItem fresh)
    {
        held.QFootnoteItemChosen = fresh.QFootnoteItemChosen;
    }

    internal static void QFootnoteItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QFootnoteItem footnote)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PFootnoteRow") is Button row)
        {
            if (footnote.QFootnoteItemChosen)
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
            flag.Source = footnote.QFootnoteItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PFootnoteName") is Run name)
        {
            name.Text = footnote.QFootnoteItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PFootnoteEpithet") is Run epithet)
        {
            epithet.Text = " " + footnote.QFootnoteItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PFootnoteLanguage") is TextBlock language)
        {
            language.Text = footnote.QFootnoteItemLanguage;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QFootnoteItemChosen
    {
        get => _qFootnoteItemChosen;

        set
        {
            if (_qFootnoteItemChosen == value)
            {
                return;
            }

            _qFootnoteItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QFootnoteItemChosen)));
        }
    }
}
