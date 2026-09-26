using System;
using System.Windows.Controls;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

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
        PScenarioTitle.SetValue(QField.QFieldHintProperty, PScenarioHintRead(PScenarioTitle, false));
        PScenarioHintApply();
        PScenarioChangeDefer();
    }

    private void PScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioKindUnknown = false;
        PScenarioKind.SetValue(QField.QFieldHintProperty, PScenarioHintRead(PScenarioKind, false));
        PScenarioMeasureApply();
        PScenarioChangeDefer();
    }

    private void PScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        if (_pScenarioLoading)
        {
            return;
        }

        _pScenarioDescriptionUnknown = false;
        PScenarioDescription.SetValue(QField.QFieldHintProperty, PScenarioHintRead(PScenarioDescription, false));
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

        return QLocalizationCatalog.QLocalizationTextRead(key);
    }

    private void PScenarioHintApply()
    {
        PScenarioHint.Text = (string)PScenarioTitle.GetValue(QField.QFieldHintProperty);
        PScenarioHint.Visibility = QLook.QLookVisibleRead(PScenarioTitle.Text.Length == 0);
        PScenarioGhost.Text = PScenarioTitle.Text;
    }

    private void PScenarioMeasureApply()
    {
        PScenarioMeasure.Text = PScenarioKind.Text.Length == 0
            ? (string)PScenarioKind.GetValue(QField.QFieldHintProperty)
            : PScenarioKind.Text;
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

        PScenarioTitle.SetValue(QField.QFieldHintProperty, PScenarioHintRead(PScenarioTitle, _pScenarioTitleUnknown));
        PScenarioKind.SetValue(QField.QFieldHintProperty, PScenarioHintRead(PScenarioKind, _pScenarioKindUnknown));
        PScenarioDescription.SetValue(
            QField.QFieldHintProperty, PScenarioHintRead(PScenarioDescription, _pScenarioDescriptionUnknown));

        PScenarioImageShow(situation?.LSituationImage ?? []);
        PScenarioVideoShow(situation?.LSituationVideo ?? []);

        PScenarioTally.Text = PRepertoireTallyRead(PScenarioDesk.LDeskStoredRead());
        PScenarioHintApply();
        PScenarioMeasureApply();

        _pScenarioLoading = false;
    }

    private void PScenarioShow(LSituation situation)
    {
        _pScenarioLoading = true;

        PScenarioFieldShow(PScenarioTitle, situation.LSituationTitle, ref _pScenarioTitleUnknown);
        PScenarioFieldShow(PScenarioKind, situation.LSituationKind, ref _pScenarioKindUnknown);
        PScenarioFieldShow(PScenarioDescription, situation.LSituationDescription, ref _pScenarioDescriptionUnknown);

        PScenarioImageShow(situation.LSituationImage);
        PScenarioVideoShow(situation.LSituationVideo);
        PScenarioHintApply();
        PScenarioMeasureApply();

        _pScenarioLoading = false;
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
        field.SetValue(QField.QFieldHintProperty, PScenarioHintRead(field, held));
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
}
