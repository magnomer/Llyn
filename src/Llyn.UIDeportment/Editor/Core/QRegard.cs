using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegard
{
    private readonly FrameworkElement _qRegardSurface;

    private CDesk _cDesk = null!;

    private CEsteem _cEsteem = null!;

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

    internal void QRegardIntroduce(CDesk desk, CEsteem esteem)
    {
        _cDesk = desk;
        _cEsteem = esteem;
        esteem.CEsteemFavoriteChanged += QRegardFavoriteRefine;
        esteem.CEsteemGraspChanged += QRegardGraspRefine;
        esteem.CEsteemFrequencyChanged += QRegardFrequencyRefine;
        desk.CDeskStarted += QRegardFavoriteRefine;
        desk.CDeskStarted += QRegardGraspRefine;
        desk.CDeskStarted += QRegardFrequencyRefine;
        QRegardGrasp.PGraspLimit = esteem.CEsteemGraspStep;
    }

    private void QRegardFavoriteRefine()
    {
        QRegardFavorite.IsEnabled = _cDesk.CDeskStored;
        QRegardFavorite.IsChecked = _cEsteem.CEsteemFavorite;
    }

    private void QRegardFavoriteObserve(object sender, RoutedEventArgs e)
    {
        _cEsteem.CEsteemFavoriteSet(QLook.QLookCheckedRead(QRegardFavorite.IsChecked));
    }

    private void QRegardGraspRefine()
    {
        QRegardGrasp.IsEnabled = _cDesk.CDeskStored;
        QRegardGrasp.PGraspStep = _cEsteem.CEsteemGrasp;
        QRegardGraspLabel.Text = _cEsteem.CEsteemGraspRead(QRegardGrasp.PGraspStep);
    }

    private void QRegardHoverRefine(object sender, RoutedEventArgs e)
    {
        QRegardGraspLabel.Text = _cEsteem.CEsteemGraspRead(QRegardGrasp.PGraspPointed);
    }

    private void QRegardGraspObserve(object sender, RoutedEventArgs e)
    {
        _cEsteem.CEsteemGraspSet(QRegardGrasp.PGraspStep);
    }

    private void QRegardFrequencyRefine()
    {
        QFrequencyLabel.QFrequencyChipRefine(
            QRegardFrequencySection,
            QRegardFrequencyChip,
            QRegardFrequency,
            QRegardFrequencyBand,
            _cEsteem.CEsteemFrequencyRead(
                QLocalizationCatalog.QLocalizationTextRead("Frequency.Once")));
    }
}
