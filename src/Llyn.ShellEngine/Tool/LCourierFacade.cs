using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LCourierFacade
{
    private readonly LEngineHearth _lCourierFacadeHearth;
    private readonly LLiveryFacade _lCourierFacadeLivery;
    private readonly LSettingsFacade _lCourierFacadeSettings;

    public LCourierFacade(LEngineHearth hearth, LLiveryFacade livery, LSettingsFacade settings)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(settings);
        _lCourierFacadeHearth = hearth;
        _lCourierFacadeLivery = livery;
        _lCourierFacadeSettings = settings;
    }

    private LEngineStaff LCourierFacadeStaff => _lCourierFacadeHearth.LEngineStaffHeld;

    public bool LEngineCourierCheck()
    {
        return LCourierFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffCourier.LCourierClerkCheck();
    }

    public async Task<LReceipt> LEngineCourierSend(Func<string, string> lookup, CancellationToken cancellation)
    {
        try
        {
            return await LCourierFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffCourier.LCourierClerkSend(
                _lCourierFacadeLivery.LEngineLiveryRead,
                language => _lCourierFacadeLivery.LEngineLiveryRead(language, _lCourierFacadeSettings.LEngineTextFind),
                lookup,
                cancellation)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (LCourierWarrant.LCourierWarrantCheck(exception))
        {
            LEngineWarrantClear();
            throw;
        }
    }

    public async Task LEngineCourierAttach(CancellationToken cancellation)
    {
        string hidden = await LCourierFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffCourier
            .LCourierClerkAttach(cancellation)
            .ConfigureAwait(false);
        if (_lCourierFacadeSettings.LEngineSettingsChange(
                settings => settings with { LSettingsWarrant = hidden }))
        {
            _lCourierFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    private void LEngineWarrantClear()
    {
        if (_lCourierFacadeSettings.LEngineSettingsChange(
                settings => settings with { LSettingsWarrant = string.Empty }))
        {
            _lCourierFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }
}
