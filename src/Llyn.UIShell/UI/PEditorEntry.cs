using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PEditorEntryShow(long id)
    {
        LDraft? started = PEditorDraftStart(id);

        if (started is null)
        {
            PEditorReset();
            return;
        }

        PSentenceFrameLoad(started.LDraftContent.LEntryDraftLanguage);
        PEditorDraftShow(started.LDraftContent);
        PEditorChangeUpdate();
        PEditorFavoriteShow();
        PEditorGraspShow();
        PEditorFrequencyShow();
        PEditorParadigmShow();
        PEditorScriptShow();
        PEditorFanqieShow();
    }

    internal void PEditorFrequencyShow()
    {
        long? entry = PEditorEntryRead();
        IReadOnlyList<LFrequency> frequency = [];

        if (entry is not null)
        {
            try
            {
                frequency = _lEngine.LEngineFrequencyRead(entry.Value);
            }
            catch (Exception)
            {
                frequency = [];
            }
        }

        if (frequency.Count == 0)
        {
            PEditorFrequency.Text = string.Empty;
            PEditorFrequencyChip.ToolTip = null;
            PEditorFrequencySection.Visibility = Visibility.Collapsed;
            return;
        }

        int count = PFrequencyLabel.PFrequencyBandResolve(frequency);
        PFrequencyLabel.PFrequencyLabelShow(
            PEditorFrequency,
            PEditorFrequencyBand,
            count,
            _pEditorHost.PLocalizationTextRead(PFrequencyLabel.PFrequencyLabelResolve(count)));
        PEditorFrequencyChip.ToolTip = PFrequencyLabel.PFrequencySourceFormat(
            frequency, _pEditorHost.PLocalizationTextRead("Frequency.Once"));
        PEditorFrequencySection.Visibility = Visibility.Visible;
    }

    internal void PEditorFavoriteShow()
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorFavorite.IsEnabled = false;
            PEditorFavorite.IsChecked = false;
            return;
        }

        PEditorFavorite.IsEnabled = true;

        try
        {
            PEditorFavorite.IsChecked = _lEngine.LEngineFavoriteCheck(entry.Value);
        }
        catch (Exception)
        {
            PEditorFavorite.IsChecked = false;
        }
    }

    private void PEditorFavoriteHandle(object sender, RoutedEventArgs e)
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorFavorite.IsChecked = false;
            return;
        }

        bool marked = PEditorFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lEngine.LEngineFavoriteSave(entry.Value);
            }
            else
            {
                _lEngine.LEngineFavoriteDelete(entry.Value);
            }
        }
        catch (Exception exception)
        {
            PEditorFavorite.IsChecked = !marked;
            _pEditorHost.PWindowFailureShow("Favorite.MarkFailed", exception);
        }
    }

    internal void PEditorGraspShow()
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorGrasp.IsEnabled = false;
            PEditorGrasp.PGraspStep = 0;
            PEditorGraspLabel.Text = string.Empty;
            return;
        }

        PEditorGrasp.IsEnabled = true;

        try
        {
            PEditorGrasp.PGraspStep = _lEngine.LEngineGraspRead(entry.Value);
        }
        catch (Exception)
        {
            PEditorGrasp.PGraspStep = 0;
        }

        PEditorGraspLabel.Text = PEditorGraspFormat(PEditorGrasp.PGraspStep);
    }

    private string PEditorGraspFormat(int step)
    {
        return PEditorEntryRead() is null
            ? string.Empty
            : _pEditorHost.PLocalizationTextRead(PGrasp.PGraspLabelResolve(step));
    }

    private void PEditorHoverHandle(object sender, RoutedEventArgs e)
    {
        PEditorGraspLabel.Text = PEditorGraspFormat(PEditorGrasp.PGraspHover ?? PEditorGrasp.PGraspStep);
    }

    private void PEditorGraspHandle(object sender, RoutedEventArgs e)
    {
        long? entry = PEditorEntryRead();

        if (entry is null)
        {
            PEditorGrasp.PGraspStep = 0;
            return;
        }

        try
        {
            _lEngine.LEngineGraspSave(entry.Value, PEditorGrasp.PGraspStep);
        }
        catch (Exception exception)
        {
            PEditorGraspShow();
            _pEditorHost.PWindowFailureShow("Grasp.MarkFailed", exception);
        }
    }

    private long? PEditorEntryRead()
    {
        if (_pEditorDraft == 0)
        {
            return null;
        }

        LDraft? held;
        try
        {
            held = _lEngine.LEngineDraftRead(_pEditorDraft);
        }
        catch (Exception)
        {
            return null;
        }

        return held?.LDraftEntryId is null or 0 ? null : held.LDraftEntryId;
    }

    private void PEditorStoreHandle(object sender, RoutedEventArgs e)
    {
        PEditorEntrySave();
    }

    internal void PEditorEntrySave()
    {
        PEditorChangeSave();

        long held = _pEditorDraft;
        if (held == 0 || _pEditorHalted)
        {
            return;
        }

        long? entry = PEditorEntryRead();
        _pEditorDraft = 0;

        LOutcome stored;
        try
        {
            stored = _pEditorHost.PWindowCommitRun(held, _lEngine.LEngineDraftCommit);
        }
        catch (Exception exception)
        {
            _pEditorDraft = held;
            _pEditorHost.PWindowFailureShow(entry is null ? "Input.SaveFailed" : "Input.UpdateFailed", exception);
            return;
        }

        if (entry is not null)
        {
            PEditorEntryShow(stored.LOutcomeEntry.LEntryId);
            return;
        }

        PEditorReset();
    }

    private void PEditorDiscardHandle(object sender, RoutedEventArgs e)
    {
        long? entry = PEditorEntryRead();

        PEditorChangeStop();
        PEditorDraftCancel();

        if (entry is null)
        {
            PEditorReset();
            return;
        }

        PEditorEntryShow(entry.Value);
    }
}
