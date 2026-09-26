using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PAuthorItem : INotifyPropertyChanged
{
    private string _pAuthorItemName;

    private bool _pAuthorItemEarlier;

    private bool _pAuthorItemLater;

    internal PAuthorItem(long id, string name, int position, bool earlier, bool later)
    {
        PAuthorItemId = id;
        PAuthorItemPosition = position;
        _pAuthorItemName = name;
        _pAuthorItemEarlier = earlier;
        _pAuthorItemLater = later;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PAuthorItemId { get; }

    public int PAuthorItemPosition { get; }

    public bool PAuthorItemBlank => PAuthorItemId == 0;

    public string PAuthorItemName
    {
        get => _pAuthorItemName;

        private set
        {
            if (string.Equals(_pAuthorItemName, value, StringComparison.Ordinal))
            {
                return;
            }

            _pAuthorItemName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAuthorItemName)));
        }
    }

    public bool PAuthorItemEarlier
    {
        get => _pAuthorItemEarlier;

        private set
        {
            if (_pAuthorItemEarlier == value)
            {
                return;
            }

            _pAuthorItemEarlier = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAuthorItemEarlier)));
        }
    }

    public bool PAuthorItemLater
    {
        get => _pAuthorItemLater;

        private set
        {
            if (_pAuthorItemLater == value)
            {
                return;
            }

            _pAuthorItemLater = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAuthorItemLater)));
        }
    }

    internal static IReadOnlyList<PAuthorItem> PAuthorItemBuild(IReadOnlyList<LAuthorRow> rows, int blankAt)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PAuthorItem> built = [];
        int position = 0;
        foreach (LAuthorRow row in rows)
        {
            if (position == blankAt)
            {
                built.Add(new PAuthorItem(0, string.Empty, blankAt, false, false));
            }

            built.Add(new PAuthorItem(
                row.LAuthorRowId,
                row.LAuthorRowName,
                row.LAuthorRowPosition,
                row.LAuthorRowEarlier,
                row.LAuthorRowLater));
            position++;
        }

        if (position == blankAt)
        {
            built.Add(new PAuthorItem(0, string.Empty, blankAt, false, false));
        }

        return built;
    }

    internal static bool PAuthorItemMatch(PAuthorItem held, PAuthorItem fresh)
    {
        return held.PAuthorItemId == fresh.PAuthorItemId && held.PAuthorItemPosition == fresh.PAuthorItemPosition;
    }

    internal static void PAuthorItemSync(PAuthorItem held, PAuthorItem fresh)
    {
        held.PAuthorItemName = fresh.PAuthorItemName;
        held.PAuthorItemEarlier = fresh.PAuthorItemEarlier;
        held.PAuthorItemLater = fresh.PAuthorItemLater;
    }

    internal static void PAuthorItemApply(FrameworkElement container, object item, string? change)
    {
        if (item is not PAuthorItem author)
        {
            return;
        }

        if (change is null or nameof(PAuthorItemName)
            && PLook.PLookPartFind<TextBox>(container, "PAuthorName") is TextBox name)
        {
            name.Text = author.PAuthorItemName;
        }

        if (PLook.PLookPartFind<Button>(container, "PAuthorEarlier") is Button earlier)
        {
            earlier.IsEnabled = author.PAuthorItemEarlier;
        }

        if (PLook.PLookPartFind<Button>(container, "PAuthorLater") is Button later)
        {
            later.IsEnabled = author.PAuthorItemLater;
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PAuthorAdditionIcon") is PIconImage addition)
        {
            addition.PIconSource = PIcon.PIconResolve("add", 12);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PAuthorRemovalIcon") is PIconImage removal)
        {
            removal.PIconSource = PIcon.PIconResolve("remove", 12);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PAuthorEarlierIcon") is PIconImage earlierIcon)
        {
            earlierIcon.PIconSource = PIcon.PIconResolve("earlier", 12);
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PAuthorLaterIcon") is PIconImage laterIcon)
        {
            laterIcon.PIconSource = PIcon.PIconResolve("later", 12);
        }
    }

    internal static PAuthorItem? PAuthorItemFind(IEnumerable<PAuthorItem> rows)
    {
        foreach (PAuthorItem row in rows)
        {
            if (row.PAuthorItemBlank)
            {
                return row;
            }
        }

        return null;
    }

    internal static void PAuthorItemRestore(object? source)
    {
        if (source is TextBox { DataContext: PAuthorItem row } box)
        {
            box.Text = row.PAuthorItemName;
        }
    }
}
