using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PGuild
{
    private static readonly IReadOnlyList<LAuthor> POeuvreNobody = [];

    private readonly ObservableCollection<PShelfItem> _pOeuvreList = [];

    private IReadOnlyList<LCatalogReference> _pOeuvreCatalog = [];

    private LVista? _pOeuvreVista;

    private void PCombHandle(object sender, TextChangedEventArgs e)
    {
        _pOeuvreVista?.LVistaQuerySet(PComb.Text ?? string.Empty);
    }

    private void PLouverHandle(object sender, RoutedEventArgs e)
    {
        if (_pGuildVista is null)
        {
            return;
        }

        _pGuildVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PLouverList));
        PLouverRestore();
    }

    private void PLouverRestore()
    {
        if (_pGuildVista is not LVista vista)
        {
            PLouverMark.Visibility = Visibility.Collapsed;
            return;
        }

        PLouverMark.Visibility = vista.LVistaFiltered ? Visibility.Visible : Visibility.Collapsed;
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

    private void POeuvreFind()
    {
        if (_pGuildVista is null || _pOeuvreVista is null)
        {
            return;
        }

        try
        {
            _pOeuvreCatalog = _lEngine.LEngineOeuvreFind(_pGuildVista, _pOeuvreVista);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string unset = PLocalizationCatalog.PLocalizationTextRead("Source.Unset");

        List<PShelfItem> fresh = [];
        bool kept = false;
        foreach (LCatalogReference row in _pOeuvreCatalog)
        {
            kept |= row.LCatalogReferenceChosen;
            fresh.Add(new PShelfItem(row, unknown, unset, row.LCatalogReferenceChosen));
        }

        PSplice.PSpliceApply(
            _pOeuvreList, fresh, PShelfItem.PShelfItemMatch, PShelfItem.PShelfItemSync);

        bool narrowed = _pOeuvreVista.LVistaQueried || _pGuildVista.LVistaFiltered;
        POeuvreEmpty.SetResourceReference(
            TextBlock.TextProperty,
            _pGuildVista.LVistaChosen is null ? "Source.Empty" : narrowed ? "Guild.Unmatched" : "Guild.Vacant");
        POeuvreEmpty.Visibility = _pOeuvreList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept)
        {
            if (_pOeuvreVista.LVistaChosen is not null)
            {
                PColophonReferenceHide();
            }
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
            _pOeuvreVista?.LVistaSelect(id);
            reference = _pOeuvreVista?.LVistaLoad()?.LDraftReference;
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
        _pOeuvreVista?.LVistaSelect(id);
        POeuvreFind();

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
        _pOeuvreVista?.LVistaSelect(null);
        POeuvreFind();
        PColophon.PColophonClear();
        PColophon.Visibility = Visibility.Collapsed;

        if (PAutograph.Visibility != Visibility.Visible)
        {
            PVita.Visibility = Visibility.Visible;
        }

        bool held = _pGuildVista?.LVistaStored is not null;
        PGuildMode.IsEnabled = held;
        PGuildBin.IsEnabled = held;
    }
}
