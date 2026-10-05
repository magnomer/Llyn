using System;
using System.Threading;
using System.Threading.Tasks;
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

    public Task<LReceipt> LEngineCourierSend(LPortraitLabel label, CancellationToken cancellation)
    {
        return LCourierFacadeStaff.LEngineStaffCourier.LCourierClerkSend(label, cancellation);
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
}
