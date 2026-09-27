using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class QEasel
{
    private readonly LDesk _qEaselDesk;

    internal QEasel(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _qEaselDesk = desk;
    }

    public void QEaselImageAdd(long card, int position)
    {
        _qEaselDesk.LDeskSend(
            new LRequestImageAddition(_qEaselDesk.LDeskId, card, LStateWritten.LStateWrittenEmpty, position));
    }

    public void QEaselImageRemove(long card, long image)
    {
        _qEaselDesk.LDeskSend(new LRequestImageRemoval(_qEaselDesk.LDeskId, card, image));
    }

    public void QEaselImageSet(long image, string location, bool deferred)
    {
        QEaselRequestRun(
            new LRequestImageLocation(_qEaselDesk.LDeskId, image, new LStateWritten(location)), deferred);
    }

    public void QEaselVideoAdd(long card, int position)
    {
        _qEaselDesk.LDeskSend(
            new LRequestVideoAddition(_qEaselDesk.LDeskId, card, LStateWritten.LStateWrittenEmpty, position));
    }

    public void QEaselVideoRemove(long card, long video)
    {
        _qEaselDesk.LDeskSend(new LRequestVideoRemoval(_qEaselDesk.LDeskId, card, video));
    }

    public void QEaselVideoSet(long video, string location, bool deferred)
    {
        QEaselRequestRun(
            new LRequestVideoLocation(_qEaselDesk.LDeskId, video, new LStateWritten(location)), deferred);
    }

    public void QEaselSpanSet(long video, string span)
    {
        _qEaselDesk.LDeskDefer(new LRequestVideoSpan(_qEaselDesk.LDeskId, video, new LStateWritten(span)));
    }

    private void QEaselRequestRun(LRequest request, bool deferred)
    {
        if (deferred)
        {
            _qEaselDesk.LDeskDefer(request);
            return;
        }

        _qEaselDesk.LDeskSend(request);
    }
}
