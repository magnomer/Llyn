using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PInventoryItem : INotifyPropertyChanged
{
    private bool _pInventoryItemChosen;

    internal PInventoryItem(
        long id, string headword, string name, string language, string sound, string epithet = "", bool chosen = false)
    {
        _pInventoryItemChosen = chosen;
        PInventoryItemId = id;
        PInventoryItemHeadword = headword;
        PInventoryItemName = name;
        PInventoryItemEpithet = epithet ?? string.Empty;
        PInventoryItemLanguage = language;
        PInventoryItemSound = sound;
        PInventoryItemPronunciation = sound.Length == 0 ? "[ ]" : $"[{sound}]";
        PInventoryItemFlag = LEnsignImage.LEnsignFind(language);
    }

    public long PInventoryItemId { get; }

    public string PInventoryItemHeadword { get; }

    public string PInventoryItemEpithet { get; }

    public string PInventoryItemName { get; }

    public string PInventoryItemLanguage { get; }

    public string PInventoryItemSound { get; }

    public string PInventoryItemPronunciation { get; }

    public ImageSource? PInventoryItemFlag { get; }

    internal static IReadOnlyList<PInventoryItem> PInventoryItemBuild(IReadOnlyList<LCatalogPronunciation> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PInventoryItem> built = new(rows.Count);
        foreach (LCatalogPronunciation row in rows)
        {
            built.Add(new PInventoryItem(
                row.LCatalogPronunciationEntry.LEntryId,
                row.LCatalogPronunciationEntry.LEntryHeadword,
                row.LCatalogPronunciationName,
                row.LCatalogPronunciationEntry.LEntryLanguage,
                row.LCatalogPronunciationSound,
                row.LCatalogPronunciationEpithet ?? string.Empty,
                row.LCatalogPronunciationChosen));
        }

        return built;
    }

    internal static bool PInventoryItemMatch(PInventoryItem held, PInventoryItem fresh)
    {
        return held.PInventoryItemId == fresh.PInventoryItemId
            && string.Equals(held.PInventoryItemHeadword, fresh.PInventoryItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PInventoryItemEpithet, fresh.PInventoryItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PInventoryItemName, fresh.PInventoryItemName, StringComparison.Ordinal)
            && string.Equals(held.PInventoryItemLanguage, fresh.PInventoryItemLanguage, StringComparison.Ordinal)
            && string.Equals(held.PInventoryItemSound, fresh.PInventoryItemSound, StringComparison.Ordinal);
    }

    internal static void PInventoryItemSync(PInventoryItem held, PInventoryItem fresh)
    {
        held.PInventoryItemChosen = fresh.PInventoryItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PInventoryItemChosen
    {
        get => _pInventoryItemChosen;

        set
        {
            if (_pInventoryItemChosen == value)
            {
                return;
            }

            _pInventoryItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PInventoryItemChosen)));
        }
    }

    internal static void PInventoryItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PInventoryItem inventory)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PInventoryRow") is Button row)
        {
            if (inventory.PInventoryItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<Image>(container, "PInventoryFlag") is Image flag)
        {
            flag.Source = inventory.PInventoryItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PInventoryPronunciation") is TextBlock pronunciation)
        {
            pronunciation.Text = inventory.PInventoryItemPronunciation;
        }

        if (PLook.PLookPartFind<Run>(container, "PInventoryName") is Run name)
        {
            name.Text = inventory.PInventoryItemName;
        }

        if (PLook.PLookPartFind<Run>(container, "PInventoryEpithet") is Run epithet)
        {
            epithet.Text = " " + inventory.PInventoryItemEpithet;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PInventoryLanguage") is TextBlock language)
        {
            language.Text = inventory.PInventoryItemLanguage;
        }
    }
}
