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

        IReadOnlyList<LCatalogReference> works;
        try
        {
            works = _lEngine.LEngineOeuvreFind(
                id, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName);
        }
        catch (Exception)
        {
            works = [];
        }

        Dictionary<long, (string PFellowName, int PFellowShared)> shared = [];
        foreach (LCatalogReference work in works)
        {
            foreach (LAuthor credit in work.LCatalogReferenceCredit)
            {
                if (credit.LAuthorId == id)
                {
                    continue;
                }

                shared[credit.LAuthorId] =
                    shared.TryGetValue(credit.LAuthorId, out (string PFellowName, int PFellowShared) held)
                        ? (held.PFellowName, held.PFellowShared + 1)
                        : (credit.LAuthorName, 1);
            }
        }

        List<(long PFellowId, string PFellowName, int PFellowShared)> fellows = [];
        foreach ((long fellow, (string name, int count)) in shared)
        {
            fellows.Add((fellow, name, count));
        }

        fellows.Sort((left, right) =>
        {
            int order = right.PFellowShared.CompareTo(left.PFellowShared);
            return order != 0
                ? order
                : string.Compare(left.PFellowName, right.PFellowName, StringComparison.CurrentCultureIgnoreCase);
        });

        foreach ((long fellow, string name, int count) in fellows)
        {
            _pFellowList.Add(new PFellowItem(fellow, name, count));
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

        PTwin.PTwinNameApply(
            _pVitaCitation, row => row.PUsageItemHeadword, (row, name) => row.PUsageItemName = name);

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
