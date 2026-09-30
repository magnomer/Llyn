using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private void QScenarioIntroduce()
    {
        QScenarioTitle.TextChanged += QScenarioTitleObserve;
        QScenarioKind.TextChanged += QScenarioKindObserve;
        QScenarioDescription.TextChanged += QScenarioDescriptionObserve;
    }

    private void QScenarioDetach()
    {
        QScenarioTitle.TextChanged -= QScenarioTitleObserve;
        QScenarioKind.TextChanged -= QScenarioKindObserve;
        QScenarioDescription.TextChanged -= QScenarioDescriptionObserve;
    }

    private void QScenarioTitleObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioTitleRefine(_cRepertoire.CRepertoireTitleSet(QScenarioTitle.Text));
    }

    private void QScenarioKindObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioKindRefine(_cRepertoire.CRepertoireKindSet(QScenarioKind.Text));
    }

    private void QScenarioDescriptionObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioDescriptionRefine(_cRepertoire.CRepertoireDescriptionSet(QScenarioDescription.Text));
    }

    private void QScenarioTitleRefine(CScenarioLine line)
    {
        QScenarioTitle.SetValue(
            QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(line.CScenarioLineHint));
        QScenarioHintRefine(line);
    }

    private void QScenarioKindRefine(CScenarioLine line)
    {
        QScenarioKind.SetValue(
            QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(line.CScenarioLineHint));
        QScenarioMeasureRefine(line);
    }

    private void QScenarioDescriptionRefine(CScenarioLine line)
    {
        QScenarioDescription.SetValue(
            QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(line.CScenarioLineHint));
    }

    private void QScenarioHintRefine(CScenarioLine title)
    {
        QScenarioHint.Text = QLocalizationCatalog.QLocalizationTextRead(title.CScenarioLineHint);
        QScenarioHint.Visibility = QLook.QLookVisibleRead(title.CScenarioLineVacant);
        QScenarioGhost.Text = title.CScenarioLineText;
    }

    private void QScenarioMeasureRefine(CScenarioLine kind)
    {
        QScenarioMeasure.Text = kind.CScenarioLineWording is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : kind.CScenarioLineText;
    }

    private void QScenarioRefine(CScenario scenario)
    {
        QScenarioFieldsRefine(scenario);
        QRepertoireTallyRefine();
    }

    private void QScenarioFieldsRefine(CScenario scenario)
    {
        QScenarioDetach();

        QScenarioFieldRefine(QScenarioTitle, scenario.CScenarioTitle);
        QScenarioFieldRefine(QScenarioKind, scenario.CScenarioKind);
        QScenarioFieldRefine(QScenarioDescription, scenario.CScenarioDescription);

        QScenarioImageRefine(scenario.CScenarioDraft.CSituationDraftImage);
        QScenarioVideoRefine(scenario.CScenarioDraft.CSituationDraftVideo);
        QScenarioHintRefine(scenario.CScenarioTitle);
        QScenarioMeasureRefine(scenario.CScenarioKind);

        QScenarioIntroduce();
    }

    private static void QScenarioFieldRefine(TextBox field, CScenarioLine line)
    {
        field.Text = line.CScenarioLineText;
        field.SetValue(
            QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(line.CScenarioLineHint));
    }
}
