using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PInventoryItem : INotifyPropertyChanged
{
    private bool _pInventoryItemChosen;

    internal PInventoryItem(
        long id, string headword, string language, string sound, string epithet = "", bool chosen = false)
    {
        _pInventoryItemChosen = chosen;
        PInventoryItemId = id;
        PInventoryItemHeadword = headword;
        PInventoryItemName = headword;
        PInventoryItemEpithet = epithet ?? string.Empty;
        PInventoryItemLanguage = language;
        PInventoryItemSound = sound;
        PInventoryItemPronunciation = sound.Length == 0 ? "[ ]" : $"[{sound}]";
        PInventoryItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PInventoryItemId { get; }

    public string PInventoryItemHeadword { get; }

    public string PInventoryItemEpithet { get; }

    public string PInventoryItemName { get; internal set; }

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
                row.LCatalogPronunciationEntry.LEntryLanguage,
                row.LCatalogPronunciationSound,
                row.LCatalogPronunciationEpithet ?? string.Empty,
                row.LCatalogPronunciationChosen)
            {
                PInventoryItemName = row.LCatalogPronunciationName,
            });
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
}
