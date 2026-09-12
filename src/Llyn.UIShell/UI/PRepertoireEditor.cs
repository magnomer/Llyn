using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private bool _pScenarioTitleUnknown;

    private bool _pScenarioKindUnknown;

    private bool _pScenarioDescriptionUnknown;

    private bool _pScenarioLoading;

    private void PScenarioTitleHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioTitleUnknown = false;
        PScenarioTitle.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnknown = false;
        PScenarioKind.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnknown = false;
        PScenarioDescription.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioApply(LSituation? situation)
    {
        _pScenarioLoading = true;

        string unknown = _pRepertoireHost.PLocalizationTextRead("Display.Unknown");

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnknown = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pScenarioKindUnknown = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pScenarioDescriptionUnknown = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PScenarioTitle.Tag = _pScenarioTitleUnknown ? unknown : string.Empty;
        PScenarioKind.Tag = _pScenarioKindUnknown ? unknown : string.Empty;
        PScenarioDescription.Tag = _pScenarioDescriptionUnknown ? unknown : string.Empty;

        long? stored = PScenarioSituationRead();
        PScenarioRemoval.IsEnabled = stored is not null;

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private void PScenarioShow(LSituation situation)
    {
        _pScenarioLoading = true;

        string unknown = _pRepertoireHost.PLocalizationTextRead("Display.Unknown");

        PScenarioFieldShow(PScenarioTitle, situation.LSituationTitle, unknown, ref _pScenarioTitleUnknown);
        PScenarioFieldShow(PScenarioKind, situation.LSituationKind, unknown, ref _pScenarioKindUnknown);
        PScenarioFieldShow(
            PScenarioDescription, situation.LSituationDescription, unknown, ref _pScenarioDescriptionUnknown);

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private static void PScenarioFieldShow(TextBox field, LStateValue value, string unknown, ref bool held)
    {
        if (new LStateWritten(field.Text, held).LStateWrittenMatch(value))
        {
            return;
        }

        field.Text = value.LStateValueShow();
        held = value.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unknown : string.Empty;
    }

    private LRequestSituationBody PScenarioRead(long draft, long situation)
    {
        return new LRequestSituationBody(
            draft,
            situation,
            new LStateWritten(PScenarioTitle.Text, _pScenarioTitleUnknown),
            new LStateWritten(PScenarioDescription.Text, _pScenarioDescriptionUnknown),
            new LStateWritten(PScenarioKind.Text, _pScenarioKindUnknown));
    }

    private void PScenarioDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PRepertoireLeaveConfirm())
        {
            return;
        }

        PScenarioDraftShow(PScenarioDraftStart(PScenarioSituationRead()));
    }

    private void PScenarioStoreHandle(object sender, RoutedEventArgs e)
    {
        PScenarioChangeSave();

        long held = _pScenarioDraft;
        if (held == 0)
        {
            return;
        }

        LSituation stored;
        try
        {
            stored = _pRepertoireHost.PWindowCommitRun(held, _lEngine.LEngineSituationCommit);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.SaveFailed", exception);
            return;
        }

        _pScenarioDraft = 0;
        _pVignetteSituation = stored.LSituationId;
        PAtlasSelect(stored.LSituationId);

        PAtlasFind(PInquest.Text ?? string.Empty);
        PRepertoireScribeShow(false);
        PRepertoireShow(stored.LSituationId);
    }

    private void PScenarioRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (PScenarioSituationRead() is not long id)
        {
            return;
        }

        int usage = _pAtlasCount.TryGetValue(id, out int count) ? count : 0;

        if (!_pRepertoireHost.PWindowRemovalConfirm(usage))
        {
            return;
        }

        try
        {
            _lEngine.LEngineSituationDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pRepertoireHost.PWindowFailureShow("Situation.DeleteFailed", exception);
            return;
        }

        PRepertoireScribeShow(false);
        PRepertoireClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }
}
