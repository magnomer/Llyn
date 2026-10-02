using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegard
{
    private readonly FrameworkElement _qRegardSurface;

    private CEditor _cEditor = null!;

    internal QRegard(FrameworkElement surface)
    {
        _qRegardSurface = surface;
        QRegardFavorite.Click += QRegardFavoriteObserve;
        QRegardGrasp.PGraspChanged += QRegardGraspObserve;
        QRegardGrasp.PGraspHovered += QRegardHoverRefine;
    }

    private ToggleButton QRegardFavorite => QContract.QContractFind<ToggleButton>(_qRegardSurface, "PEditorFavorite");

    private PGrasp QRegardGrasp => QContract.QContractFind<PGrasp>(_qRegardSurface, "PEditorGrasp");

    private TextBlock QRegardGraspLabel => QContract.QContractFind<TextBlock>(_qRegardSurface, "PEditorGraspLabel");

    private StackPanel QRegardFrequencySection =>
        QContract.QContractFind<StackPanel>(_qRegardSurface, "PEditorFrequencySection");

    private Border QRegardFrequencyChip => QContract.QContractFind<Border>(_qRegardSurface, "PEditorFrequencyChip");

    private TextBlock QRegardFrequency => QContract.QContractFind<TextBlock>(_qRegardSurface, "PEditorFrequency");

    private TextBlock QRegardFrequencyBand =>
        QContract.QContractFind<TextBlock>(_qRegardSurface, "PEditorFrequencyBand");

    internal void QRegardIntroduce(CEditor editor)
    {
        _cEditor = editor;
        CEsteem esteem = editor.CEditorEsteem;
        esteem.CEsteemFavoriteChanged += QRegardFavoriteRefine;
        esteem.CEsteemGraspChanged += QRegardGraspRefine;
        esteem.CEsteemFrequencyChanged += QRegardFrequencyRefine;
        editor.CEditorDesk.CDeskStarted += QRegardFavoriteRefine;
        editor.CEditorDesk.CDeskStarted += QRegardGraspRefine;
        editor.CEditorDesk.CDeskStarted += QRegardFrequencyRefine;
        QRegardGrasp.PGraspLimit = esteem.CEsteemGraspStep;
    }

    private void QRegardFavoriteRefine()
    {
        QRegardFavorite.IsEnabled = _cEditor.CEditorDesk.CDeskStored;
        QRegardFavorite.IsChecked = _cEditor.CEditorEsteem.CEsteemFavorite;
    }

    private void QRegardFavoriteObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEsteem.CEsteemFavoriteSet(QLook.QLookCheckedRead(QRegardFavorite.IsChecked));
    }

    private void QRegardGraspRefine()
    {
        QRegardGrasp.IsEnabled = _cEditor.CEditorDesk.CDeskStored;
        QRegardGrasp.PGraspStep = _cEditor.CEditorEsteem.CEsteemGrasp;
        QRegardGraspLabel.Text = _cEditor.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspStep);
    }

    private void QRegardHoverRefine(object sender, RoutedEventArgs e)
    {
        QRegardGraspLabel.Text = _cEditor.CEditorEsteem.CEsteemGraspRead(QRegardGrasp.PGraspPointed);
    }

    private void QRegardGraspObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorEsteem.CEsteemGraspSet(QRegardGrasp.PGraspStep);
    }

    private void QRegardFrequencyRefine()
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
