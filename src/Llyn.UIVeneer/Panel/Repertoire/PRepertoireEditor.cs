using System;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

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

        return PLocalizationCatalog.PLocalizationTextRead(key);
    }

    private void PScenarioApply(LSituation? situation)
    {
        _pScenarioLoading = true;

        PScenarioTitle.Text = situation?.LSituationTitle.LStateValueShow() ?? string.Empty;
        PScenarioKind.Text = situation?.LSituationKind.LStateValueShow() ?? string.Empty;
        PScenarioDescription.Text = situation?.LSituationDescription.LStateValueShow() ?? string.Empty;

        _pScenarioTitleUnknown = situation?.LSituationTitle.LStateValueUncertain ?? false;
        _pScenarioKindUnknown = situation?.LSituationKind.LStateValueUncertain ?? false;
        _pScenarioDescriptionUnknown = situation?.LSituationDescription.LStateValueUncertain ?? false;

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
        bool uncertain = value.LStateValueUncertain;
        held = uncertain;
        field.Tag = PScenarioHintRead(field, held);
    }

    private LRequestSituationBody PScenarioRead(long draft)
    {
        return new LRequestSituationBody(
            draft,
            0,
            new LStateWritten(PScenarioTitle.Text, _pScenarioTitleUnknown),
            new LStateWritten(PScenarioDescription.Text, _pScenarioDescriptionUnknown),
            new LStateWritten(PScenarioKind.Text, _pScenarioKindUnknown));
    }

    private void PScenarioStoreRun()
    {
        if (!PScenarioDesk.LDeskChangeCheck())
        {
            return;
        }

        PScenarioDesk.LDeskFinish(true);
    }

    private void PScenarioStoredShow(long situation)
    {
        _lRepertoire.LRepertoireSelect(situation);

        PAtlasFind();
        PRepertoireScribeShow(false);
        PRepertoireShow(situation);
    }
}
