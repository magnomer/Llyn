using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PCorpus
{
    private const int PCitationLimit = 8;
    private const double PCitationShade = 10;
    private const double PCitationGap = 6;

    private readonly ObservableCollection<PCandidateItem> _pCitationItem = [];

    private bool _pCitationFilling;

    private void PCitationFind()
    {
        _pCitationCatalog.Clear();

        IReadOnlyList<LCatalogReference> read;
        try
        {
            read = _lEngine.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Reference.LoadFailed", exception);
            read = [];
        }

        foreach (LCatalogReference row in read)
        {
            _pCitationCatalog.Add(PCitationItem.PCitationItemCreate(row));
        }
    }

    private string PCitationNameRead(long id)
    {
        if (id == 0)
        {
            return string.Empty;
        }

        foreach (PCitationItem row in _pCitationCatalog)
        {
            if (row.PCitationItemId == id)
            {
                return row.PCitationItemName;
            }
        }

        return id.ToString(CultureInfo.InvariantCulture);
    }

    private void PCitationUpdate()
    {
        _pCitationFilling = true;
        PCitationField.Text = PCitationNameRead(_pTranscriptCitation);
        _pCitationFilling = false;
    }

    private void PCitationTextHandle(object sender, TextChangedEventArgs e)
    {
        if (_pCitationFilling || _pTranscriptLoading || !PCitationField.IsKeyboardFocusWithin)
        {
            return;
        }

        PCitationShow(PCitationField.Text);
    }

    private void PCitationKeyHandle(object sender, KeyEventArgs e)
    {
        if (PCitationDrawer.IsOpen && PCitationHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PCitationCommit();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            PCitationHide();
            PCitationUpdate();
            e.Handled = true;
        }
    }

    private void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PCitationHide();
        PCitationUpdate();
    }

    internal void PCitationPickHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCandidateItem item })
        {
            PCitationHide();
            return;
        }

        PCitationSelect(item);
        e.Handled = true;
    }

    private bool PCitationHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PCitationHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pCitationItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PCitationList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PCitationList.SelectedIndex;
            PCitationList.SelectedIndex = (chosen + step) % count;
            PCitationList.ScrollIntoView(PCitationList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PCitationList.SelectedItem is PCandidateItem item)
        {
            PCitationSelect(item);
            return true;
        }

        return false;
    }

    private void PCitationSelect(PCandidateItem item)
    {
        PCitationHide();
        PCitationSet(item.PCandidateItemId);
    }

    private void PCitationSet(long id)
    {
        _pTranscriptCitation = id;
        PCitationUpdate();
        PTranscriptChangeDefer();
    }

    private void PCitationCommit()
    {
        PCitationHide();

        string typed = PCitationField.Text.Trim();
        if (typed.Length == 0)
        {
            PCitationSet(0);
            return;
        }

        if (string.Equals(typed, PCitationNameRead(_pTranscriptCitation), StringComparison.Ordinal))
        {
            PCitationUpdate();
            return;
        }

        foreach (PCitationItem item in _pCitationCatalog)
        {
            if (string.Equals(item.PCitationItemName, typed, StringComparison.CurrentCultureIgnoreCase))
            {
                PCitationSet(item.PCitationItemId);
                return;
            }
        }

        LReference stored;
        try
        {
            stored = _lEngine.LEngineCitationCreate(typed);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Reference.CreateFailed", exception);
            PCitationUpdate();
            return;
        }

        _pCitationCatalog.Add(PCitationItem.PCitationItemCreate(
            LCatalogReference.LCatalogReferenceCreate(stored, null, 0)));
        PCitationSet(stored.LReferenceId);
    }

    private void PCitationShow(string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0 || string.Equals(word, PCitationNameRead(_pTranscriptCitation), StringComparison.Ordinal))
        {
            PCitationHide();
            return;
        }

        IReadOnlyList<LCatalogReference> found;
        try
        {
            found = _lEngine.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage);
        }
        catch (Exception)
        {
            PCitationHide();
            return;
        }

        _pCitationItem.Clear();
        foreach (LCatalogReference reference in found)
        {
            _pCitationItem.Add(new PCandidateItem(
                reference.LCatalogReferenceStored.LReferenceId,
                reference.LCatalogReferenceByline,
                reference.LCatalogReferenceUsage,
                word));

            if (_pCitationItem.Count == PCitationLimit)
            {
                break;
            }
        }

        if (_pCitationItem.Count == 0)
        {
            PCitationHide();
            return;
        }

        PCitationDrawer.PlacementTarget = PCitationFrameFind(PCitationField) ?? PCitationField;
        PCitationDrawer.IsOpen = true;
        PCitationList.SelectedIndex = -1;
    }

    private void PCitationHide()
    {
        PCitationDrawer.IsOpen = false;
        PCitationList.SelectedIndex = -1;
        _pCitationItem.Clear();
    }

    private static CustomPopupPlacement[] PCitationPlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PCitationShade, target.Height + PCitationGap - PCitationShade);
        var pAbove = new Point(-PCitationShade, PCitationShade - PCitationGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PCitationFrameFind(TextBox box)
    {
        box.ApplyTemplate();
        return box.Template?.FindName(PField.PFieldSurfaceName, box) as FrameworkElement;
    }
}
