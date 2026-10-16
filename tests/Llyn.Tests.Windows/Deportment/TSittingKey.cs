using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TSittingKey
{
    [Fact]
    public void SittingKeyObserve_Space_TurnsCard()
    {
        TSittingKeyRun((study, surface) =>
        {
            bool handled = TSittingKeyRaise(surface, Key.Space);

            CSittingState state = study.CStudySitting!.CSittingRead();
            ContentControl back = QContract.QContractFind<ContentControl>(surface, "PSittingBack");
            Assert.True(handled);
            Assert.True(state.CSittingStateTurned);
            Assert.Equal("1 / 2", state.CSittingStatePosition);
            Assert.Equal(Visibility.Visible, back.Visibility);
        });
    }

    [Fact]
    public void SittingKeyObserve_RightOnTurnedCard_AdvancesUnturned()
    {
        TSittingKeyRun((study, surface) =>
        {
            TSittingKeyRaise(surface, Key.Space);

            bool handled = TSittingKeyRaise(surface, Key.Right);

            CSittingState state = study.CStudySitting!.CSittingRead();
            Assert.True(handled);
            Assert.False(state.CSittingStateTurned);
            Assert.Equal("2 / 2", state.CSittingStatePosition);
            Assert.Equal("2 / 2", QContract.QContractFind<TextBlock>(surface, "PSittingPosition").Text);
        });
    }

    [Fact]
    public void SittingKeyObserve_LeftOnSecondCard_RetreatsToFirst()
    {
        TSittingKeyRun((study, surface) =>
        {
            TSittingKeyRaise(surface, Key.Right);

            bool handled = TSittingKeyRaise(surface, Key.Left);

            CSittingState state = study.CStudySitting!.CSittingRead();
            Assert.True(handled);
            Assert.Equal("1 / 2", state.CSittingStatePosition);
            Assert.Equal("1 / 2", QContract.QContractFind<TextBlock>(surface, "PSittingPosition").Text);
        });
    }

    [Fact]
    public void SittingEscapeObserve_RunningSitting_EndsSitting()
    {
        TSittingKeyRun((study, surface) =>
        {
            bool handled = TSittingKeyRaise(surface, Key.Escape);

            Assert.True(handled);
            Assert.Null(study.CStudySitting);
        });
    }

    private static void TSittingKeyRun(Action<CStudy, Border> body)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LDeckOutlet port = TInterface.TDeckOutletCreate(engine);
        long deck = port.TEngineDeckCreate("Study", null, LGrain.LGrainMeaning);
        foreach (string front in new[] { "one", "two" })
        {
            port.TEngineCueCreate(
                deck,
                [
                    TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 1, front),
                    TInterface.TCueCellCreate(LCueColumn.LCueColumnText, 2, front + " back"),
                ]);
        }

        CStudy study = new(atelier, TEnvoyFake.TEnvoyCreate(false, []), new Random(7).Next);
        study.CStudySelect(deck);
        study.CStudyStart();

        TWindow.TWindowRun(() =>
        {
            System.Windows.Application application =
                System.Windows.Application.Current ?? new System.Windows.Application();
            application.Resources["PIconRoot"] = "pack://application:,,,/Llyn.Tests.Windows;component/icons/";
            application.Resources["Theme.Popup.ProgressBar.Sweep"] = new DoubleAnimation().GetAsFrozen();
            application.Resources["Theme.Sound.Rebuild.Spin"] = new DoubleAnimation().GetAsFrozen();
            Border surface = TSittingKeyCreate();
            QSitting sitting = new(surface);
            sitting.QSittingIntroduce(study);
            sitting.QSittingRefine(study.CStudySitting!.CSittingRead());
            using HwndSource host = new(new HwndSourceParameters("TSittingKey", 400, 400)) { RootVisual = surface };
            body(study, surface);
        });
    }

    private static Border TSittingKeyCreate()
    {
        StackPanel faces = new();
        faces.Children.Add(new TextBlock { Name = "PSittingPosition" });
        faces.Children.Add(new ContentControl { Name = "PSittingFront", Focusable = false });
        faces.Children.Add(new Border { Name = "PSittingRule" });
        faces.Children.Add(new ContentControl { Name = "PSittingBack", Focusable = false });
        faces.Children.Add(new TextBlock { Name = "PSittingBlank" });
        faces.Children.Add(new TextBlock { Name = "PSittingSummary" });
        foreach (string name in new[]
            { "PSittingPrevious", "PSittingSide", "PSittingNext", "PSittingAgain", "PSittingEnd" })
        {
            faces.Children.Add(new Button { Name = name, Focusable = false });
        }

        return new Border { Name = "PSitting", Focusable = true, Visibility = Visibility.Collapsed, Child = faces };
    }

    private static bool TSittingKeyRaise(Border surface, Key key)
    {
        KeyEventArgs press = new(Keyboard.PrimaryDevice, PresentationSource.FromVisual(surface), 0, key)
        {
            RoutedEvent = Keyboard.KeyDownEvent,
        };
        surface.RaiseEvent(press);
        return press.Handled;
    }
}
