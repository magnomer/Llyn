using System;
using System.Windows.Threading;

namespace Llyn.UIShell;

public partial class PEditor
{
    private DispatcherTimer? _pEditorClock;

    private void PEditorChangeStart()
    {
        _pEditorClock ??= PEditorClockCreate();
        _pEditorClock.Start();
        PEditorChangeUpdate();
    }

    private void PEditorChangeStop()
    {
        _pEditorClock?.Stop();
    }

    private DispatcherTimer PEditorClockCreate()
    {
        DispatcherTimer clock = new(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };

        clock.Tick += (_, _) => PEditorChangeUpdate();
        return clock;
    }

    private void PEditorChangeUpdate()
    {
        bool changed = PEditorChangeCheck();
        PEditorDiscard.IsEnabled = changed;
        PEditorStore.IsEnabled = changed;
    }
}
