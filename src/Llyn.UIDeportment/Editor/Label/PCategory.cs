using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PCategoryTemplate _pCategoryTemplate;

    private Popup PCategory => (Popup)FindName(nameof(PCategory));

    private TextBlock PCategoryNotice => (TextBlock)FindName(nameof(PCategoryNotice));

    private TextBlock PCategoryAbsent => (TextBlock)FindName(nameof(PCategoryAbsent));

    private ItemsControl PCategoryList => (ItemsControl)FindName(nameof(PCategoryList));

    private void PCategoryAttach()
    {
        PCategoryList.ItemsSource = _pCategoryItem;
        PLookItem.PLookItemAttach(PCategoryList, PCategoryApply);
        PChoice.PChoiceDropperAttach(PMarkerSwitch, PCategory, PMarkerSurface);
    }

    private void PCategoryApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PCategoryItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PCategoryName") is TextBlock name)
        {
            name.Text = row.PCategoryItemName;
        }

        if (PLook.PLookPartFind<PIconImage>(container, "PCategoryIcon") is PIconImage icon)
        {
            icon.PIconSource = PIcon.PIconResolve("check", 12);
            icon.Visibility = PLook.PLookVisibleRead(row.PCategoryItemTaken);
        }

        if (PLook.PLookPartFind<Button>(container, "PCategoryChoice") is Button choice)
        {
            choice.Click -= _pCategoryTemplate.PCategoryHandle;
            choice.Click += _pCategoryTemplate.PCategoryHandle;
        }
    }

    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private readonly List<PCategoryItem> _pCategoryPreset = [];

    internal void PCategoryLoad()
    {
        IReadOnlyList<LSpeechValue> values;
        try
        {
            values = _pEditorHost.PWindowDeportment.LWindowSpeechRead(_lEditor.LEditorLanguage);
        }
        catch (Exception)
        {
            values = [];
        }

        _pCategoryPreset.Clear();
        foreach (LSpeechValue value in values)
        {
            _pCategoryPreset.Add(new PCategoryItem(value.LSpeechValueId, value.LSpeechValueName, false));
        }

        PCategoryUpdate();
    }

    private LSpeechValue? PCategoryAdd(string name)
    {
        LSpeechValue? created;
        try
        {
            created = _pEditorHost.PWindowDeportment.LWindowSpeechAdd(_lEditor.LEditorLanguage, name);
        }
        catch (Exception)
        {
            return null;
        }

        PCategoryLoad();
        return created;
    }

    private void PCategoryUpdate()
    {
        string typed = (PMarkerField.Text ?? string.Empty).Trim();

        _pCategoryItem.Clear();
        foreach (PCategoryItem preset in _pCategoryPreset)
        {
            string name = preset.PCategoryItemName;
            if (typed.Length > 0 && name.IndexOf(typed, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            _pCategoryItem.Add(new PCategoryItem(preset.PCategoryItemValue, name, PMarkerFind(name)));
        }

        bool declared = _pCategoryPreset.Count > 0;
        PCategoryNotice.Visibility = declared ? Visibility.Collapsed : Visibility.Visible;
        PCategoryAbsent.Visibility = declared && _pCategoryItem.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    internal void PCategoryHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCategoryItem item })
        {
            return;
        }

        PMarkerAdd(item.PCategoryItemName);
        PMarkerSwitch.IsChecked = false;
    }
}
