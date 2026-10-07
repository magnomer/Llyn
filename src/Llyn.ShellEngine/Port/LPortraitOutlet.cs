using System;
using System.Collections.Generic;
using System.Threading;
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

    public Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label) =>
        _lPortraitOutletEngine.LEnginePortrait.LEnginePortraitExport(vista, path, format, label);

    public Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(
        string path,
        Func<
            IReadOnlyList<LMarkupEntry>,
            IReadOnlyList<IReadOnlyList<LMarkupTarget>>,
            IReadOnlyList<LMarkupIntake>?> declare) =>
        _lPortraitOutletEngine.LEngineMarkup.LEngineMarkupStart(path, declare);

    public bool LEngineCourierCheck() => _lPortraitOutletEngine.LEngineCourier.LEngineCourierCheck();

    public Task<LReceipt> LEngineCourierSend(Func<string, string> lookup, CancellationToken cancellation) =>
        _lPortraitOutletEngine.LEngineCourier.LEngineCourierSend(lookup, cancellation);

    public Task LEngineCourierAttach(CancellationToken cancellation) =>
        _lPortraitOutletEngine.LEngineCourier.LEngineCourierAttach(cancellation);
}
