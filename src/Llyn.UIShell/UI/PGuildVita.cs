using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PGuild
{
    private readonly ObservableCollection<PFellowItem> _pFellowList = [];

    private readonly ObservableCollection<PUsageItem> _pVitaCitation = [];

    private LCatalogAuthor? PVitaCatalogRead(long id)
    {
        if (PRollCatalogFind(id) is LCatalogAuthor listed)
        {
            return listed;
        }

        IReadOnlyList<LCatalogAuthor> whole;
        try
        {
            whole = _lEngine.LEngineAuthorFind(string.Empty, LCatalogOrder.LCatalogOrderName);
        }
        catch (Exception exception)
        {
            _pGuildHost.PWindowFailureShow("Guild.LoadFailed", exception);
            return null;
        }

        foreach (LCatalogAuthor row in whole)
        {
            if (row.LCatalogAuthorStored.LAuthorId == id)
            {
                return row;
            }
        }

        return null;
    }

    internal void PVitaShow(long id)
    {
        LCatalogAuthor? row = PVitaCatalogRead(id);
        if (row is null)
        {
            PGuildClear();
            return;
        }

        PVitaNameShow(row.LCatalogAuthorStored.LAuthorName);
        PVitaWork.Text = PGuildWorkRead(row.LCatalogAuthorWork);
        PVitaTally.Text = PGuildTallyRead(row.LCatalogAuthorUsage);
        PFellowShow(id);
        PVitaCitationShow(id);

        PVitaBody.Visibility = Visibility.Visible;
        PVitaUnselected.Visibility = Visibility.Collapsed;
    }

    private void PVitaNameShow(string name)
    {
        bool blank = name.Trim().Length == 0;

        PVitaName.Text = blank ? _pGuildHost.PLocalizationTextRead("Guild.Unnamed") : name;
        PField.PFieldPlaceholderShow(PVitaName, blank);
    }

    private void PFellowShow(long id)
    {
        _pFellowList.Clear();

        IReadOnlyList<LFellow> fellows;
        try
        {
            fellows = _lEngine.LEngineFellowFind(id);
        }
        catch (Exception)
        {
            fellows = [];
        }

        foreach (LFellow fellow in fellows)
        {
            _pFellowList.Add(new PFellowItem(fellow.LFellowId, fellow.LFellowName, fellow.LFellowShared));
        }

        PVitaFellowSection.Visibility = _pFellowList.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVitaCitationShow(long id)
    {
        _pVitaCitation.Clear();

        IReadOnlyList<LUsage> usages;
        try
        {
            usages = _lEngine.LEngineUsageRead(id, LOwner.LOwnerAuthor);
        }
        catch (Exception)
        {
            usages = [];
        }

        string unknown = _pGuildHost.PLocalizationTextRead("Display.Unknown");
        string meaning = _pGuildHost.PLocalizationTextRead("Display.MeaningSingle");
        string collocation = _pGuildHost.PLocalizationTextRead("Display.CollocationSingle");
        string example = _pGuildHost.PLocalizationTextRead("Vita.Example");

        foreach (LUsage usage in usages)
        {
            string owner = usage.LUsageOwner switch
            {
                LOwner.LOwnerCollocation => collocation,
                LOwner.LOwnerExample => example,
                _ => meaning,
            };
            _pVitaCitation.Add(new PUsageItem(
                usage, owner, unknown, string.Empty, _lEngine.LEngineEpithetRead(usage.LUsageEntry)));
        }

        PVitaCitationSection.Visibility = _pVitaCitation.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PFellowHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PFellowItem item)
        {
            return;
        }

        if (!PGuildLeaveConfirm())
        {
            return;
        }

        PRollAuthorShow(item.PFellowItemId);
    }

    private void PVitaCitationHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PUsageItem item)
        {
            return;
        }

        if (item.PUsageItemKind == LOwner.LOwnerExample)
        {
            _pGuildHost.PWindowExampleShow(item.PUsageItemId);
            return;
        }

        _pGuildHost.PWindowEntryShow(item.PUsageItemEntry);
    }

    private void PVitaClear()
    {
        _pFellowList.Clear();
        _pVitaCitation.Clear();
        PVitaBody.Visibility = Visibility.Collapsed;
        PVitaUnselected.Visibility = Visibility.Visible;
    }
}
