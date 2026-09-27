using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PMarkerTemplate _pMarkerTemplate;

    private WrapPanel PMarker => (WrapPanel)FindName(nameof(PMarker));

    private ItemsControl PMarkerList => (ItemsControl)FindName(nameof(PMarkerList));

    private Border PMarkerSurface => (Border)FindName(nameof(PMarkerSurface));

    private TextBox PMarkerField => (TextBox)FindName(nameof(PMarkerField));

    private ToggleButton PMarkerSwitch => (ToggleButton)FindName(nameof(PMarkerSwitch));

    private QIconImage PMarkerIcon => (QIconImage)FindName(nameof(PMarkerIcon));

    private void PMarkerAttach()
    {
        PMarkerList.ItemsSource = _pMarkerChip;
        QLookItem.QLookItemAttach(PMarkerList, PMarkerApply);
        PMarker.SizeChanged += (_, e) => PMarkerList.MaxWidth = e.NewSize.Width;
        PMarkerField.SetResourceReference(QField.QFieldHintProperty, "Speech.Title");
        PMarkerField.KeyDown += PMarkerFieldHandle;
        PMarkerField.TextChanged += PMarkerTextHandle;
        PMarkerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
    }

    private void PMarkerApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PMarkerChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PMarkerName") is TextBlock name)
        {
            name.Text = chip.PMarkerChipName;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PMarkerIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("close", 12);
        }

        if (QLook.QLookPartFind<Button>(container, "PMarkerEraser") is Button eraser)
        {
            eraser.Click -= _pMarkerTemplate.PMarkerChipHandle;
            eraser.Click += _pMarkerTemplate.PMarkerChipHandle;
        }
    }

    private readonly ObservableCollection<PMarkerChip> _pMarkerChip = [];

    internal void PMarkerChipHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PMarkerChip chip })
        {
            _pMarkerChip.Remove(chip);
            PCategoryUpdate();
            PEditorSpeechSend();
        }
    }

    private void PMarkerFieldHandle(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            PMarkerSwitch.IsChecked = false;
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Enter)
        {
            return;
        }

        PMarkerAdd(PMarkerField.Text ?? string.Empty);
        e.Handled = true;
    }

    private void PMarkerTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_lEditor.LEditorDesk.LDeskFilling)
        {
            return;
        }

        PCategoryUpdate();
        PEditorRequestDefer(new LRequestSpeech(PEditorDraft, PMarkerRead()));

        string typed = (PMarkerField.Text ?? string.Empty).Trim();
        PMarkerSwitch.IsChecked = typed.Length > 0 && _pCategoryItem.Count > 0;
    }

    private void PMarkerAdd(string name)
    {
        string typed = name.Trim();
        PMarkerField.Text = string.Empty;

        if (typed.Length == 0)
        {
            return;
        }

        if (!PMarkerFind(typed))
        {
            CSpeechValue? value = PCategoryAdd(typed);
            _pMarkerChip.Add(value is null
                ? new PMarkerChip(0, typed)
                : new PMarkerChip(value.CSpeechValueId, value.CSpeechValueName));
        }

        PCategoryUpdate();
        PEditorSpeechSend();
    }

    private bool PMarkerMatch(IReadOnlyList<CSpeechDraft> speeches)
    {
        IReadOnlyList<LSpeechDraft> shown = PMarkerRead();
        if (shown.Count != speeches.Count)
        {
            return false;
        }

        for (int index = 0; index < shown.Count; index++)
        {
            if (shown[index].LSpeechDraftValue != speeches[index].CSpeechDraftValue
                || !string.Equals(
                    shown[index].LSpeechDraftName, speeches[index].CSpeechDraftName, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private bool PMarkerFind(string name)
    {
        foreach (PMarkerChip chip in _pMarkerChip)
        {
            if (string.Equals(chip.PMarkerChipName, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private IReadOnlyList<LSpeechDraft> PMarkerRead()
    {
        List<LSpeechDraft> drafts = new(_pMarkerChip.Count + 1);
        foreach (PMarkerChip chip in _pMarkerChip)
        {
            drafts.Add(chip.PMarkerChipValue > 0
                ? LSpeechDraft.LSpeechDraftCreate(chip.PMarkerChipValue, chip.PMarkerChipName)
                : LSpeechDraft.LSpeechDraftCreate(chip.PMarkerChipName));
        }

        string typed = (PMarkerField.Text ?? string.Empty).Trim();
        if (typed.Length > 0 && !PMarkerFind(typed))
        {
            drafts.Add(LSpeechDraft.LSpeechDraftCreate(typed));
        }

        return drafts;
    }

    private void PMarkerShow(IReadOnlyList<CSpeechDraft> speeches)
    {
        if (PMarkerMatch(speeches))
        {
            return;
        }

        _pMarkerChip.Clear();
        PMarkerField.Text = string.Empty;

        foreach (CSpeechDraft speech in speeches)
        {
            string name = speech.CSpeechDraftName.Trim();
            if (name.Length > 0 && !PMarkerFind(name))
            {
                _pMarkerChip.Add(new PMarkerChip(speech.CSpeechDraftValue, name));
            }
        }

        PCategoryUpdate();
    }
}
