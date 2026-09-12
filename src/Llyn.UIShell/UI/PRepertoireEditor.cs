using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private bool _pScenarioTitleUnreadable;

    private bool _pScenarioKindUnreadable;

    private bool _pScenarioDescriptionUnreadable;

    private bool _pScenarioLoading;

    private void PScenarioTitleHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioTitleUnreadable = false;
        PScenarioTitle.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnreadable = false;
        PScenarioKind.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnreadable = false;
        PScenarioDescription.Tag = string.Empty;
        PScenarioChangeDefer();
    }

    private void PScenarioApply(LSituation? situation)
    {
        _pScenarioLoading = true;

        string unreadable = _pRepertoireHost.PLocalizationTextRead("Display.Unreadable");

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnreadable = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pScenarioKindUnreadable = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pScenarioDescriptionUnreadable = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PScenarioTitle.Tag = _pScenarioTitleUnreadable ? unreadable : string.Empty;
        PScenarioKind.Tag = _pScenarioKindUnreadable ? unreadable : string.Empty;
        PScenarioDescription.Tag = _pScenarioDescriptionUnreadable ? unreadable : string.Empty;

        long? stored = PScenarioSituationRead();
        PScenarioRemoval.IsEnabled = stored is not null;

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private void PScenarioShow(LSituation situation)
    {
        _pScenarioLoading = true;

        string unreadable = _pRepertoireHost.PLocalizationTextRead("Display.Unreadable");

        PScenarioFieldShow(PScenarioTitle, situation.LSituationTitle, unreadable, ref _pScenarioTitleUnreadable);
        PScenarioFieldShow(PScenarioKind, situation.LSituationKind, unreadable, ref _pScenarioKindUnreadable);
        PScenarioFieldShow(
            PScenarioDescription, situation.LSituationDescription, unreadable, ref _pScenarioDescriptionUnreadable);

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private static void PScenarioFieldShow(TextBox field, LStateValue value, string unreadable, ref bool held)
    {
        if (LStateValue.LStateValueResolve(field.Text, held) == value)
        {
            return;
        }

        field.Text = value.LStateValueShow();
        held = value.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unreadable : string.Empty;
    }

    private LSituation PScenarioRead()
    {
        return new LSituation(
            0,
            LStateValue.LStateValueResolve(PScenarioTitle.Text, _pScenarioTitleUnreadable),
            LStateValue.LStateValueResolve(PScenarioDescription.Text, _pScenarioDescriptionUnreadable),
            LStateValue.LStateValueResolve(PScenarioKind.Text, _pScenarioKindUnreadable));
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
            stored = _lEngine.LEngineSituationCommit(held);
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
