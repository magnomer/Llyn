using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LPortraitOutlet : LPortraitPort
{
    private readonly LEngine _lPortraitOutletEngine;

    public LPortraitOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lPortraitOutletEngine = engine;
    }

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket) =>
        _lPortraitOutletEngine.LEnginePortrait.LEnginePortraitPrint(vista, label, ticket);

    public Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket) =>
        _lPortraitOutletEngine.LEnginePortrait.LEnginePortraitPrint(vista, legend, ticket);

    public Task LEnginePortraitExport(LVista? vista, string path, LPortraitFormat format, LPortraitLabel label) =>
        _lPortraitOutletEngine.LEnginePortrait.LEnginePortraitExport(vista, path, format, label);

    public Task<LMarkupCargo> LEngineMarkupStart(string path) =>
        _lPortraitOutletEngine.LEngineMarkup.LEngineMarkupStart(path);

    public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes) =>
        _lPortraitOutletEngine.LEngineMarkup.LEngineMarkupStart(cargo, intakes);
}
