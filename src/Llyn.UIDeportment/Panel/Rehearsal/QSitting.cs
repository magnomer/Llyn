using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSitting
{
    private readonly Border _qSittingSurface;

    private CStudy _cStudy = null!;

    internal QSitting(Border surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qSittingSurface = surface;
        QSittingPrevious.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QSittingSide.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("duplex", 24));
        QSittingNext.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QSittingAgain.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QSittingEnd.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("close", 24));
        QSittingPrevious.Click += QSittingPreviousObserve;
        QSittingSide.Click += QSittingSideObserve;
        QSittingNext.Click += QSittingNextObserve;
        QSittingAgain.Click += QSittingAgainObserve;
        QSittingEnd.Click += QSittingEndObserve;
        _qSittingSurface.KeyDown += QSittingKeyObserve;
        _qSittingSurface.KeyDown += QSittingEscapeObserve;
        _qSittingSurface.MouseLeftButtonDown += QSittingPressObserve;
    }

    private TextBlock QSittingPosition => QContract.QContractFind<TextBlock>(_qSittingSurface, "PSittingPosition");

    private ContentControl QSittingFront => QContract.QContractFind<ContentControl>(_qSittingSurface, "PSittingFront");

    private Border QSittingRule => QContract.QContractFind<Border>(_qSittingSurface, "PSittingRule");

    private ContentControl QSittingBack => QContract.QContractFind<ContentControl>(_qSittingSurface, "PSittingBack");

    private TextBlock QSittingBlank => QContract.QContractFind<TextBlock>(_qSittingSurface, "PSittingBlank");

    private TextBlock QSittingSummary => QContract.QContractFind<TextBlock>(_qSittingSurface, "PSittingSummary");

    private Button QSittingPrevious => QContract.QContractFind<Button>(_qSittingSurface, "PSittingPrevious");

    private Button QSittingSide => QContract.QContractFind<Button>(_qSittingSurface, "PSittingSide");

    private Button QSittingNext => QContract.QContractFind<Button>(_qSittingSurface, "PSittingNext");

    private Button QSittingAgain => QContract.QContractFind<Button>(_qSittingSurface, "PSittingAgain");

    private Button QSittingEnd => QContract.QContractFind<Button>(_qSittingSurface, "PSittingEnd");

    internal void QSittingIntroduce(CStudy study)
    {
        ArgumentNullException.ThrowIfNull(study);

        _cStudy = study;
    }

    internal void QSittingRefine(CSittingState? state)
    {
        _qSittingSurface.Visibility = QLook.QLookVisibleRead(state is not null);
        if (state is null)
        {
            return;
        }

        QSittingPosition.Text = state.CSittingStatePosition;
        QSittingFaceRefine(QSittingFront, state.CSittingStateFront, QSittingFrontObserve);
        QSittingFaceRefine(QSittingBack, state.CSittingStateBack, QSittingBackObserve);
        if (state.CSittingStateBlank is string blank)
        {
            QSittingBlank.SetResourceReference(TextBlock.TextProperty, blank);
        }
        else
        {
            QSittingBlank.ClearValue(TextBlock.TextProperty);
        }

        Visibility running = QLook.QLookVisibleRead(!state.CSittingStateEnd);
        Visibility ended = QLook.QLookVisibleRead(state.CSittingStateEnd);
        QSittingFront.Visibility = running;
        QSittingRule.Visibility = running;
        QSittingBack.Visibility = QLook.QLookVisibleRead(state.CSittingStateTurned);
        QSittingBlank.Visibility = QLook.QLookVisibleRead(state.CSittingStateBlank is not null);
        QSittingPrevious.Visibility = running;
        QSittingSide.Visibility = running;
        QSittingNext.Visibility = running;
        QSittingSummary.Visibility = ended;
        QSittingAgain.Visibility = ended;
    }

    internal void QSittingFocusRefine()
    {
        _qSittingSurface.Focus();
    }

    private static void QSittingFaceRefine(ContentControl face, CSittingSide side, RoutedEventHandler play)
    {
        if (QLook.QLookPartFind<TextBlock>(face, "PFaceText") is TextBlock text)
        {
            text.Text = side.CSittingSideText;
        }

        if (QLook.QLookPartFind<ItemsControl>(face, "PFacePicture") is ItemsControl picture)
        {
            QLookItem.QLookItemAttach(picture, QImageItem.QImageItemApply);
            picture.ItemsSource = side.CSittingSidePictures.Select(static draft => new QImageItem(draft)).ToList();
        }

        if (QLook.QLookPartFind<Button>(face, "PFaceSound") is Button sound)
        {
            sound.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
            sound.Visibility = QLook.QLookVisibleRead(side.CSittingSideAudible);
            sound.Click -= play;
            sound.Click += play;
        }
    }

    private void QSittingStepRefine(Func<CSitting, CSittingState> step)
    {
        if (_cStudy.CStudySitting is CSitting sitting)
        {
            QSittingRefine(step(sitting));
        }
    }

    private void QSittingPreviousObserve(object sender, RoutedEventArgs e)
    {
        QSittingStepRefine(static sitting => sitting.CSittingRetreat());
    }

    private void QSittingSideObserve(object sender, RoutedEventArgs e)
    {
        QSittingStepRefine(static sitting => sitting.CSittingSideToggle());
    }

    private void QSittingNextObserve(object sender, RoutedEventArgs e)
    {
        QSittingStepRefine(static sitting => sitting.CSittingAdvance());
    }

    private void QSittingAgainObserve(object sender, RoutedEventArgs e)
    {
        _cStudy.CStudyStart();
        QSittingFocusRefine();
    }

    private void QSittingFrontObserve(object sender, RoutedEventArgs e)
    {
        _cStudy.CStudySoundPlay(false);
    }

    private void QSittingBackObserve(object sender, RoutedEventArgs e)
    {
        _cStudy.CStudySoundPlay(true);
    }

    private void QSittingEndObserve(object sender, RoutedEventArgs e)
    {
        _cStudy.CStudyFinish();
    }

    private void QSittingPressObserve(object sender, MouseButtonEventArgs e)
    {
        QSittingFocusRefine();
    }

    private void QSittingKeyObserve(object sender, KeyEventArgs e)
    {
        Func<CSitting, CSittingState>? step = e.Key switch
        {
            Key.Space => static sitting => sitting.CSittingSideToggle(),
            Key.Left => static sitting => sitting.CSittingRetreat(),
            Key.Right => static sitting => sitting.CSittingAdvance(),
            _ => null,
        };

        if (step is not null)
        {
            QSittingStepRefine(step);
            e.Handled = true;
        }
    }

    private void QSittingEscapeObserve(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _cStudy.CStudyFinish();
            e.Handled = true;
        }
    }
}
