using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PNotationItem : INotifyPropertyChanged
{
    private string _pNotationItemNotice;
    private bool _pNotationItemReady;

    internal PNotationItem(string sourceLabel, int order, string notice)
    {
        PNotationItemSource = sourceLabel;
        PNotationItemOrder = order;
        _pNotationItemNotice = notice;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string PNotationItemSource { get; }

    internal int PNotationItemOrder { get; }

    public ObservableCollection<PNotationReading> PNotationItemReading { get; } = [];

    public string PNotationItemNotice
    {
        get => _pNotationItemNotice;
        private set => PNotationItemChange(ref _pNotationItemNotice, value, nameof(PNotationItemNotice));
    }

    public bool PNotationItemReady
    {
        get => _pNotationItemReady;
        private set => PNotationItemChange(ref _pNotationItemReady, value, nameof(PNotationItemReady));
    }

    internal void PNotationItemShow(CCandidate candidate, PNotationReading? reading, string missing, string broken)
    {
        if (reading is not null)
        {
            PNotationItemReading.Add(reading);
            PNotationItemNotice = string.Empty;
            PNotationItemReady = true;
            return;
        }

        if (PNotationItemReading.Count > 0)
        {
            return;
        }

        PNotationItemNotice = candidate.CCandidateReached ? missing : broken;
        PNotationItemReady = false;
    }

    private void PNotationItemChange(ref string held, string value, string name)
    {
        if (string.Equals(held, value, StringComparison.Ordinal))
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void PNotationItemChange(ref bool held, bool value, string name)
    {
        if (held == value)
        {
            return;
        }

        held = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal static void PNotationItemApply(FrameworkElement container, object item, RoutedEventHandler select)
    {
        if (item is not PNotationItem notation)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationSource") is TextBlock source)
        {
            source.Text = notation.PNotationItemSource;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PNotationReadingList") is ItemsControl list)
        {
            list.Visibility = QLook.QLookVisibleRead(notation.PNotationItemReady);
            if (!ReferenceEquals(list.ItemsSource, notation.PNotationItemReading))
            {
                list.ItemsSource = notation.PNotationItemReading;
            }

            QLookItem.QLookItemAttach(
                list, (reading, row, _) => PNotationReading.PNotationReadingApply(reading, row, select));
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PNotationNote") is TextBlock note)
        {
            note.Text = notation.PNotationItemNotice;
            note.Visibility = QLook.QLookVisibleRead(!notation.PNotationItemReady);
        }
    }
}
