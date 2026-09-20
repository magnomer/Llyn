using System;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)
    {
        lock (_lEngineGate)
        {
            return _lEnginePortraitClerk.LPortraitClerkRead(entryId, label);
        }
    }

    internal LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        lock (_lEngineGate)
        {
            LPortraitPage? page = owner switch
            {
                LOwner.LOwnerExample => _lEngineExampleClerk.LExampleClerkRead(id, legend),
                LOwner.LOwnerReference => _lEngineReferenceClerk.LReferenceClerkRead(id, legend),
                LOwner.LOwnerSituation => _lEngineSituationClerk.LSituationClerkRead(id, legend),
                _ => throw LEngineOwnerRaise(owner),
            };

            return page ?? throw new InvalidOperationException("The page no longer stands in the workspace.");
        }
    }

    internal Task LEnginePortraitExport(
        long entryId, string path, LPortraitFormat format, LPortraitLabel label)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitFormat.LPortraitFormatMarkup)
        {
            lock (_lEngineGate)
            {
                _lEnginePortraitClerk.LPortraitMarkupExport(entryId, path);
            }

            return Task.CompletedTask;
        }

        return _lEnginePortraitClerk.LPortraitClerkExport(LEnginePortraitRead(entryId, label), path, format);
    }

    internal Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(ticket);

        return _lEnginePortraitClerk.LPortraitClerkPrint(LEnginePortraitRead(entryId, label), ticket);
    }

    public Task LEnginePortraitExport(
        LVista? vista, string path, LPortraitFormat format, LPortraitLabel label)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? LEnginePortraitExport(id, path, format, label)
            : Task.CompletedTask;
    }

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? LEnginePortraitPrint(id, label, ticket)
            : Task.CompletedTask;
    }

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket)
    {
        if (vista?.LVistaChosen is not long id)
        {
            return Task.CompletedTask;
        }

        LOwner owner = vista.LVistaSubject switch
        {
            LSubject.LSubjectExample => LOwner.LOwnerExample,
            LSubject.LSubjectSituation => LOwner.LOwnerSituation,
            LSubject.LSubjectReference => LOwner.LOwnerReference,
            _ => throw new ArgumentException("The vista does not hold a printable catalog subject.", nameof(vista)),
        };
        return LEnginePortraitPrint(id, owner, legend, ticket);
    }

    internal Task LEnginePortraitPrint(long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        return _lEnginePortraitClerk.LPortraitClerkPrint(LEnginePortraitRead(id, owner, legend), ticket);
    }
}
