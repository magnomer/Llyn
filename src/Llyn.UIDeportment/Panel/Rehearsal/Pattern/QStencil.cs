using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStencil
{
    private readonly UserControl _qStencilSurface;

    private readonly ObservableCollection<QStencilItem> _qStencilList = [];

    private readonly ObservableCollection<QStencilSide> _qStencilSides = [];

    private CStencil _cStencil = null!;

    internal QStencil(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStencilSurface = surface;
        QStencilStore.Click += QStencilStoreObserve;
        QStencilApplication.Click += QStencilApplyObserve;
        QStencilText.Click += QStencilTextObserve;
        QStencilSlots.ItemsSource = _qStencilList;
        QLookItem.QLookItemAttach(QStencilSlots, QStencilItemRefine);
        QStencilSides.ItemsSource = _qStencilSides;
        QLookItem.QLookItemAttach(QStencilSides, QStencilSideRefine);
    }

    private Button QStencilStore => QContract.QContractFind<Button>(_qStencilSurface, "PStencilStore");

    private Button QStencilApplication => QContract.QContractFind<Button>(_qStencilSurface, "PStencilApplication");

    private ComboBox QStencilSaved => QContract.QContractFind<ComboBox>(_qStencilSurface, "PStencilSaved");

    private ItemsControl QStencilSlots => QContract.QContractFind<ItemsControl>(_qStencilSurface, "PStencilSlots");

    private TextBlock QStencilGrain => QContract.QContractFind<TextBlock>(_qStencilSurface, "PStencilGrain");

    private ItemsControl QStencilSides => QContract.QContractFind<ItemsControl>(_qStencilSurface, "PStencilSides");

    private ComboBox QStencilPicture => QContract.QContractFind<ComboBox>(_qStencilSurface, "PStencilPicture");

    private ComboBox QStencilSound => QContract.QContractFind<ComboBox>(_qStencilSurface, "PStencilSound");

    private ToggleButton QStencilText => QContract.QContractFind<ToggleButton>(_qStencilSurface, "PStencilText");

    internal void QStencilIntroduce(CStencil stencil)
    {
        ArgumentNullException.ThrowIfNull(stencil);

        _cStencil = stencil;
        _cStencil.CStencilChanged += QStencilRefine;
    }

    internal void QStencilRefine()
    {
        CStencilState? state = _cStencil.CStencilRead();
        _qStencilSurface.Visibility = QLook.QLookVisibleRead(state is not null);
        if (state is null)
        {
            _qStencilList.Clear();
            _qStencilSides.Clear();
            QStencilSaved.ItemsSource = null;
            return;
        }

        QStencilGrain.SetResourceReference(TextBlock.TextProperty, state.CStencilStateGrain);
        QSplice.QSpliceRefine(
            _qStencilSides,
            [.. state.CStencilStateSides.Select(side =>
                new QStencilSide(side, state.CStencilStateFields, state.CStencilStateJoins))],
            QStencilSide.QStencilSideMatch,
            QStencilSide.QStencilSideSync);
        QStencilMediaRefine(
            QStencilPicture, state.CStencilStateSides, state.CStencilStatePicture, QStencilPictureObserve);
        QStencilMediaRefine(QStencilSound, state.CStencilStateSides, state.CStencilStateSound, QStencilSoundObserve);
        QStencilText.IsChecked = _cStencil.CStencilTextChecked;
        QStencilSlots.Visibility = QLook.QLookVisibleRead(_cStencil.CStencilTextChecked);
        QSplice.QSpliceRefine(
            _qStencilList,
            [.. state.CStencilStateSlots.Select(static slot => new QStencilItem(slot))],
            QStencilItem.QStencilItemMatch,
            QStencilItem.QStencilItemSync);
        QStencilSaved.ItemsSource = state.CStencilStateSaved.Select(static row => new QStencilChoice(row)).ToList();
    }

    private static void QStencilMediaRefine(
        ComboBox pick, IReadOnlyList<CStencilSide> sides, int chosen, SelectionChangedEventHandler observe)
    {
        pick.SelectionChanged -= observe;
        pick.Items.Clear();
        ComboBoxItem none = new();
        none.SetResourceReference(ContentControl.ContentProperty, "Stencil.MediaNone");
        pick.Items.Add(none);
        foreach (CStencilSide side in sides)
        {
            pick.Items.Add(new ComboBoxItem { Content = side.CStencilSideIndex.ToString(CultureInfo.CurrentCulture) });
        }

        pick.SelectedIndex = chosen;
        pick.SelectionChanged += observe;
    }

    private void QStencilSideRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QStencilSide side)
        {
            return;
        }

        CStencilSide row = side.QStencilSideRow;
        if (QLook.QLookPartFind<TextBlock>(container, "PStencilNumber") is TextBlock number)
        {
            number.Text = row.CStencilSideIndex.ToString(CultureInfo.CurrentCulture);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PStencilCustom") is TextBlock custom)
        {
            custom.Visibility = QLook.QLookVisibleRead(row.CStencilSideCustom);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PStencilChips") is ItemsControl chips)
        {
            chips.Visibility = QLook.QLookVisibleRead(!row.CStencilSideCustom);
            chips.ItemsSource = side.QStencilSideChips;
            QLookItem.QLookItemAttach(chips, QStencilChipRefine);
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PStencilField") is ComboBox field)
        {
            field.Visibility = QLook.QLookVisibleRead(!row.CStencilSideCustom);
            field.SelectionChanged -= QStencilFieldObserve;
            field.Items.Clear();
            foreach (CStencilField offer in side.QStencilSideFields)
            {
                ComboBoxItem choice = new();
                choice.SetResourceReference(ContentControl.ContentProperty, offer.CStencilFieldKey);
                field.Items.Add(choice);
            }

            field.SelectionChanged += QStencilFieldObserve;
        }
    }

    private void QStencilChipRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QStencilChip chip)
        {
            return;
        }

        QLook.QLookPartFind<TextBlock>(container, "PStencilCaption")?
            .SetResourceReference(TextBlock.TextProperty, chip.QStencilChipField.CStencilChipCaption);
        if (QLook.QLookPartFind<ComboBox>(container, "PStencilJoin") is ComboBox join)
        {
            join.SelectionChanged -= QStencilJoinObserve;
            join.Items.Clear();
            foreach (CStencilJoin offer in chip.QStencilChipJoins)
            {
                ComboBoxItem choice = new()
                {
                    IsSelected = offer.CStencilJoinValue == chip.QStencilChipField.CStencilChipJoin,
                };
                choice.SetResourceReference(ContentControl.ContentProperty, offer.CStencilJoinKey);
                join.Items.Add(choice);
            }

            join.SelectionChanged += QStencilJoinObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PStencilRemoval") is Button removal)
        {
            removal.Click -= QStencilChipObserve;
            removal.Click += QStencilChipObserve;
        }
    }

    private void QStencilItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QStencilItem row)
        {
            return;
        }

        CStencilSlot slot = row.QStencilItemSlot;
        QLook.QLookPartFind<TextBlock>(container, "PStencilCaption")?
            .SetResourceReference(TextBlock.TextProperty, slot.CStencilSlotCaption);
        if (QLook.QLookPartFind<TextBlock>(container, "PStencilNumber") is TextBlock number)
        {
            number.Text = slot.CStencilSlotNumber;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PStencilBody") is TextBox body)
        {
            body.TextChanged -= QStencilBodyObserve;
            QField.QFieldTextShow(body, slot.CStencilSlotBody);
            body.TextChanged += QStencilBodyObserve;
        }
    }

    private void QStencilBodyObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { DataContext: QStencilItem row } body)
        {
            _cStencil.CStencilSlotSet(
                row.QStencilItemSlot.CStencilSlotColumn, row.QStencilItemSlot.CStencilSlotIndex, body.Text);
        }
    }

    private void QStencilFieldObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QStencilSide side } pick)
        {
            _cStencil.CStencilFieldAppend(side.QStencilSideRow.CStencilSideIndex, pick.SelectedIndex);
        }
    }

    private void QStencilChipObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QStencilChip>(sender) is QStencilChip chip)
        {
            _cStencil.CStencilFieldRemove(chip.QStencilChipSide, chip.QStencilChipPosition);
        }
    }

    private void QStencilJoinObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QStencilChip chip } pick)
        {
            _cStencil.CStencilJoinSet(chip.QStencilChipSide, chip.QStencilChipPosition, pick.SelectedIndex);
        }
    }

    private void QStencilPictureObserve(object sender, SelectionChangedEventArgs e)
    {
        _cStencil.CStencilPictureSet(QStencilPicture.SelectedIndex);
    }

    private void QStencilSoundObserve(object sender, SelectionChangedEventArgs e)
    {
        _cStencil.CStencilSoundSet(QStencilSound.SelectedIndex);
    }

    private void QStencilTextObserve(object sender, RoutedEventArgs e)
    {
        _cStencil.CStencilTextToggle();
    }

    private void QStencilStoreObserve(object sender, RoutedEventArgs e)
    {
        _cStencil.CStencilSave();
    }

    private void QStencilApplyObserve(object sender, RoutedEventArgs e)
    {
        if (QStencilSaved.SelectedItem is QStencilChoice choice)
        {
            _cStencil.CStencilApply(choice.QStencilChoiceRow.CStencilRowId);
        }
    }
}
