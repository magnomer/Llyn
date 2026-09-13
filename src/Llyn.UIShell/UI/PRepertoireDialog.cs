using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private readonly ObservableCollection<PImage> _pScenarioImage = [];

    private readonly ObservableCollection<PVideo> _pScenarioVideo = [];

    private readonly Dictionary<string, LRequest> _pScenarioRequestPending = [];

    private static string PScenarioRequestFormat(string kind, long rowId, string field)
    {
        return string.Concat(kind, ":", rowId.ToString(CultureInfo.InvariantCulture), ":", field);
    }

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
                row.PImageShow(draft, PScenarioRequestCheck(PScenarioImageFormat(row)));
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
                row.PVideoShow(draft, field => PScenarioRequestCheck(PScenarioVideoFormat(row, field)));
                return row;
            });
    }

    private static string PScenarioImageFormat(PImage row)
    {
        return PScenarioRequestFormat(nameof(PImage), row.PImageId, nameof(PImage.PImageLocation));
    }

    private static string PScenarioVideoFormat(PVideo row, string field)
    {
        return PScenarioRequestFormat(nameof(PVideo), row.PVideoId, field);
    }

    private PImage PScenarioImageCreate(LImageDraft draft)
    {
        PImage row = new(draft);
        row.PropertyChanged += PScenarioImageChange;
        return row;
    }

    private PVideo PScenarioVideoCreate(LVideoDraft draft)
    {
        PVideo row = new(draft);
        row.PropertyChanged += PScenarioVideoChange;
        return row;
    }

    private void PScenarioImageChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PImage row
            && string.Equals(arguments.PropertyName, nameof(PImage.PImageLocation), StringComparison.Ordinal))
        {
            PScenarioRequestDefer(
                PScenarioImageFormat(row),
                new LRequestImageLocation(_pScenarioDraft, row.PImageId, row.PImageLocationRead()));
        }
    }

    private void PScenarioVideoChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is not PVideo row
            || arguments.PropertyName is not (nameof(PVideo.PVideoLocation) or nameof(PVideo.PVideoTimestamp)))
        {
            return;
        }

        PScenarioRequestDefer(
            PScenarioVideoFormat(row, arguments.PropertyName),
            arguments.PropertyName == nameof(PVideo.PVideoLocation)
                ? new LRequestVideoLocation(_pScenarioDraft, row.PVideoId, row.PVideoLocationRead())
                : new LRequestVideoSpan(_pScenarioDraft, row.PVideoId, row.PVideoSpanRead()));
    }

    private void PImageAddHandle(object sender, RoutedEventArgs e)
    {
        PScenarioRequestSend(new LRequestImageAddition(
            _pScenarioDraft, 0, LStateWritten.LStateWrittenEmpty, _pScenarioImage.Count));
    }

    private void PVideoAddHandle(object sender, RoutedEventArgs e)
    {
        PScenarioRequestSend(new LRequestVideoAddition(
            _pScenarioDraft, 0, LStateWritten.LStateWrittenEmpty, _pScenarioVideo.Count));
    }

    public void PImageRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row })
        {
            PScenarioRequestSend(new LRequestImageRemoval(_pScenarioDraft, 0, row.PImageId));
        }
    }

    public void PVideoRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row })
        {
            PScenarioRequestSend(new LRequestVideoRemoval(_pScenarioDraft, 0, row.PVideoId));
        }
    }

    public void PImageOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PImage row } && PImage.PImageOpen(_pRepertoireHost) is string chosen)
        {
            row.PImageLocation = chosen;
        }
    }

    public void PVideoOpenHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PVideo row } && PVideo.PVideoOpen(_pRepertoireHost) is string chosen)
        {
            row.PVideoLocation = chosen;
        }
    }

    private bool PScenarioRequestCheck(string key)
    {
        return _pScenarioRequestPending.ContainsKey(key);
    }

    private void PScenarioRequestDefer(string key, LRequest request)
    {
        if (_pScenarioLoading || _pScenarioHalted || _pScenarioDraft == 0)
        {
            return;
        }

        _pScenarioRequestPending[key] = request;
        PScenarioChangeDefer();
    }

    private void PScenarioRequestSend(LRequest request)
    {
        if (_pScenarioLoading || _pScenarioHalted || _pScenarioDraft == 0)
        {
            return;
        }

        PScenarioChangeSave();

        if (_pScenarioHalted)
        {
            return;
        }

        try
        {
            _lEngine.LEngineRequestApply(request);
        }
        catch (Exception exception)
        {
            PScenarioHoldSuspend(exception);
            return;
        }

        PScenarioChangeUpdate();
    }

    private void PScenarioRequestPersist()
    {
        List<string> keys = [.. _pScenarioRequestPending.Keys];
        foreach (string key in keys)
        {
            if (_pScenarioRequestPending.TryGetValue(key, out LRequest? request))
            {
                _lEngine.LEngineRequestApply(request);
                _pScenarioRequestPending.Remove(key);
            }
        }
    }
}
