using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegard
{
    private readonly PEditor _pEditor;

    private CEditor _cEditor = null!;

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

    internal void QRegardIntroduce(CEditor editor)
    {
        _cEditor = editor;
        CEsteem esteem = editor.CEditorEsteem;
        esteem.CEsteemFavoriteChanged += QRegardFavoriteUpdate;
        esteem.CEsteemGraspChanged += QRegardGraspUpdate;
        esteem.CEsteemFrequencyChanged += QRegardFrequencyUpdate;
        editor.CEditorDesk.CDeskStarted += QRegardFavoriteUpdate;
        editor.CEditorDesk.CDeskStarted += QRegardGraspUpdate;
        editor.CEditorDesk.CDeskStarted += QRegardFrequencyUpdate;
        QRegardGrasp.PGraspLimit = esteem.CEsteemGraspStep;
    }

    private void QRegardFavoriteUpdate()
    {
        QRegardFavorite.IsEnabled = _cEditor.CEditorDesk.CDeskStored;
        QRegardFavorite.IsChecked = _cEditor.CEditorEsteem.CEsteemFavorite;
    }

    private void QRegardFavoriteHandle(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEsteem.CEsteemFavoriteSet(QLook.QLookCheckedRead(QRegardFavorite.IsChecked));
    }

    private void QRegardGraspUpdate()
    {
        QRegardGrasp.IsEnabled = _cEditor.CEditorDesk.CDeskStored;
        QRegardGrasp.PGraspStep = _cEditor.CEditorEsteem.CEsteemGrasp;
        QRegardGraspLabel.Text = _cEditor.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspStep);
    }

    private void QRegardHoverHandle(object sender, RoutedEventArgs e)
    {
        QRegardGraspLabel.Text = _cEditor.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspPointed);
    }

    private void QRegardGraspHandle(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEsteem.CEsteemGraspSet(QRegardGrasp.PGraspStep);
    }

    private void QRegardFrequencyUpdate()
    {
        QFrequencyLabel.QFrequencyChipRefine(
            QRegardFrequencySection,
            QRegardFrequencyChip,
            QRegardFrequency,
            QRegardFrequencyBand,
            _cEditor.CEditorEsteem.CEsteemFrequencyRead(
                QLocalizationCatalog.QLocalizationTextRead("Frequency.Once")));
    }
}
