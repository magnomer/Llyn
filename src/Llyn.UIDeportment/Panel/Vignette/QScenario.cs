using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QScenario
{
    private readonly UserControl _qScenarioScope;

    private readonly QImage _qScenarioImage = new();

    private readonly QVideo _qScenarioVideo = new();

    private CRepertoire _cRepertoire = null!;

    private CPlaywright _cPlaywright = null!;

    internal QScenario(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qScenarioScope = scope;

        QScenarioPictureIcon.QIconSource = QIcon.QIconResolve("image", 24);
        QScenarioFilmIcon.QIconSource = QIcon.QIconResolve("video", 24);

        QScenarioIntroduce();
    }

    private Grid QScenarioView => QContract.QContractFind<Grid>(_qScenarioScope, "PScenario");

    private TextBlock QScenarioHint => QContract.QContractFind<TextBlock>(_qScenarioScope, "PScenarioHint");

    private TextBlock QScenarioGhost => QContract.QContractFind<TextBlock>(_qScenarioScope, "PScenarioGhost");

    private TextBox QScenarioTitle => QContract.QContractFind<TextBox>(_qScenarioScope, "PScenarioTitle");

    private TextBlock QScenarioMeasure => QContract.QContractFind<TextBlock>(_qScenarioScope, "PScenarioMeasure");

    private TextBox QScenarioKind => QContract.QContractFind<TextBox>(_qScenarioScope, "PScenarioKind");

    private TextBlock QScenarioTally => QContract.QContractFind<TextBlock>(_qScenarioScope, "PScenarioTally");

    private TextBox QScenarioDescription =>
        QContract.QContractFind<TextBox>(_qScenarioScope, "PScenarioDescription");

    private ItemsControl QScenarioImage => QContract.QContractFind<ItemsControl>(_qScenarioScope, "PScenarioImage");

    private ItemsControl QScenarioVideo => QContract.QContractFind<ItemsControl>(_qScenarioScope, "PScenarioVideo");

    private Button QScenarioPicture => QContract.QContractFind<Button>(_qScenarioScope, "PScenarioPicture");

    private QIconImage QScenarioPictureIcon =>
        QContract.QContractFind<QIconImage>(_qScenarioScope, "PScenarioPictureIcon");

    private Button QScenarioFilm => QContract.QContractFind<Button>(_qScenarioScope, "PScenarioFilm");

    private QIconImage QScenarioFilmIcon =>
        QContract.QContractFind<QIconImage>(_qScenarioScope, "PScenarioFilmIcon");

    internal void QScenarioDeskIntroduce(CRepertoire repertoire)
    {
        ArgumentNullException.ThrowIfNull(repertoire);

        _cRepertoire = repertoire;
        _cPlaywright = repertoire.CRepertoirePlaywright;
        _qScenarioImage.QImageIntroduce(_cPlaywright.CPlaywrightImage);
        _qScenarioVideo.QVideoIntroduce(_cPlaywright.CPlaywrightVideo);
        _qScenarioImage.QImageRowIntroduce(QScenarioImage, QScenarioPicture);
        _qScenarioVideo.QVideoRowIntroduce(QScenarioVideo, QScenarioFilm);

        _cPlaywright.CPlaywrightDraftChanged += QScenarioFieldsRefine;
        _cPlaywright.CPlaywrightScenarioChanged += QScenarioRefine;
        _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureRowsChanged += QScenarioTallyRefine;
    }

    internal void QScenarioVisibleRefine()
    {
        QScenarioView.Visibility = QLook.QLookVisibleRead(_cRepertoire.CRepertoireDiptych.CDiptychParentEditing);
        QScenarioView.IsEnabled = _cRepertoire.CRepertoireScenarioEnabled;
    }

    private void QScenarioIntroduce()
    {
        QScenarioTitle.TextChanged += QScenarioTitleObserve;
        QScenarioKind.TextChanged += QScenarioKindObserve;
        QScenarioDescription.TextChanged += QScenarioDescriptionObserve;
    }

    private void QScenarioTeardown()
    {
        QScenarioTitle.TextChanged -= QScenarioTitleObserve;
        QScenarioKind.TextChanged -= QScenarioKindObserve;
        QScenarioDescription.TextChanged -= QScenarioDescriptionObserve;
    }

    private void QScenarioTitleObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioTitleRefine(_cPlaywright.CPlaywrightTitleSet(QScenarioTitle.Text));
    }

    private void QScenarioKindObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioKindRefine(_cPlaywright.CPlaywrightKindSet(QScenarioKind.Text));
    }

    private void QScenarioDescriptionObserve(object sender, TextChangedEventArgs e)
    {
        QScenarioDescriptionRefine(
            _cPlaywright.CPlaywrightDescriptionSet(QScenarioDescription.Text));
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
        QScenarioTallyRefine();
    }

    private void QScenarioFieldsRefine(CScenario scenario)
    {
        QScenarioTeardown();

        QScenarioFieldRefine(QScenarioTitle, scenario.CScenarioTitle);
        QScenarioFieldRefine(QScenarioKind, scenario.CScenarioKind);
        QScenarioFieldRefine(QScenarioDescription, scenario.CScenarioDescription);

        _qScenarioImage.QImageRowRefine(scenario.CScenarioDraft.CSituationDraftImage);
        _qScenarioVideo.QVideoRowRefine(scenario.CScenarioDraft.CSituationDraftVideo);
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

    private void QScenarioTallyRefine()
    {
        QScenarioTally.Text = _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture.CApertureTallyRead();
    }
}
