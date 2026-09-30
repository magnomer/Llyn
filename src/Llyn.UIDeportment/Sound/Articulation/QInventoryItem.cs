using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QInventoryItem : INotifyPropertyChanged
{
    private bool _qInventoryItemChosen;

    internal QInventoryItem(CCatalogPronunciation row, bool chosen)
    {
        _qInventoryItemChosen = chosen;
        QInventoryItemId = row.CCatalogPronunciationEntry.CVistaRowId;
        QInventoryItemHeadword = row.CCatalogPronunciationEntry.CVistaRowHeadword;
        QInventoryItemName = row.CCatalogPronunciationEntry.CVistaRowName;
        QInventoryItemEpithet = row.CCatalogPronunciationEntry.CVistaRowEpithet;
        QInventoryItemLanguage = row.CCatalogPronunciationEntry.CVistaRowLanguage;
        QInventoryItemSound = row.CCatalogPronunciationSound;
        QInventoryItemPronunciation = row.CCatalogPronunciationText;
        QInventoryItemFlag = LEnsignImage.LEnsignFind(QInventoryItemLanguage);
    }

    public long QInventoryItemId { get; }

    public string QInventoryItemHeadword { get; }

    public string QInventoryItemEpithet { get; }

    public string QInventoryItemName { get; }

    public string QInventoryItemLanguage { get; }

    public string QInventoryItemSound { get; }

    public string QInventoryItemPronunciation { get; }

    public ImageSource? QInventoryItemFlag { get; }

    internal static IReadOnlyList<QInventoryItem> QInventoryItemBuild(IReadOnlyList<CCatalogPronunciation> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QInventoryItem> built = new(rows.Count);
        foreach (CCatalogPronunciation row in rows)
        {
            built.Add(new QInventoryItem(row, row.CCatalogPronunciationEntry.CVistaRowChosen));
        }

        return built;
    }

    internal static bool QInventoryItemMatch(QInventoryItem held, QInventoryItem fresh)
    {
        return held.QInventoryItemId == fresh.QInventoryItemId
            && string.Equals(held.QInventoryItemHeadword, fresh.QInventoryItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QInventoryItemEpithet, fresh.QInventoryItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QInventoryItemName, fresh.QInventoryItemName, StringComparison.Ordinal)
            && string.Equals(held.QInventoryItemLanguage, fresh.QInventoryItemLanguage, StringComparison.Ordinal)
            && string.Equals(held.QInventoryItemSound, fresh.QInventoryItemSound, StringComparison.Ordinal)
            && ReferenceEquals(held.QInventoryItemFlag, fresh.QInventoryItemFlag);
    }

    internal static void QInventoryItemSync(QInventoryItem held, QInventoryItem fresh)
    {
        held.QInventoryItemChosen = fresh.QInventoryItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QInventoryItemChosen
    {
        get => _qInventoryItemChosen;

        set
        {
            if (_qInventoryItemChosen == value)
            {
                return;
            }

            _qInventoryItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QInventoryItemChosen)));
        }
    }

    internal static void QInventoryItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QInventoryItem inventory)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PInventoryRow") is Button row)
        {
            if (inventory.QInventoryItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<Image>(container, "PInventoryFlag") is Image flag)
        {
            flag.Source = inventory.QInventoryItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PInventoryPronunciation") is TextBlock pronunciation)
        {
            pronunciation.Text = inventory.QInventoryItemPronunciation;
        }

        if (QLook.QLookPartFind<Run>(container, "PInventoryName") is Run name)
        {
            name.Text = inventory.QInventoryItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PInventoryEpithet") is Run epithet)
        {
            epithet.Text = " " + inventory.QInventoryItemEpithet;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PInventoryLanguage") is TextBlock language)
        {
            language.Text = inventory.QInventoryItemLanguage;
        }
    }
}
