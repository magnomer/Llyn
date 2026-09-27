using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

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
        QLookItem.QLookItemAttach(PCategoryList, PCategoryApply);
        QChoice.QChoiceDropperAttach(PMarkerSwitch, PCategory, PMarkerSurface);
    }

    private void PCategoryApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PCategoryItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCategoryName") is TextBlock name)
        {
            name.Text = row.PCategoryItemName;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCategoryIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("check", 12);
            icon.Visibility = QLook.QLookVisibleRead(row.PCategoryItemTaken);
        }

        if (QLook.QLookPartFind<Button>(container, "PCategoryChoice") is Button choice)
        {
            choice.Click -= _pCategoryTemplate.PCategoryHandle;
            choice.Click += _pCategoryTemplate.PCategoryHandle;
        }
    }

    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private readonly List<PCategoryItem> _pCategoryPreset = [];

    internal void PCategoryLoad()
    {
        IReadOnlyList<CSpeechValue> values;
        try
        {
            values = _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogSpeechRead(_lEditor.LEditorLanguage);
        }
        catch (Exception)
        {
            values = [];
        }

        _pCategoryPreset.Clear();
        foreach (CSpeechValue value in values)
        {
            _pCategoryPreset.Add(new PCategoryItem(value.CSpeechValueId, value.CSpeechValueName, false));
        }

        PCategoryUpdate();
    }

    private CSpeechValue? PCategoryAdd(string name)
    {
        CSpeechValue? created;
        try
        {
            created = _pEditorHost.PWindowAtelier.CAtelierCatalog.CCatalogSpeechAdd(_lEditor.LEditorLanguage, name);
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
