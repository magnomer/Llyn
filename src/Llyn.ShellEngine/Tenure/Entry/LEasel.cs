using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEasel
{
    private readonly LTenure _lEaselTenure;

    public LEasel(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lEaselTenure = tenure;
    }

    public void LEaselImageAdd(LCardKind kind, long card)
    {
        if (_lEaselTenure.LTenureRead() is not LDraft held)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestImageAddition(
            _lEaselTenure.LTenureId,
            kind,
            card,
            LStateWritten.LStateWrittenEmpty,
            LDraftClerkMedia.LImageEndRead(held, kind, card)));
    }

    public void LEaselImageRemove(LCardKind kind, long card, long image)
    {
        if (_lEaselTenure.LTenureRead() is null)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestImageRemoval(_lEaselTenure.LTenureId, kind, card, image));
    }

    public void LEaselImageSet(long image, string location, bool deferred)
    {
        LEaselRequestRun(
            new LRequestImageLocation(_lEaselTenure.LTenureId, image, new LStateWritten(location)), deferred);
    }

    public void LEaselVideoAdd(LCardKind kind, long card)
    {
        if (_lEaselTenure.LTenureRead() is not LDraft held)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestVideoAddition(
            _lEaselTenure.LTenureId,
            kind,
            card,
            LStateWritten.LStateWrittenEmpty,
            LDraftClerkMedia.LVideoEndRead(held, kind, card)));
    }

    public void LEaselVideoRemove(LCardKind kind, long card, long video)
    {
        if (_lEaselTenure.LTenureRead() is null)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestVideoRemoval(_lEaselTenure.LTenureId, kind, card, video));
    }

    public void LEaselVideoSet(long video, string location, bool deferred)
    {
        LEaselRequestRun(
            new LRequestVideoLocation(_lEaselTenure.LTenureId, video, new LStateWritten(location)), deferred);
    }

    public void LEaselSpanSet(long video, string span)
    {
        _lEaselTenure.LTenureRequestDefer(
            new LRequestVideoSpan(_lEaselTenure.LTenureId, video, new LStateWritten(span)));
    }

    private void LEaselRequestRun(LRequest request, bool deferred)
    {
        if (deferred)
        {
            _lEaselTenure.LTenureRequestDefer(request);
            return;
        }

        _lEaselTenure.LTenureRequestApply(request);
    }
}
