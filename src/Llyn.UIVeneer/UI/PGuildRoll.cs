using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PGuild
{
    private readonly ObservableCollection<PRollItem> _pRollList = [];

    private IReadOnlyList<LCatalogAuthor> _pRollCatalog = [];

    private LVista? _pGuildVista;

    private void PGuildCatalogUpdate()
    {
        PRollFind();

        if (_pGuildVista?.LVistaChosen is long author)
        {
            if (PRollCatalogFind(author) is not null)
            {
                if (PAutograph.Visibility == Visibility.Visible)
                {
                    PAutographTallyShow(author);
                }
                else
                {
                    PVitaShow(author);
                }
            }
        }

        if (_pOeuvreVista?.LVistaChosen is long reference)
        {
            PColophonReferenceShow(reference);
        }
    }

    private void PMusterHandle(object sender, TextChangedEventArgs e)
    {
        _pGuildVista?.LVistaQuerySet(PMuster.Text ?? string.Empty);
    }

    private void PEchelonHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pGuildVista is null)
        {
            return;
        }

        PEchelonDropper.IsChecked = false;
        _pGuildVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pGuildVista.LVistaOrder));
    }

    internal void PGuildVistaRestore(LVista vista, LVista oeuvre)
    {
        _pGuildVista = vista;
        _pOeuvreVista = oeuvre;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PRollFind));
        oeuvre.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, POeuvreFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PGuildReset));
        vista.LVistaObserverAttach(LSubject.LSubjectAuthor, new PObserver(this, PGuildCatalogUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectReference, new PObserver(this, PGuildCatalogUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectExample, new PObserver(this, PGuildCatalogUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PGuildCatalogUpdate));
        PEchelonRestore();
        PLouverRestore();
        PLouverBuild(vista.LVistaFilter);
        vista.LVistaQuerySet(PMuster.Text ?? string.Empty);
        oeuvre.LVistaQuerySet(PComb.Text ?? string.Empty);
        PRollFind();
    }

    private void PEchelonRestore()
    {
        if (_pGuildVista is not null)
        {
            PChoice.PChoiceOrderApply(PEchelonDropdown, _pGuildVista.LVistaOrder);
        }
    }

    private LCatalogAuthor? PRollCatalogFind(long id)
    {
        foreach (LCatalogAuthor row in _pRollCatalog)
        {
            if (row.LCatalogAuthorStored.LAuthorId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PRollFind()
    {
        if (_pGuildVista is null)
        {
            return;
        }

        string query = _pGuildVista.LVistaQuery;
        IReadOnlyList<LCatalogReference> orphan;
        try
        {
            _pRollCatalog = _lEngine.LEngineAuthorFind(_pGuildVista);
            orphan = _lEngine.LEngineOeuvreFind(
                0, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
            return;
        }

        List<PRollItem> fresh = [];
        if (string.IsNullOrWhiteSpace(query) && orphan.Count > 0)
        {
            int cited = 0;
            foreach (LCatalogReference row in orphan)
            {
                cited += row.LCatalogReferenceUsage;
            }

            fresh.Add(new PRollItem(
                PLocalizationCatalog.PLocalizationTextRead("Guild.Uncredited"),
                PGuildWorkRead(orphan.Count),
                cited,
                _pGuildVista.LVistaChosen == 0));
        }

        bool kept = _pGuildVista.LVistaChosen == 0 && fresh.Count > 0;
        foreach (LCatalogAuthor row in _pRollCatalog)
        {
            kept |= row.LCatalogAuthorChosen;
            fresh.Add(new PRollItem(row, PGuildWorkRead(row.LCatalogAuthorWork), row.LCatalogAuthorChosen));
        }

        PSplice.PSpliceApply(
            _pRollList, fresh, PRollItem.PRollItemMatch, PRollItem.PRollItemSync);
        PRollEmpty.Visibility = _pRollList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        if (!kept)
        {
            if (_pGuildVista.LVistaChosen is not null)
            {
                PGuildClear();
                return;
            }
        }

        POeuvreFind();
    }

    private void PRollHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PRollItem item)
        {
            return;
        }

        if (!PGuildLeaveConfirm())
        {
            return;
        }

        PRollAuthorShow(item.PRollItemId);
    }

    internal void PRollAuthorShow(long id)
    {
        bool editing = PAutograph.Visibility == Visibility.Visible;
        PAutographCancel();

        _pGuildVista?.LVistaSelect(id);
        PRollFind();
        PColophonReferenceHide();
        POeuvreFind();

        if (id <= 0)
        {
            PVitaClear();
            PGuildScribeShow(false);
            PGuildMode.IsEnabled = false;
            PGuildBin.IsEnabled = false;
            return;
        }

        PGuildMode.IsEnabled = true;
        PGuildBin.IsEnabled = true;

        if (editing)
        {
            PGuildScribeShow(true);
            PAutographOpen(id);
            return;
        }

        PGuildScribeShow(false);
        PVitaShow(id);
    }

    internal string PGuildWorkRead(int count)
    {
        return count switch
        {
            0 => PLocalizationCatalog.PLocalizationTextRead("Guild.WorkNone"),
            1 => PLocalizationCatalog.PLocalizationTextRead("Guild.WorkOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                PLocalizationCatalog.PLocalizationTextRead("Guild.WorkMany")),
        };
    }

    internal string PGuildTallyRead(int count)
    {
        return count switch
        {
            0 => PLocalizationCatalog.PLocalizationTextRead("Source.UsageNone"),
            1 => PLocalizationCatalog.PLocalizationTextRead("Source.UsageOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                PLocalizationCatalog.PLocalizationTextRead("Source.UsageMany")),
        };
    }
}
