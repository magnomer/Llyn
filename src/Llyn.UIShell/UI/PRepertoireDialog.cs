using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PImage> _pScenarioImage = [];

    private readonly ObservableCollection<PVideo> _pScenarioVideo = [];

    private void PScenarioImageShow(IReadOnlyList<LImageDraft> rows)
    {
        PCard.PCardRowShow(
            _pScenarioImage,
            rows,
            static row => row.PImageId,
            static draft => draft.LImageDraftId,
            PScenarioImageCreate,
            (row, draft) =>
            {
                row.PImageShow(draft);
                return row;
            });
    }

    private void PScenarioVideoShow(IReadOnlyList<LVideoDraft> rows)
    {
        PCard.PCardRowShow(
            _pScenarioVideo,
            rows,
            static row => row.PVideoId,
            static draft => draft.LVideoDraftId,
            PScenarioVideoCreate,
            (row, draft) =>
            {
                row.PVideoShow(draft);
                return row;
            });
    }

    private static PImage PScenarioImageCreate(LImageDraft draft)
    {
        return new PImage(draft);
    }

    private static PVideo PScenarioVideoCreate(LVideoDraft draft)
    {
        return new PVideo(draft);
    }

    private void PScenarioImageChange(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { IsKeyboardFocusWithin: true, DataContext: PImage row } box)
        {
            PScenarioRequestDefer(
                new LRequestImageLocation(PScenarioDraft, row.PImageId, new LStateWritten(box.Text)));
        }
    }

    private void PScenarioVideoChange(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is not TextBox { IsKeyboardFocusWithin: true, DataContext: PVideo row } box)
        {
            return;
        }

        LStateWritten written = new(box.Text);
        PScenarioRequestDefer(
            PEditor.PEditorFieldRead(box) == nameof(PVideo.PVideoLocation)
                ? new LRequestVideoLocation(PScenarioDraft, row.PVideoId, written)
                : new LRequestVideoSpan(PScenarioDraft, row.PVideoId, written));
    }

    private void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        PScenarioRequestSend(new LRequestImageAddition(
            PScenarioDraft, 0, LStateWritten.LStateWrittenEmpty, _pScenarioImage.Count));
    }

    private void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        PScenarioRequestSend(new LRequestVideoAddition(
            PScenarioDraft, 0, LStateWritten.LStateWrittenEmpty, _pScenarioVideo.Count));
    }

    public void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            PScenarioRequestSend(new LRequestImageRemoval(PScenarioDraft, 0, row.PImageId));
        }
    }

    public void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            PScenarioRequestSend(new LRequestVideoRemoval(PScenarioDraft, 0, row.PVideoId));
        }
    }

    public void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row }
            && PImage.PImageOpen(_pRepertoireHost) is string chosen)
        {
            PScenarioRequestSend(new LRequestImageLocation(PScenarioDraft, row.PImageId, new LStateWritten(chosen)));
        }
    }

    public void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row }
            && PVideo.PVideoOpen(_pRepertoireHost) is string chosen)
        {
            PScenarioRequestSend(new LRequestVideoLocation(PScenarioDraft, row.PVideoId, new LStateWritten(chosen)));
        }
    }
}
