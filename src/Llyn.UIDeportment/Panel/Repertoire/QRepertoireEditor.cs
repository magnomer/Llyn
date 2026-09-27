using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private void QScenarioAttach()
    {
        QScenarioTitle.TextChanged += QScenarioTitleHandle;
        QScenarioKind.TextChanged += QScenarioKindHandle;
        QScenarioDescription.TextChanged += QScenarioDescriptionHandle;
    }

    private void QScenarioDetach()
    {
        QScenarioTitle.TextChanged -= QScenarioTitleHandle;
        QScenarioKind.TextChanged -= QScenarioKindHandle;
        QScenarioDescription.TextChanged -= QScenarioDescriptionHandle;
    }

    private void QScenarioTitleHandle(object sender, TextChangedEventArgs e)
    {
        QScenarioTitle.SetValue(QField.QFieldHintProperty, QScenarioHintRead("Situation.Untitled", false));
        QScenarioHintApply();
        QScenarioChangeDefer();
    }

    private void QScenarioKindHandle(object sender, TextChangedEventArgs e)
    {
        QScenarioKind.SetValue(QField.QFieldHintProperty, QScenarioHintRead("Situation.Kind", false));
        QScenarioMeasureApply();
        QScenarioChangeDefer();
    }

    private void QScenarioDescriptionHandle(object sender, TextChangedEventArgs e)
    {
        QScenarioDescription.SetValue(
            QField.QFieldHintProperty, QScenarioHintRead("Situation.DescriptionHint", false));
        QScenarioChangeDefer();
    }

    private static string QScenarioHintRead(string key, bool unknown)
    {
        return QLocalizationCatalog.QLocalizationTextRead(unknown ? "Display.Unknown" : key);
    }

    private void QScenarioHintApply()
    {
        QScenarioHint.Text = (string)QScenarioTitle.GetValue(QField.QFieldHintProperty);
        QScenarioHint.Visibility = QLook.QLookVisibleRead(_lRepertoire.LRepertoireVacantCheck(QScenarioTitle.Text));
        QScenarioGhost.Text = QScenarioTitle.Text;
    }

    private void QScenarioMeasureApply()
    {
        QScenarioMeasure.Text = _lRepertoire.LRepertoireMeasureRead(
            QScenarioKind.Text, (string)QScenarioKind.GetValue(QField.QFieldHintProperty));
    }

    private void QScenarioApply(CSituationDraft? situation)
    {
        QScenarioShow(
            situation ?? new CSituationDraft(
                0, CStateValue.CStateValueEmpty, CStateValue.CStateValueEmpty, CStateValue.CStateValueEmpty, [], []));
        QScenarioTally.Text = QRepertoireTallyRead(QScenarioDesk.CDeskStoredRead());
    }

    private void QScenarioShow(CSituationDraft situation)
    {
        QScenarioDetach();

        QScenarioFieldShow(QScenarioTitle, "Situation.Untitled", situation.CSituationDraftTitle);
        QScenarioFieldShow(QScenarioKind, "Situation.Kind", situation.CSituationDraftKind);
        QScenarioFieldShow(QScenarioDescription, "Situation.DescriptionHint", situation.CSituationDraftDescription);

        QScenarioImageShow(situation.CSituationDraftImage);
        QScenarioVideoShow(situation.CSituationDraftVideo);
        QScenarioHintApply();
        QScenarioMeasureApply();

        QScenarioAttach();
    }

    private static void QScenarioFieldShow(TextBox field, string key, CStateValue value)
    {
        field.Text = value.CStateValueText;
        field.SetValue(QField.QFieldHintProperty, QScenarioHintRead(key, value.CStateValueUncertain));
    }
}
