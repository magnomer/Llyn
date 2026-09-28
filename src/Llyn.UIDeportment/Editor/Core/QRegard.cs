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
        _lEditor.LEditorStudio.CEditorEsteem.CEsteemFavoriteChanged += QRegardFavoriteUpdate;
        _lEditor.LEditorStudio.CEditorEsteem.CEsteemGraspChanged += QRegardGraspUpdate;
        QRegardGrasp.PGraspLimit = _lEditor.LEditorStudio.CEditorEsteem.CEsteemGraspStep;
    }

    internal void QRegardUpdate()
    {
        QRegardFavoriteUpdate();
        QRegardGraspUpdate();
        QRegardFrequencyUpdate();
    }

    private void QRegardFavoriteUpdate()
    {
        QRegardFavorite.IsEnabled = _lEditor.LEditorStudio.CEditorDesk.CDeskStored;
        QRegardFavorite.IsChecked = _lEditor.LEditorStudio.CEditorEsteem.CEsteemFavorite;
    }

    private void QRegardFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorStudio.CEditorEsteem.CEsteemFavoriteSet(QLook.QLookCheckedRead(QRegardFavorite.IsChecked));
    }

    internal void QRegardGraspUpdate()
    {
        QRegardGrasp.IsEnabled = _lEditor.LEditorStudio.CEditorDesk.CDeskStored;
        QRegardGrasp.PGraspStep = _lEditor.LEditorStudio.CEditorEsteem.CEsteemGrasp;
        QRegardGraspLabel.Text = _lEditor.LEditorStudio.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspStep);
    }

    private void QRegardHoverHandle(object sender, RoutedEventArgs e)
    {
        QRegardGraspLabel.Text = _lEditor.LEditorStudio.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspPointed);
    }

    private void QRegardGraspHandle(object sender, RoutedEventArgs e)
    {
        _lEditor.LEditorStudio.CEditorEsteem.CEsteemGraspSet(QRegardGrasp.PGraspStep);
    }

    internal void QRegardFrequencyUpdate()
    {
        LFrequencyLabel.LFrequencyChipShow(
            QRegardFrequencySection,
            QRegardFrequencyChip,
            QRegardFrequency,
            QRegardFrequencyBand,
            _lEditor.LEditorStudio.CEditorEsteem.CEsteemFrequencyRead(
                QLocalizationCatalog.QLocalizationTextRead("Frequency.Once")));
    }
}
