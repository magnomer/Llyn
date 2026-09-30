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

    public void LEaselImageAdd(long card)
    {
        if (_lEaselTenure.LTenureRead() is not LDraft held)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestImageAddition(
            _lEaselTenure.LTenureId,
            card,
            LStateWritten.LStateWrittenEmpty,
            LDraftClerkMedia.LImageEndRead(held, card)));
    }

    public void LEaselImageRemove(long image)
    {
        if (_lEaselTenure.LTenureRead() is LDraft held
            && LDraftClerkMedia.LImageCardFind(held, image) is long card)
        {
            _lEaselTenure.LTenureRequestApply(new LRequestImageRemoval(_lEaselTenure.LTenureId, card, image));
        }
    }

    public void LEaselImageSet(long image, string location, bool deferred)
    {
        LEaselRequestRun(
            new LRequestImageLocation(_lEaselTenure.LTenureId, image, new LStateWritten(location)), deferred);
    }

    public void LEaselVideoAdd(long card)
    {
        if (_lEaselTenure.LTenureRead() is not LDraft held)
        {
            return;
        }

        _lEaselTenure.LTenureRequestApply(new LRequestVideoAddition(
            _lEaselTenure.LTenureId,
            card,
            LStateWritten.LStateWrittenEmpty,
            LDraftClerkMedia.LVideoEndRead(held, card)));
    }

    public void LEaselVideoRemove(long video)
    {
        if (_lEaselTenure.LTenureRead() is LDraft held
            && LDraftClerkMedia.LVideoCardFind(held, video) is long card)
        {
            _lEaselTenure.LTenureRequestApply(new LRequestVideoRemoval(_lEaselTenure.LTenureId, card, video));
        }
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
