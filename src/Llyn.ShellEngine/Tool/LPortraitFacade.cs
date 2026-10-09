using System;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LPortraitFacade
{
    private readonly LEngineHearth _lPortraitFacadeHearth;
    private readonly object _lPortraitFacadeGate;

    public LPortraitFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lPortraitFacadeHearth = hearth;

        _lPortraitFacadeGate = _lPortraitFacadeHearth.LEngineGate;
    }

    internal LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)
    {
        lock (_lPortraitFacadeGate)
        {
            return LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPortrait
                .LPortraitClerkRead(entryId, label);
        }
    }

    internal LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        lock (_lPortraitFacadeGate)
        {
            LPortraitPage? page = owner switch
            {
                LOwner.LOwnerExample => LPortraitFacadeStaff.LEngineStaffCatalog.LCatalogStaffExample
                    .LExampleClerkRead(id, legend),
                LOwner.LOwnerReference => LPortraitFacadeStaff.LEngineStaffCatalog.LCatalogStaffReference
                    .LReferenceClerkRead(id, legend),
                LOwner.LOwnerSituation => LPortraitFacadeStaff.LEngineStaffCatalog.LCatalogStaffSituation
                    .LSituationClerkRead(id, legend),
                _ => throw LCardFacade.LEngineOwnerRaise(owner),
            };

            return page ?? throw new InvalidOperationException("The page no longer stands in the workspace.");
        }
    }

    internal Task LEnginePortraitExport(
        long entryId, string path, LPortraitMedium format, LPortraitLabel label)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitMedium.LPortraitMediumMarkup)
        {
            lock (_lPortraitFacadeGate)
            {
                LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffMarkup.LMarkupClerkExport([entryId], path);
            }

            return Task.CompletedTask;
        }

        return LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress.LPortraitClerkExport(
            LEnginePortraitRead(entryId, label), path, format);
    }

    internal Task LEnginePortraitPrint(long entryId, LPortraitLabel label, LPressTicket ticket)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(ticket);

        return LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress
            .LPortraitClerkPrint(LEnginePortraitRead(entryId, label), ticket);
    }

    public Task LEnginePortraitExport(
        LVista? vista, string path, LPortraitMedium format, LPortraitLabel label)
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

        return LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress
            .LPortraitClerkPrint(LEnginePortraitRead(id, owner, legend), ticket);
    }

    private LEngineStaff LPortraitFacadeStaff => _lPortraitFacadeHearth.LEngineStaffHeld;
}
