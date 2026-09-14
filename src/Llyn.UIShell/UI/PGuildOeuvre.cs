using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PGuild
{
    private static readonly IReadOnlyList<LAuthor> POeuvreNobody = [];

    private readonly ObservableCollection<PShelfItem> _pOeuvreList = [];

    private IReadOnlyList<LCatalogReference> _pOeuvreCatalog = [];

    private long? _pColophonReference;

    private LCatalogFilter _pLouverChoice = LCatalogFilter.LCatalogFilterEmpty;

    private void PCombHandle(object sender, TextChangedEventArgs e)
    {
        POeuvreFind();
    }

    private void PLouverHandle(object sender, RoutedEventArgs e)
    {
        _pLouverChoice = PChoice.PChoiceFilterRead(PLouverList);
        _lEngine.LEngineLouverSave(_pLouverChoice);
        PLouverMark.Visibility = _pLouverChoice.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        POeuvreFind();
    }

    internal void PLouverRestore(LCatalogFilter filter)
    {
        _pLouverChoice = filter;
        PLouverMark.Visibility = filter.LCatalogFilterActive ? Visibility.Visible : Visibility.Collapsed;
        PLouverBuild(filter);
    }

    private void PLouverBuild(LCatalogFilter filter)
    {
        PLouverList.Children.Clear();
        foreach (LReferenceKind kind in Enum.GetValues<LReferenceKind>())
        {
            string word = LReference.LReferenceKindFormat(kind);
            CheckBox box = new()
            {
                Style = (Style)PLouverList.FindResource("Theme.Choice.Filter"),
                Tag = word,
                IsChecked = filter.LCatalogFilterMatch(word),
            };
            box.SetResourceReference(ContentControl.ContentProperty, PReference.PReferenceKindRead(kind));
            box.Click += PLouverHandle;
            PLouverList.Children.Add(box);
        }
    }

    private LCatalogReference? POeuvreCatalogFind(long id)
    {
        foreach (LCatalogReference row in _pOeuvreCatalog)
        {
            if (row.LCatalogReferenceStored.LReferenceId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void POeuvreSelect(long? id)
    {
        foreach (PShelfItem item in _pOeuvreList)
        {
            item.PShelfItemChosen = id is not null && item.PShelfItemId == id;
        }
    }

    private void POeuvreFind()
    {
        try
        {
            _pOeuvreCatalog = _lEngine.LEngineOeuvreFind(
                _pRollAuthor, PComb.Text ?? string.Empty, _pLouverChoice, LCatalogOrder.LCatalogOrderName);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
            return;
        }

        string unknown = _pGuildHost.PLocalizationTextRead("Display.Unknown");
        string unset = _pGuildHost.PLocalizationTextRead("Source.Unset");

        _pOeuvreList.Clear();
        bool kept = false;
        foreach (LCatalogReference row in _pOeuvreCatalog)
        {
            kept |= row.LCatalogReferenceStored.LReferenceId == _pColophonReference;
            _pOeuvreList.Add(new PShelfItem(row, unknown, unset));
        }

        bool narrowed = !string.IsNullOrWhiteSpace(PComb.Text) || _pLouverChoice.LCatalogFilterActive;
        POeuvreEmpty.SetResourceReference(
            TextBlock.TextProperty,
            _pRollAuthor is null ? "Source.Empty" : narrowed ? "Guild.Unmatched" : "Guild.Vacant");
        POeuvreEmpty.Visibility = _pOeuvreList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        POeuvreSelect(_pColophonReference);

        if (!kept && _pColophonReference is not null)
        {
            PColophonReferenceHide();
        }
    }

    private void POeuvreHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PShelfItem item)
        {
            return;
        }

        if (!PGuildLeaveConfirm())
        {
            return;
        }

        PColophonReferenceShow(item.PShelfItemId);
    }

    private void PColophonReferenceShow(long id)
    {
        LReference? reference;
        try
        {
            reference = _lEngine.LEngineReferenceRead(id);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        if (reference is null)
        {
            PColophonReferenceHide();
            POeuvreFind();
            return;
        }

        PAutographCancel();
        PGuildScribeShow(false);

        LCatalogReference? row = POeuvreCatalogFind(id);
        _pColophonReference = id;
        POeuvreSelect(id);

        PColophon.PColophonShow(
            reference,
            row?.LCatalogReferenceCredit ?? POeuvreNobody,
            PGuildTallyRead(row?.LCatalogReferenceUsage ?? 0));

        PVita.Visibility = Visibility.Collapsed;
        PColophon.Visibility = Visibility.Visible;
        PGuildMode.IsEnabled = false;
        PGuildBin.IsEnabled = false;
    }

    private void PColophonReferenceHide()
    {
        _pColophonReference = null;
        POeuvreSelect(null);
        PColophon.PColophonClear();
        PColophon.Visibility = Visibility.Collapsed;

        if (PAutograph.Visibility != Visibility.Visible)
        {
            PVita.Visibility = Visibility.Visible;
        }

        bool held = _pRollAuthor is long author && author > 0;
        PGuildMode.IsEnabled = held;
        PGuildBin.IsEnabled = held;
    }
}
