using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PReference
{
    private static readonly IReadOnlyList<LAuthor> PShelfNobody = [];

    private readonly ObservableCollection<PShelfItem> _pShelfList = [];

    private IReadOnlyDictionary<long, int> _pShelfCount = new Dictionary<long, int>();

    private IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> _pShelfCredit =
        new Dictionary<long, IReadOnlyList<LAuthor>>();

    private LVista? _pReferenceVista;

    private void PReferenceWorkspaceUpdate()
    {
        PReferenceReset();
        PImprint.PAuthorFind();
    }

    private void PShelfReferenceUpdate()
    {
        PImprint.PAuthorFind();
        PShelfFind();
    }

    private void PSurveyHandle(object sender, TextChangedEventArgs e)
    {
        _pReferenceVista?.LVistaQuerySet(PSurvey.Text ?? string.Empty);
    }

    private void PRummageHandle(object sender, TextChangedEventArgs e)
    {
        _pFootnoteVista?.LVistaQuerySet(PRummage.Text);
    }

    private void PTrellisHandle(object sender, RoutedEventArgs e)
    {
        if (_pReferenceVista is null)
        {
            return;
        }

        _pReferenceVista.LVistaFilterSet(PChoice.PChoiceFilterRead(PTrellisList));
        PTrellisRestore();
    }

    private void PGradeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string choice } || _pReferenceVista is null)
        {
            return;
        }

        PGradeDropper.IsChecked = false;
        _pReferenceVista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, _pReferenceVista.LVistaOrder));
    }

    internal async void PReferenceVistaRestore(LVista vista, LVista footnote)
    {
        _pReferenceVista = vista;
        _pFootnoteVista = footnote;
        vista.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PShelfFind));
        vista.LVistaObserverAttach(LSubject.LSubjectWorkspace, new PObserver(this, PReferenceWorkspaceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectAuthor, new PObserver(this, PImprint.PImprintAuthorUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectAuthor, new PObserver(this, PShelfFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReference, new PObserver(this, PShelfReferenceUpdate));
        vista.LVistaObserverAttach(LSubject.LSubjectExample, new PObserver(this, PShelfFind));
        vista.LVistaObserverAttach(LSubject.LSubjectReflex, new PObserver(this, PShelfFind));
        vista.LVistaObserverAttach(LSubject.LSubjectSettings, new PObserver(this, PShelfFind));
        footnote.LVistaObserverAttach(LSubject.LSubjectEntry, new PObserver(this, PFootnoteEntryUpdate));
        footnote.LVistaChosenAttach(LSubject.LSubjectEntry, new PObserver(this, PReferenceEntryUpdate));
        footnote.LVistaObserverAttach(LSubject.LSubjectVista, new PObserver(this, PFootnoteFind));
        PDisplay.PDisplayVistaRestore(footnote);
        PGradeRestore();
        PTrellisRestore();

        await PEnsign.PEnsignLoad(_lEngine);

        PChoice.PChoiceFilterBuild(PTrellisList, _lEngine.LEngineLanguageRead(), vista.LVistaFilter, PTrellisHandle);
        vista.LVistaQuerySet(PSurvey.Text ?? string.Empty);
        footnote.LVistaQuerySet(PRummage.Text);
        PImprint.PAuthorFind();
        PShelfFind();
    }

    private void PGradeRestore()
    {
        if (_pReferenceVista is not null)
        {
            PChoice.PChoiceOrderApply(PGradeDropdown, _pReferenceVista.LVistaOrder);
        }
    }

    private void PTrellisRestore()
    {
        if (_pReferenceVista is not LVista vista)
        {
            PTrellisMark.Visibility = Visibility.Collapsed;
            return;
        }

        PTrellisMark.Visibility = vista.LVistaFiltered ? Visibility.Visible : Visibility.Collapsed;
    }

    internal IReadOnlyList<LAuthor> PShelfCreditRead(long id)
    {
        return _pShelfCredit.GetValueOrDefault(id) ?? PShelfNobody;
    }

    internal int PShelfCountRead(long id)
    {
        return _pShelfCount.TryGetValue(id, out int usage) ? usage : 0;
    }

    private void PShelfFind()
    {
        if (_pReferenceVista is null)
        {
            return;
        }

        IReadOnlyList<LCatalogReference> read;
        try
        {
            read = _lEngine.LEngineReferenceFind(_pReferenceVista);
            _pShelfCount = _lEngine.LEngineUsageRead(LOwner.LOwnerReference);
            _pShelfCredit = _lEngine.LEngineAuthorRead(LOwner.LOwnerReference);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        string unknown = PLocalizationCatalog.PLocalizationTextRead("Display.Unknown");
        string unset = PLocalizationCatalog.PLocalizationTextRead("Source.Unset");

        List<PShelfItem> fresh = [];
        bool kept = false;
        foreach (LCatalogReference row in read)
        {
            kept |= row.LCatalogReferenceChosen;
            fresh.Add(new PShelfItem(row, unknown, unset, row.LCatalogReferenceChosen));
        }

        PSplice.PSpliceApply(
            _pShelfList, fresh, PShelfItem.PShelfItemMatch, PShelfItem.PShelfItemSync);

        PShelfEmpty.Visibility = _pShelfList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        PColophon.PColophonTallyShow(PReferenceTallyRead(_pReferenceVista?.LVistaChosen));
        PImprint.PImprintTallyShow();

        if (!kept)
        {
            if (_pReferenceVista?.LVistaChosen is not null)
            {
                if (PImprint.Visibility != Visibility.Visible)
                {
                    PReferenceClear();
                }
            }
        }

        PFootnoteFind();
    }

    private void PShelfHandle(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement row || row.DataContext is not PShelfItem item)
        {
            return;
        }

        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceShow(item.PShelfItemId);
    }

    internal void PReferenceShow(long id)
    {
        LReference? reference;
        try
        {
            _pReferenceVista?.LVistaSelect(id);
            reference = _pReferenceVista?.LVistaLoad()?.LDraftReference;
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.LoadFailed", exception);
            return;
        }

        if (reference is null)
        {
            PReferenceClear();
            PShelfFind();
            return;
        }

        _pReferenceVista?.LVistaSelect(id);
        PShelfFind();
        PFootnoteEntryHide();

        PColophon.PColophonShow(reference, PShelfCreditRead(id), PReferenceTallyRead(id));

        PFootnoteFind();

        PReferenceMode.IsEnabled = true;
        PReferenceBin.IsEnabled = true;

        if (PImprint.Visibility == Visibility.Visible)
        {
            PImprint.PImprintDraftOpen(id);
        }
    }

    private void PReferenceBinHandle(object sender, RoutedEventArgs e)
    {
        if (_pFootnoteVista?.LVistaChosen is not null)
        {
            return;
        }

        if (_pReferenceVista?.LVistaChosen is not long id)
        {
            return;
        }

        int usage = PShelfCountRead(id);

        if (!_pReferenceHost.PWindowRemovalConfirm(usage, "Source"))
        {
            return;
        }

        try
        {
            _pReferenceVista.LVistaDelete();
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.DeleteFailed", exception);
            return;
        }

        PReferenceScribeShow(false);
        PReferenceClear();
    }

    internal string PReferenceTallyRead(long? id)
    {
        int count = id is long stored ? PShelfCountRead(stored) : 0;

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

    private void PReferenceScribeHandle(object sender, RoutedEventArgs e)
    {
        bool editing = ReferenceEquals(sender, PReferenceScribe);
        if (_pFootnoteVista?.LVistaChosen is not null || PEditor.Visibility == Visibility.Visible)
        {
            PFootnoteScribeHandle(editing);
            return;
        }

        if (editing == (PImprint.Visibility == Visibility.Visible))
        {
            return;
        }

        if (!editing)
        {
            if (!PReferenceLeaveConfirm())
            {
                PReferenceScribeShow(true);
                return;
            }

            PImprint.PImprintDraftCancel();
            PReferenceScribeShow(false);

            if (_pReferenceVista?.LVistaChosen is long chosen)
            {
                PReferenceShow(chosen);
                return;
            }

            PReferenceClear();
            return;
        }

        if (_pReferenceVista?.LVistaChosen is not long shown)
        {
            PReferenceClear();
            return;
        }

        PReferenceScribeShow(true);
        PImprint.PImprintDraftOpen(shown);
    }

    internal void PReferenceScribeShow(bool editing)
    {
        _pReferenceVista?.LVistaEditingSet(editing);

        PImprint.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        PColophon.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
        PReferenceViewer.IsChecked = !editing;
        PReferenceScribe.IsChecked = editing;
        PImprint.PImprintChangeUpdate();
    }

    internal void PReferenceScribeRestore(bool editing)
    {
        if (editing)
        {
            if (_pReferenceVista?.LVistaChosen is null)
            {
                return;
            }
        }

        if (editing)
        {
            PReferenceMode.IsEnabled = true;
        }

        PReferenceScribeShow(editing);
    }

    internal bool PReferenceLeaveConfirm()
    {
        return _pReferenceHost.PWindowDiscardConfirm(PReferenceChangeCheck(), PReferenceDraftFinish);
    }

    internal void PReferenceClear()
    {
        PImprint.PImprintDraftCancel();

        _pReferenceVista?.LVistaSelect(null);
        PShelfFind();
        PFootnoteEntryHide();
        PFootnoteFind();

        PColophon.PColophonClear();
        PImprint.PImprintClear();
        PReferenceScribeShow(false);
        PReferenceMode.IsEnabled = false;
        PReferenceBin.IsEnabled = false;
    }
}
