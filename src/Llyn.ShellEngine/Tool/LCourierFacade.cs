using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LCourierFacade
{
    private readonly LEngine _lCourierFacadeEngine;

    public LCourierFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lCourierFacadeEngine = engine;
    }

    private LEngineStaff LCourierFacadeStaff => _lCourierFacadeEngine.LEngineStaffHeld;

    public bool LEngineCourierCheck()
    {
        return LCourierFacadeStaff.LEngineStaffCourier.LCourierClerkCheck();
    }

    public async Task<LReceipt> LEngineCourierSend(Func<string, string> lookup, CancellationToken cancellation)
    {
        try
        {
            LEngine engine = _lCourierFacadeEngine;
            return await LCourierFacadeStaff.LEngineStaffCourier.LCourierClerkSend(
                engine.LEngineLivery.LEngineLiveryRead,
                language => engine.LEngineLivery.LEngineLiveryRead(language, engine.LEngineSettings.LEngineTextFind),
                lookup,
                cancellation)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (LCourierClerk.LCourierWarrantCheck(exception))
        {
            LEngineWarrantClear();
            throw;
        }
    }

    public async Task LEngineCourierAttach(CancellationToken cancellation)
    {
        string hidden = await LCourierFacadeStaff.LEngineStaffCourier.LCourierClerkAttach(cancellation)
            .ConfigureAwait(false);
        if (_lCourierFacadeEngine.LEngineSettings.LEngineSettingsChange(
                settings => settings with { LSettingsWarrant = hidden }))
        {
            _lCourierFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    private void LEngineWarrantClear()
    {
        if (_lCourierFacadeEngine.LEngineSettings.LEngineSettingsChange(
                settings => settings with { LSettingsWarrant = string.Empty }))
        {
            _lCourierFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }
}
