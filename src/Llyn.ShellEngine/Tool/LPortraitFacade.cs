using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LPortraitFacade
{
    private readonly LEngineHearth _lPortraitFacadeHearth;
    private readonly LLiveryFacade _lPortraitFacadeLivery;
    private readonly LSettingsFacade _lPortraitFacadeSettings;
    private readonly object _lPortraitFacadeGate;

    public LPortraitFacade(LEngineHearth hearth, LLiveryFacade livery, LSettingsFacade settings)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(settings);
        _lPortraitFacadeHearth = hearth;
        _lPortraitFacadeLivery = livery;
        _lPortraitFacadeSettings = settings;

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

        LPortraitPage portrait = LEnginePortraitRead(entryId, label);
        LPortraitClerkPress press = LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress;
        if (format is not (LPortraitMedium.LPortraitMediumHtml or LPortraitMedium.LPortraitMediumPdf))
        {
            return press.LPortraitClerkExport(portrait, path, format);
        }

        LLiveryPage livery = _lPortraitFacadeLivery.LEngineLiveryRead(entryId, true)
            ?? throw new InvalidOperationException("The page no longer stands in the workspace.");
        return press.LPortraitClerkExport(
            livery, portrait, label, _lPortraitFacadeSettings.LEngineTextRead, path, format);
    }

    public Task LEnginePortraitExport(
        LVista? vista, string path, LPortraitMedium format, LPortraitLabel label)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? LEnginePortraitExport(id, path, format, label)
            : Task.CompletedTask;
    }

    internal async Task<LGalleyRelay> LEnginePressOpen(long entryId, LPortraitLabel label)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);

        LPortraitPage portrait = LEnginePortraitRead(entryId, label);
        LLiveryPage livery = _lPortraitFacadeLivery.LEngineLiveryRead(entryId, true)
            ?? throw new InvalidOperationException("The page no longer stands in the workspace.");
        string file = _lPortraitFacadeSettings.LEngineFileFormat(portrait.LPortraitPageTitle);
        LPortraitClerkPress clerk = LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress;
        LGalleyClerk galley = await clerk.LPortraitClerkOpen(
            livery, portrait, label, _lPortraitFacadeSettings.LEngineTextRead);
        return new LGalleyRelay(
            galley, new LGalley(galley, clerk, LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffGalley, file));
    }

    public async Task<LGalleyRelay?> LEnginePressOpen(LVista? vista, LPortraitLabel label)
    {
        return vista?.LVistaSubject == LSubject.LSubjectEntry && vista.LVistaChosen is long id
            ? await LEnginePressOpen(id, label)
            : null;
    }

    public async Task<LGalleyRelay?> LEnginePressOpen(LVista? vista, LPortraitLegend legend)
    {
        if (vista?.LVistaChosen is not long id)
        {
            return null;
        }

        LOwner owner = vista.LVistaSubject switch
        {
            LSubject.LSubjectExample => LOwner.LOwnerExample,
            LSubject.LSubjectSituation => LOwner.LOwnerSituation,
            LSubject.LSubjectReference => LOwner.LOwnerReference,
            _ => throw new ArgumentException("The vista does not hold a printable catalog subject.", nameof(vista)),
        };
        return await LEnginePressOpen(id, owner, legend);
    }

    internal async Task<LGalleyRelay> LEnginePressOpen(long id, LOwner owner, LPortraitLegend legend)
    {
        LPortraitPage page = LEnginePortraitRead(id, owner, legend);
        string file = _lPortraitFacadeSettings.LEngineFileFormat(page.LPortraitPageTitle);
        LPortraitClerkPress clerk = LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffPress;
        LGalleyClerk galley = await clerk.LPortraitClerkOpen(page);
        return new LGalleyRelay(
            galley, new LGalley(galley, clerk, LPortraitFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffGalley, file));
    }

    private LEngineStaff LPortraitFacadeStaff => _lPortraitFacadeHearth.LEngineStaffHeld;
}
