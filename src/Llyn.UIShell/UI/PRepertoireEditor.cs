using System;
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
        PScenarioTitle.Tag = PScenarioHintRead(PScenarioTitle, false);
        PScenarioChangeDefer();
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnknown = false;
        PScenarioKind.Tag = PScenarioHintRead(PScenarioKind, false);
        PScenarioChangeDefer();
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnknown = false;
        PScenarioDescription.Tag = PScenarioHintRead(PScenarioDescription, false);
        PScenarioChangeDefer();
    }

    private string PScenarioHintRead(TextBox field, bool unknown)
    {
        string key = unknown
            ? "Display.Unknown"
            : ReferenceEquals(field, PScenarioTitle)
                ? "Situation.Untitled"
                : ReferenceEquals(field, PScenarioKind)
                    ? "Situation.Kind"
                    : "Situation.DescriptionHint";

        return _pRepertoireHost.PLocalizationTextRead(key);
    }

    private void PScenarioApply(LSituation? situation)
    {
        _pScenarioLoading = true;

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnknown = situation?.LSituationTitle.LStateValueState == LState.LStateUnknown;
        _pScenarioKindUnknown = situation?.LSituationKind.LStateValueState == LState.LStateUnknown;
        _pScenarioDescriptionUnknown = situation?.LSituationDescription.LStateValueState == LState.LStateUnknown;

        PScenarioTitle.Tag = PScenarioHintRead(PScenarioTitle, _pScenarioTitleUnknown);
        PScenarioKind.Tag = PScenarioHintRead(PScenarioKind, _pScenarioKindUnknown);
        PScenarioDescription.Tag = PScenarioHintRead(PScenarioDescription, _pScenarioDescriptionUnknown);

        PScenarioImageShow(situation?.LSituationImage ?? []);
        PScenarioVideoShow(situation?.LSituationVideo ?? []);

        PScenarioTally.Text = PRepertoireTallyRead(PScenarioSituationRead());

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private void PScenarioShow(LSituation situation)
    {
        _pScenarioLoading = true;

        PScenarioFieldShow(PScenarioTitle, situation.LSituationTitle, ref _pScenarioTitleUnknown);
        PScenarioFieldShow(PScenarioKind, situation.LSituationKind, ref _pScenarioKindUnknown);
        PScenarioFieldShow(PScenarioDescription, situation.LSituationDescription, ref _pScenarioDescriptionUnknown);

        PScenarioImageShow(situation.LSituationImage);
        PScenarioVideoShow(situation.LSituationVideo);

        _pScenarioLoading = false;

        PScenarioChangeUpdate();
    }

    private void PScenarioFieldShow(TextBox field, LStateValue value, ref bool held)
    {
        if (new LStateWritten(field.Text, held).LStateWrittenMatch(value))
        {
            return;
        }

        field.Text = value.LStateValueShow();
        held = value.LStateValueState == LState.LStateUnknown;
        field.Tag = PScenarioHintRead(field, held);
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

    private void PScenarioStoreRun()
    {
        PScenarioChangeSave();

        long held = _pScenarioDraft;
        if (held == 0 || _pScenarioHalted)
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
}
