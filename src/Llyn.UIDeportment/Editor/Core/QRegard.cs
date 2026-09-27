using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Llyn.UIDeportment;

internal sealed class QRegard
{
    private readonly PEditor _pEditor;

    private LEditor _lEditor = null!;

    internal QRegard(PEditor editor)
    {
        _pEditor = editor;
        QRegardFavorite.Click += QRegardFavoriteHandle;
        QRegardGrasp.PGraspChanged += QRegardGraspHandle;
        QRegardGrasp.PGraspHovered += QRegardHoverHandle;
    }

    private ToggleButton QRegardFavorite => QContract.QContractFind<ToggleButton>(_pEditor, "PEditorFavorite");

    private PGrasp QRegardGrasp => QContract.QContractFind<PGrasp>(_pEditor, "PEditorGrasp");

    private TextBlock QRegardGraspLabel => QContract.QContractFind<TextBlock>(_pEditor, "PEditorGraspLabel");

    private StackPanel QRegardFrequencySection =>
        QContract.QContractFind<StackPanel>(_pEditor, "PEditorFrequencySection");

    private Border QRegardFrequencyChip => QContract.QContractFind<Border>(_pEditor, "PEditorFrequencyChip");

    private TextBlock QRegardFrequency => QContract.QContractFind<TextBlock>(_pEditor, "PEditorFrequency");

    private TextBlock QRegardFrequencyBand => QContract.QContractFind<TextBlock>(_pEditor, "PEditorFrequencyBand");

    internal void QRegardAttach(LEditor editor)
    {
        _lEditor = editor;
        _lEditor.LEditorEsteem.QEsteemFavoriteChanged += QRegardFavoriteUpdate;
        _lEditor.LEditorEsteem.QEsteemGraspChanged += QRegardGraspUpdate;
        QRegardGrasp.PGraspLimit = _lEditor.LEditorEsteem.QEsteemGraspStep;
    }

    internal void QRegardUpdate()
    {
        QRegardFavoriteUpdate();
        QRegardGraspUpdate();
        QRegardFrequencyUpdate();
    }

    private void QRegardFavoriteUpdate()
    {
        QRegardFavorite.IsEnabled = _lEditor.LEditorDesk.LDeskStored;
        QRegardFavorite.IsChecked = _lEditor.LEditorEsteem.QEsteemFavorite;
    }

    private void QRegardFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorEsteem.QEsteemFavoriteSet(QLook.QLookCheckedRead(QRegardFavorite.IsChecked));
    }

    internal void QRegardGraspUpdate()
    {
        QRegardGrasp.IsEnabled = _lEditor.LEditorDesk.LDeskStored;
        QRegardGrasp.PGraspStep = _lEditor.LEditorEsteem.QEsteemGrasp;
        QRegardGraspLabel.Text = _lEditor.LEditorEsteem.QEsteemGraspFormat(QRegardGrasp.PGraspStep);
    }

    private void QRegardHoverHandle(object sender, RoutedEventArgs e)
    {
        QRegardGraspLabel.Text = _lEditor.LEditorEsteem.QEsteemGraspFormat(QRegardGrasp.PGraspPointed);
    }

    private void QRegardGraspHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorEsteem.QEsteemGraspSet(QRegardGrasp.PGraspStep);
    }

    internal void QRegardFrequencyUpdate()
    {
        LFrequencyLabel.LFrequencyChipShow(
            QRegardFrequencySection,
            QRegardFrequencyChip,
            QRegardFrequency,
            QRegardFrequencyBand,
            _lEditor.LEditorEsteem.QEsteemFrequencyRead(QLocalizationCatalog.QLocalizationTextRead("Frequency.Once")));
    }
}
